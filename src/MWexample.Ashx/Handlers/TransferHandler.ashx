<%@ WebHandler Language="C#" Class="TransferHandler" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.IO;
using MWexample.Helper.Models;

/// <summary>
/// Para transferi API Handler'ı
/// Hesaplar arası para transferi işlemlerini gerçekleştiren HTTP endpoint'i
/// 
/// API Endpoint: /Handlers/TransferHandler.ashx
/// Method: POST
/// Content-Type: application/json
/// 
/// Örnek İstek:
/// POST /Handlers/TransferHandler.ashx
/// Content-Type: application/json
/// {
///   "FromAccountId": 1,
///   "ToAccountId": 2,
///   "Amount": 1000.00,
///   "Description": "TL'den USD'ye transfer"
/// }
/// 
/// Yanıt Formatı:
/// {
///   "success": true,
///   "data": {
///     "Success": true,
///     "Message": "Transfer başarılı",
///     "TransactionId": 1,
///     "ConvertedAmount": 34.00
///   }
/// }
/// </summary>
public class TransferHandler : IHttpHandler
{
    /// <summary>
    /// HTTP isteklerini işleyen ana metod
    /// CORS headers ekler ve JSON yanıt döner
    /// </summary>
    /// <param name="context">HTTP context nesnesi</param>
    public void ProcessRequest(HttpContext context)
    {
        // JSON yanıt formatı ve CORS headers ayarla
        context.Response.ContentType = "application/json";
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

        try
        {
            // HTTP metodunu al
            var method = context.Request.HttpMethod;

            // HTTP metoduna göre işlem yap
            switch (method.ToUpper())
            {
                case "POST":
                    // Para transferi işlemini gerçekleştir
                    ProcessTransferRequest(context);
                    break;

                case "OPTIONS":
                    // CORS preflight request - sadece 200 döndür
                    context.Response.StatusCode = 200;
                    break;

                default:
                    // Desteklenmeyen HTTP metodu
                    WriteErrorResponse(context, "Desteklenmeyen HTTP metodu");
                    break;
            }
        }
        catch (Exception ex)
        {
            // Beklenmeyen hata durumunda log ve yanıt
            WriteErrorResponse(context, $"Sunucu hatası: {ex.Message}");
        }
    }

    /// <summary>
    /// POST isteğindeki transfer verilerini işler
    /// JSON deserialization, validasyon ve transfer işlemi yapar
    /// </summary>
    /// <param name="context">HTTP context nesnesi</param>
    private void ProcessTransferRequest(HttpContext context)
    {
        try
        {
            // Request body'den JSON verisini oku
            string requestBody;
            using (var reader = new StreamReader(context.Request.InputStream))
            {
                requestBody = reader.ReadToEnd();
            }

            // Request body boş kontrolü
            if (string.IsNullOrEmpty(requestBody))
            {
                WriteErrorResponse(context, "Transfer verisi gerekli");
                return;
            }

            // JSON'u TransferRequest objesine deserialize et
            var serializer = new JavaScriptSerializer();
            var transferRequest = serializer.Deserialize<TransferRequest>(requestBody);

            // Transfer verilerini validate et
            if (transferRequest.FromAccountId <= 0 || transferRequest.ToAccountId <= 0)
            {
                WriteErrorResponse(context, "Geçersiz hesap ID'leri");
                return;
            }

            if (transferRequest.Amount <= 0)
            {
                WriteErrorResponse(context, "Transfer tutarı 0'dan büyük olmalıdır");
                return;
            }

            if (transferRequest.FromAccountId == transferRequest.ToAccountId)
            {
                WriteErrorResponse(context, "Aynı hesaba transfer yapılamaz");
                return;
            }

            // BankingService ile transfer işlemini gerçekleştir
            var bankingService = new BankingService();
            var result = bankingService.TransferMoney(transferRequest);

            // Transfer sonucuna göre yanıt döndür
            if (result.Success)
            {
                WriteSuccessResponse(context, result);
            }
            else
            {
                WriteErrorResponse(context, result.Message);
            }
        }
        catch (Exception ex)
        {
            // Transfer işlemi sırasında oluşan hataları yakala
            WriteErrorResponse(context, $"Transfer işlemi hatası: {ex.Message}");
        }
    }

    /// <summary>
    /// Başarılı işlem sonucunu JSON formatında yazar
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="data">Döndürülecek veri</param>
    private void WriteSuccessResponse(HttpContext context, object data)
    {
        var serializer = new JavaScriptSerializer();
        var json = serializer.Serialize(new { success = true, data = data });
        context.Response.Write(json);
    }

    /// <summary>
    /// Hata durumunu JSON formatında yazar
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="message">Hata mesajı</param>
    private void WriteErrorResponse(HttpContext context, string message)
    {
        var serializer = new JavaScriptSerializer();
        var json = serializer.Serialize(new { success = false, message = message });
        context.Response.Write(json);
    }

    /// <summary>
    /// Handler'ın yeniden kullanılabilir olup olmadığını belirtir
    /// </summary>
    public bool IsReusable
    {
        get { return false; }
    }
}
