using System;

namespace MWexample.Ashx
{
    /// <summary>
    /// Banking API Server ana program sınıfı
    /// Uygulama başlatıldığında mevcut endpoint'leri gösterir
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Uygulama giriş noktası
        /// Server başlatıldığında bilgilendirme mesajları gösterir
        /// </summary>
        /// <param name="args">Komut satırı argümanları</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Banking API Server ===");
            Console.WriteLine("Server başlatılıyor...");
            Console.WriteLine();
            Console.WriteLine("Mevcut API Endpoint'leri:");
            Console.WriteLine("1. Hesap Listesi:");
            Console.WriteLine("   GET /Handlers/AccountHandler.ashx?userId=1");
            Console.WriteLine();
            Console.WriteLine("2. Para Transferi:");
            Console.WriteLine("   POST /Handlers/TransferHandler.ashx");
            Console.WriteLine("   Content-Type: application/json");
            Console.WriteLine();
            Console.WriteLine("3. Döviz Kurları:");
            Console.WriteLine("   GET /Handlers/ExchangeRateHandler.ashx");
            Console.WriteLine();
            Console.WriteLine("Çıkmak için herhangi bir tuşa basın...");
            Console.ReadKey();
        }
    }
}
