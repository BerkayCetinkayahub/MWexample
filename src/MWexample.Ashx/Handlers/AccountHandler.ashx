<%@ WebHandler Language="C#" Class="AccountHandler" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using MWexample.Helper.Models;

/// <summary>
/// Hesap bilgileri API Handler'ı
/// Kullanıcının hesaplarını listeleyen HTTP endpoint'i
/// 
/// API Endpoint: /Handlers/AccountHandler.ashx
/// Method: GET
/// Parameters: userId (query string)
/// 
/// Örnek Kullanım:
/// GET /Handlers/AccountHandler.ashx?userId=1
/// 
/// Yanıt Formatı:
/// {
///   "success": true,
///   "data": [
///     {
///       "AccountId": 1,
///       "AccountNumber": "TR123456",
///       "Currency": 1,
///       "Balance": 50000.00,
///       "CurrencySymbol": "₺"
///     }
///   ]
/// }
/// </summary>
public class AccountHandler : IHttpHandler
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
            // HTTP metodunu ve parametreleri al
            var method = context.Request.HttpMethod;
            var userId = context.Request.QueryString["userId"];

            // Kullanıcı ID validasyonu
            if (string.IsNullOrEmpty(userId))
            {
                WriteErrorResponse(context, "Kullanıcı ID gerekli");
                return;
            }

            // Kullanıcı ID'nin sayısal olup olmadığını kontrol et
            if (!int.TryParse(userId, out int userIdInt))
            {
                WriteErrorResponse(context, "Geçersiz kullanıcı ID");
                return;
            }

            // BankingService instance'ı oluştur
            var bankingService = new BankingService();

            // HTTP metoduna göre işlem yap
            switch (method.ToUpper())
            {
                case "GET":
                    // Kullanıcının hesaplarını getir
                    var accounts = bankingService.GetUserAccounts(userIdInt);
                    WriteSuccessResponse(context, accounts);
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
