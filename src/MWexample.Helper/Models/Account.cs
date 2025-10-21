using System;

namespace MWexample.Helper.Models
{
    /// <summary>
    /// Bankacılık hesap bilgilerini temsil eden model sınıfı
    /// Kullanıcının farklı para birimlerindeki hesaplarını yönetir
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Hesabın benzersiz kimlik numarası
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Hesabın sahibi olan kullanıcının kimlik numarası
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Hesap numarası (örn: TR123456, US123456, EU123456)
        /// Para birimine göre prefix alır
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Hesabın para birimi (TRY, USD, EUR)
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// Hesaptaki mevcut bakiye
        /// Decimal tipi para hesaplamalarında hassasiyet için kullanılır
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Hesabın oluşturulma tarihi
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Hesabın aktif olup olmadığını belirten flag
        /// Pasif hesaplar işlem yapamaz
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Desteklenen para birimlerini tanımlayan enum
    /// </summary>
    public enum CurrencyType
    {
        /// <summary>
        /// Türk Lirası (₺)
        /// </summary>
        TRY = 1,

        /// <summary>
        /// Amerikan Doları ($)
        /// </summary>
        USD = 2,

        /// <summary>
        /// Euro (€)
        /// </summary>
        EUR = 3
    }
}
