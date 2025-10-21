using System;
using System.Collections.Generic;
using System.Linq;
using MWexample.Helper.Models;

/// <summary>
/// Genel amaçlı servis wrapper sınıfı
/// Servis çağrılarını sarmalama ve yönetme işlevleri sağlar
/// </summary>
public class WrapperService
{
    /// <summary>
    /// Servis çağrılarını sarmalar ve yönetir
    /// </summary>
    public void WrapServiceCall()
    {
        // Implementation for wrapping service calls
    }

    /// <summary>
    /// Servisleri yönetir ve koordine eder
    /// </summary>
    public void ManageService()
    {
        // Implementation for managing services
    }
}

/// <summary>
/// Bankacılık işlemlerini yöneten ana servis sınıfı
/// Hesap yönetimi, para transferi ve döviz kuru işlemlerini içerir
/// In-memory veri saklama kullanır (production için veritabanı gerekli)
/// </summary>
public class BankingService
{
    #region Private Fields
    /// <summary>
    /// Sistemdeki tüm hesapları saklayan liste
    /// </summary>
    private readonly List<Account> _accounts;

    /// <summary>
    /// Gerçekleştirilen tüm işlemleri saklayan liste
    /// </summary>
    private readonly List<Transaction> _transactions;

    /// <summary>
    /// Güncel döviz kurlarını saklayan liste
    /// </summary>
    private readonly List<ExchangeRate> _exchangeRates;
    #endregion

    #region Constructor
    /// <summary>
    /// BankingService constructor
    /// Veri listelerini başlatır ve test verilerini yükler
    /// </summary>
    public BankingService()
    {
        // Veri listelerini başlat
        _accounts = new List<Account>();
        _transactions = new List<Transaction>();
        _exchangeRates = new List<ExchangeRate>();
        
        // Test verilerini yükle
        InitializeTestData();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Belirtilen kullanıcının aktif hesaplarını döndürür
    /// Hesap bakiyelerini para birimi sembolleri ile birlikte döner
    /// </summary>
    /// <param name="userId">Kullanıcı kimlik numarası</param>
    /// <returns>Kullanıcının hesap bakiyeleri listesi</returns>
    public List<AccountBalance> GetUserAccounts(int userId)
    {
        // Kullanıcının aktif hesaplarını filtrele
        var userAccounts = _accounts.Where(a => a.UserId == userId && a.IsActive).ToList();
        
        // AccountBalance formatına dönüştür
        return userAccounts.Select(account => new AccountBalance
        {
            AccountId = account.AccountId,
            AccountNumber = account.AccountNumber,
            Currency = account.Currency,
            Balance = account.Balance,
            CurrencySymbol = BankingUtility.GetCurrencySymbol(account.Currency)
        }).ToList();
    }

    /// <summary>
    /// Hesaplar arası para transferi işlemini gerçekleştirir
    /// Döviz çevirimi, validasyon ve işlem kaydı yapar
    /// </summary>
    /// <param name="request">Transfer isteği detayları</param>
    /// <returns>Transfer işlemi sonucu</returns>
    public TransactionResult TransferMoney(TransferRequest request)
    {
        try
        {
            // Kaynak ve hedef hesapları bul
            var fromAccount = _accounts.FirstOrDefault(a => a.AccountId == request.FromAccountId);
            var toAccount = _accounts.FirstOrDefault(a => a.AccountId == request.ToAccountId);

            // Hesap varlık kontrolü
            if (fromAccount == null || toAccount == null)
                return new TransactionResult { Success = false, Message = "Hesap bulunamadı" };

            // Transfer tutarı validasyonu
            if (!BankingUtility.ValidateTransferAmount(request.Amount))
                return new TransactionResult { Success = false, Message = "Geçersiz transfer tutarı" };

            // Bakiye yeterlilik kontrolü
            if (!BankingUtility.ValidateAccountBalance(fromAccount.Balance, request.Amount))
                return new TransactionResult { Success = false, Message = "Yetersiz bakiye" };

            // Döviz kuru hesaplama
            decimal exchangeRate = 1;
            if (fromAccount.Currency != toAccount.Currency)
            {
                exchangeRate = GetExchangeRate(fromAccount.Currency, toAccount.Currency);
                if (exchangeRate == 0)
                    return new TransactionResult { Success = false, Message = "Döviz kuru bulunamadı" };
            }

            // Para birimi çevirimi
            decimal convertedAmount = BankingUtility.ConvertCurrency(request.Amount, fromAccount.Currency, toAccount.Currency, exchangeRate);

            // Transfer işlemi - bakiyeleri güncelle
            fromAccount.Balance -= request.Amount;
            toAccount.Balance += convertedAmount;

            // İşlem kaydı oluştur
            var transaction = new Transaction
            {
                TransactionId = _transactions.Count + 1,
                FromAccountId = request.FromAccountId,
                ToAccountId = request.ToAccountId,
                Amount = request.Amount,
                Currency = fromAccount.Currency,
                ExchangeRate = exchangeRate,
                Type = TransactionType.Transfer,
                Status = TransactionStatus.Completed,
                TransactionDate = DateTime.Now,
                Description = request.Description ?? "Para transferi"
            };

            // İşlemi kaydet
            _transactions.Add(transaction);

            // Başarılı sonuç döndür
            return new TransactionResult 
            { 
                Success = true, 
                Message = "Transfer başarılı",
                TransactionId = transaction.TransactionId,
                ConvertedAmount = convertedAmount
            };
        }
        catch (Exception ex)
        {
            // Hata durumunda detaylı hata mesajı döndür
            return new TransactionResult { Success = false, Message = $"Transfer hatası: {ex.Message}" };
        }
    }

    /// <summary>
    /// Güncel döviz kurlarını döndürür
    /// </summary>
    /// <returns>Döviz kurları listesi</returns>
    public List<ExchangeRate> GetExchangeRates()
    {
        return _exchangeRates.ToList();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// İki para birimi arasındaki çevrim oranını bulur
    /// </summary>
    /// <param name="fromCurrency">Kaynak para birimi</param>
    /// <param name="toCurrency">Hedef para birimi</param>
    /// <returns>Çevrim oranı (bulunamazsa 0)</returns>
    private decimal GetExchangeRate(CurrencyType fromCurrency, CurrencyType toCurrency)
    {
        var rate = _exchangeRates.FirstOrDefault(r => 
            r.FromCurrency == fromCurrency && r.ToCurrency == toCurrency);
        
        return rate?.Rate ?? 0;
    }

    /// <summary>
    /// Test verilerini sisteme yükler
    /// Production ortamında bu veriler veritabanından gelecek
    /// </summary>
    private void InitializeTestData()
    {
        // Test hesapları - UserId: 1 için 3 farklı para birimi hesabı
        _accounts.AddRange(new[]
        {
            new Account { 
                AccountId = 1, 
                UserId = 1, 
                AccountNumber = "TR123456", 
                Currency = CurrencyType.TRY, 
                Balance = 50000, 
                CreatedDate = DateTime.Now.AddDays(-30), 
                IsActive = true 
            },
            new Account { 
                AccountId = 2, 
                UserId = 1, 
                AccountNumber = "US123456", 
                Currency = CurrencyType.USD, 
                Balance = 2500, 
                CreatedDate = DateTime.Now.AddDays(-30), 
                IsActive = true 
            },
            new Account { 
                AccountId = 3, 
                UserId = 1, 
                AccountNumber = "EU123456", 
                Currency = CurrencyType.EUR, 
                Balance = 2000, 
                CreatedDate = DateTime.Now.AddDays(-30), 
                IsActive = true 
            }
        });

        // Test döviz kurları - Gerçek zamanlı kurlar için external API gerekli
        _exchangeRates.AddRange(new[]
        {
            // TL'den diğer para birimlerine
            new ExchangeRate { FromCurrency = CurrencyType.TRY, ToCurrency = CurrencyType.USD, Rate = 0.034m, LastUpdated = DateTime.Now },
            new ExchangeRate { FromCurrency = CurrencyType.TRY, ToCurrency = CurrencyType.EUR, Rate = 0.031m, LastUpdated = DateTime.Now },
            
            // USD'den diğer para birimlerine
            new ExchangeRate { FromCurrency = CurrencyType.USD, ToCurrency = CurrencyType.TRY, Rate = 29.5m, LastUpdated = DateTime.Now },
            new ExchangeRate { FromCurrency = CurrencyType.USD, ToCurrency = CurrencyType.EUR, Rate = 0.92m, LastUpdated = DateTime.Now },
            
            // EUR'dan diğer para birimlerine
            new ExchangeRate { FromCurrency = CurrencyType.EUR, ToCurrency = CurrencyType.TRY, Rate = 32.2m, LastUpdated = DateTime.Now },
            new ExchangeRate { FromCurrency = CurrencyType.EUR, ToCurrency = CurrencyType.USD, Rate = 1.09m, LastUpdated = DateTime.Now }
        });
    }
    #endregion
}

/// <summary>
/// Transfer işlemi sonucunu temsil eden model sınıfı
/// İşlem başarı durumu, mesaj ve detayları içerir
/// </summary>
public class TransactionResult
{
    /// <summary>
    /// İşlemin başarılı olup olmadığını belirten flag
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// İşlem sonucu mesajı (başarı/hata)
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Oluşturulan işlem kimlik numarası (başarılı işlemlerde)
    /// </summary>
    public int? TransactionId { get; set; }

    /// <summary>
    /// Çevrilmiş tutar (farklı para birimi transferlerinde)
    /// </summary>
    public decimal? ConvertedAmount { get; set; }
}