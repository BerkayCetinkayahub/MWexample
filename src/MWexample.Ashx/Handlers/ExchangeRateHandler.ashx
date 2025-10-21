<%@ WebHandler Language="C#" Class="ExchangeRateHandler" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using MWexample.Helper.Models;

/// <summary>
/// Döviz kurları API Handler'ı
/// Güncel döviz kurlarını döndüren HTTP endpoint'i
/// 
/// API Endpoint: /Handlers/ExchangeRateHandler.ashx
/// Method: GET
/// 
/// Örnek Kullanım:
/// GET /Handlers/ExchangeRateHandler.ashx
/// 
/// Yanıt Formatı:
/// {
///   "success": true,
///   "data": [
///     {
///       "FromCurrency": 1,
///       "ToCurrency": 2,
///       "Rate": 0.034,
///       "LastUpdated": "2024-01-15T10:30:00"
///     },
///     {
///       "FromCurrency": 1,
///       "ToCurrency": 3,
///       "Rate": 0.031,
///       "LastUpdated": "2024-01-15T10:30:00"
///     }
///   ]
/// }
/// 
/// Para Birimi Kodları:
/// 1 = TRY (Türk Lirası)
/// 2 = USD (Amerikan Doları)
/// 3 = EUR (Euro)
/// </summary>
public class ExchangeRateHandler : IHttpHandler
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
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, OPTIONS");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

        try
        {
            // HTTP metodunu al
            var method = context.Request.HttpMethod;

            // HTTP metoduna göre işlem yap
            switch (method.ToUpper())
            {
                case "GET":
                    // Döviz kurlarını getir
                    var bankingService = new BankingService();
                    var exchangeRates = bankingService.GetExchangeRates();
                    WriteSuccessResponse(context, exchangeRates);
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
