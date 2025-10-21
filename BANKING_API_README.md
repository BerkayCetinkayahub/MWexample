# Bankacılık Uygulaması Backend API

Bu proje, iOS bankacılık uygulaması için backend servislerini içerir. Kullanıcıların TL, USD ve EUR hesapları arasında para transferi yapabilmesini sağlar.

## Proje Yapısı

### Katmanlar
- **Helper**: Modeller ve utility sınıfları
- **Wrapper**: İş mantığı servisleri
- **ASHX**: HTTP handler'ları (API endpoints)

## API Endpoints

### 1. Hesap Listesi
**URL:** `/Handlers/AccountHandler.ashx`  
**Method:** GET  
**Parametreler:** `userId` (query string)

**Örnek İstek:**
```
GET /Handlers/AccountHandler.ashx?userId=1
```

**Örnek Yanıt:**
```json
{
  "success": true,
  "data": [
    {
      "AccountId": 1,
      "AccountNumber": "TR123456",
      "Currency": 1,
      "Balance": 50000.00,
      "CurrencySymbol": "₺"
    },
    {
      "AccountId": 2,
      "AccountNumber": "US123456",
      "Currency": 2,
      "Balance": 2500.00,
      "CurrencySymbol": "$"
    },
    {
      "AccountId": 3,
      "AccountNumber": "EU123456",
      "Currency": 3,
      "Balance": 2000.00,
      "CurrencySymbol": "€"
    }
  ]
}
```

### 2. Para Transferi
**URL:** `/Handlers/TransferHandler.ashx`  
**Method:** POST  
**Content-Type:** application/json

**Örnek İstek:**
```json
{
  "FromAccountId": 1,
  "ToAccountId": 2,
  "Amount": 1000.00,
  "Description": "TL'den USD'ye transfer"
}
```

**Örnek Yanıt:**
```json
{
  "success": true,
  "data": {
    "Success": true,
    "Message": "Transfer başarılı",
    "TransactionId": 1,
    "ConvertedAmount": 34.00
  }
}
```

### 3. Döviz Kurları
**URL:** `/Handlers/ExchangeRateHandler.ashx`  
**Method:** GET

**Örnek Yanıt:**
```json
{
  "success": true,
  "data": [
    {
      "FromCurrency": 1,
      "ToCurrency": 2,
      "Rate": 0.034,
      "LastUpdated": "2024-01-15T10:30:00"
    },
    {
      "FromCurrency": 1,
      "ToCurrency": 3,
      "Rate": 0.031,
      "LastUpdated": "2024-01-15T10:30:00"
    }
  ]
}
```

## Para Birimleri

- **TRY (1)**: Türk Lirası (₺)
- **USD (2)**: Amerikan Doları ($)
- **EUR (3)**: Euro (€)

## Test Verileri

Sistem başlatıldığında otomatik olarak aşağıdaki test verileri yüklenir:

### Kullanıcı Hesapları (UserId: 1)
- **TL Hesabı**: 50,000.00 ₺
- **USD Hesabı**: 2,500.00 $
- **EUR Hesabı**: 2,000.00 €

### Döviz Kurları
- 1 TL = 0.034 USD
- 1 TL = 0.031 EUR
- 1 USD = 29.5 TL
- 1 USD = 0.92 EUR
- 1 EUR = 32.2 TL
- 1 EUR = 1.09 USD

## iOS Client Kullanım Örnekleri

### Swift ile Hesap Listesi Çekme
```swift
func fetchAccounts(userId: Int) {
    let url = URL(string: "https://yourdomain.com/Handlers/AccountHandler.ashx?userId=\(userId)")!
    
    URLSession.shared.dataTask(with: url) { data, response, error in
        if let data = data {
            let accounts = try? JSONDecoder().decode(AccountResponse.self, from: data)
            DispatchQueue.main.async {
                // UI güncelleme
            }
        }
    }.resume()
}
```

### Swift ile Para Transferi
```swift
func transferMoney(fromAccountId: Int, toAccountId: Int, amount: Decimal, description: String) {
    let url = URL(string: "https://yourdomain.com/Handlers/TransferHandler.ashx")!
    var request = URLRequest(url: url)
    request.httpMethod = "POST"
    request.setValue("application/json", forHTTPHeaderField: "Content-Type")
    
    let transferRequest = TransferRequest(
        fromAccountId: fromAccountId,
        toAccountId: toAccountId,
        amount: amount,
        description: description
    )
    
    request.httpBody = try? JSONEncoder().encode(transferRequest)
    
    URLSession.shared.dataTask(with: request) { data, response, error in
        if let data = data {
            let result = try? JSONDecoder().decode(TransferResponse.self, from: data)
            DispatchQueue.main.async {
                // Sonuç işleme
            }
        }
    }.resume()
}
```

## Hata Durumları

### Yaygın Hata Mesajları
- "Kullanıcı ID gerekli"
- "Geçersiz kullanıcı ID"
- "Hesap bulunamadı"
- "Geçersiz transfer tutarı"
- "Yetersiz bakiye"
- "Aynı hesaba transfer yapılamaz"
- "Döviz kuru bulunamadı"

## Güvenlik Notları

- CORS headers tüm endpoint'lerde aktif
- Transfer limiti: Maksimum 1,000,000 birim
- Minimum transfer tutarı: 0.01 birim
- Tüm işlemler loglanır ve transaction ID ile takip edilir

## Geliştirme Notları

- Proje .NET 6.0 ile geliştirilmiştir
- In-memory veri saklama kullanılmaktadır (production için veritabanı gerekli)
- Döviz kurları statik olarak tanımlanmıştır (production için gerçek zamanlı API gerekli)
- Tüm endpoint'ler JSON formatında yanıt döner
