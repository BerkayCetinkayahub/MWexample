using System;

namespace MWexample.Helper.Models
{
    /// <summary>
    /// Bankacılık işlemlerini temsil eden model sınıfı
    /// Para transferleri, yatırma ve çekme işlemlerini kaydeder
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// İşlemin benzersiz kimlik numarası
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// Para çekilen hesabın kimlik numarası
        /// </summary>
        public int FromAccountId { get; set; }

        /// <summary>
        /// Para yatırılan hesabın kimlik numarası
        /// </summary>
        public int ToAccountId { get; set; }

        /// <summary>
        /// İşlem tutarı (kaynak para biriminde)
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// İşlemin yapıldığı para birimi
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// İşlem sırasında kullanılan döviz kuru
        /// Aynı para birimi transferlerinde 1.0 olur
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// İşlem türü (Transfer, Deposit, Withdrawal)
        /// </summary>
        public TransactionType Type { get; set; }

        /// <summary>
        /// İşlemin mevcut durumu
        /// </summary>
        public TransactionStatus Status { get; set; }

        /// <summary>
        /// İşlemin gerçekleştirilme tarihi
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// İşlem açıklaması veya notu
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// İşlem türlerini tanımlayan enum
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Hesaplar arası para transferi
        /// </summary>
        Transfer = 1,

        /// <summary>
        /// Hesaba para yatırma
        /// </summary>
        Deposit = 2,

        /// <summary>
        /// Hesaptan para çekme
        /// </summary>
        Withdrawal = 3
    }

    /// <summary>
    /// İşlem durumlarını tanımlayan enum
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// İşlem bekliyor (onay bekliyor)
        /// </summary>
        Pending = 1,

        /// <summary>
        /// İşlem başarıyla tamamlandı
        /// </summary>
        Completed = 2,

        /// <summary>
        /// İşlem başarısız oldu
        /// </summary>
        Failed = 3,

        /// <summary>
        /// İşlem iptal edildi
        /// </summary>
        Cancelled = 4
    }
}
