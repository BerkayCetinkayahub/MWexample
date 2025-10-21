using System;

namespace MWexample.Helper.Models
{
    /// <summary>
    /// Döviz kuru bilgilerini temsil eden model sınıfı
    /// İki para birimi arasındaki çevrim oranını saklar
    /// </summary>
    public class ExchangeRate
    {
        /// <summary>
        /// Kaynak para birimi (çevrilecek para)
        /// </summary>
        public CurrencyType FromCurrency { get; set; }

        /// <summary>
        /// Hedef para birimi (çevrilen para)
        /// </summary>
        public CurrencyType ToCurrency { get; set; }

        /// <summary>
        /// Çevrim oranı (1 FromCurrency = Rate ToCurrency)
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Kurun son güncellenme tarihi
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Para transferi isteğini temsil eden model sınıfı
    /// Client'tan gelen transfer taleplerini karşılar
    /// </summary>
    public class TransferRequest
    {
        /// <summary>
        /// Para çekilecek hesabın kimlik numarası
        /// </summary>
        public int FromAccountId { get; set; }

        /// <summary>
        /// Para yatırılacak hesabın kimlik numarası
        /// </summary>
        public int ToAccountId { get; set; }

        /// <summary>
        /// Transfer edilecek tutar (kaynak para biriminde)
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Transfer açıklaması (opsiyonel)
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Hesap bakiyesi bilgilerini temsil eden model sınıfı
    /// Client'a gönderilen hesap özeti için kullanılır
    /// </summary>
    public class AccountBalance
    {
        /// <summary>
        /// Hesabın kimlik numarası
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Hesap numarası
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Hesabın para birimi
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// Hesaptaki mevcut bakiye
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Para birimi sembolü (₺, $, €)
        /// UI'da gösterim için kullanılır
        /// </summary>
        public string CurrencySymbol { get; set; }
    }
}
