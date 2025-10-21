# iOS Client - Banking API Entegrasyon Rehberi

## 📱 iOS Uygulaması için API Bağlantısı

### 1. Backend Server Başlatma

```bash
# Terminal'de proje dizininde:
cd /Users/berkaycetinkaya/Desktop/MWexample
dotnet run --project src/MWexample.Ashx
```

Server başlatıldıktan sonra şu URL'ler kullanılabilir:
- **Base URL**: `http://localhost:5000` (development)
- **Production URL**: `https://yourdomain.com` (production)

### 2. iOS Swift Kod Örnekleri

#### A. Hesap Listesi Çekme

```swift
import Foundation

// MARK: - Model Classes
struct AccountBalance: Codable {
    let accountId: Int
    let accountNumber: String
    let currency: Int
    let balance: Double
    let currencySymbol: String
    
    enum CodingKeys: String, CodingKey {
        case accountId = "AccountId"
        case accountNumber = "AccountNumber"
        case currency = "Currency"
        case balance = "Balance"
        case currencySymbol = "CurrencySymbol"
    }
}

struct AccountResponse: Codable {
    let success: Bool
    let data: [AccountBalance]?
    let message: String?
}

// MARK: - API Service
class BankingAPIService {
    static let shared = BankingAPIService()
    private let baseURL = "http://localhost:5000" // Development URL
    
    private init() {}
    
    // MARK: - Hesap Listesi
    func fetchAccounts(userId: Int, completion: @escaping (Result<[AccountBalance], Error>) -> Void) {
        let urlString = "\(baseURL)/Handlers/AccountHandler.ashx?userId=\(userId)"
        
        guard let url = URL(string: urlString) else {
            completion(.failure(APIError.invalidURL))
            return
        }
        
        var request = URLRequest(url: url)
        request.httpMethod = "GET"
        request.setValue("application/json", forHTTPHeaderField: "Content-Type")
        
        URLSession.shared.dataTask(with: request) { data, response, error in
            if let error = error {
                completion(.failure(error))
                return
            }
            
            guard let data = data else {
                completion(.failure(APIError.noData))
                return
            }
            
            do {
                let response = try JSONDecoder().decode(AccountResponse.self, from: data)
                if response.success, let accounts = response.data {
                    completion(.success(accounts))
                } else {
                    completion(.failure(APIError.serverError(response.message ?? "Bilinmeyen hata")))
                }
            } catch {
                completion(.failure(error))
            }
        }.resume()
    }
}

// MARK: - Error Handling
enum APIError: Error {
    case invalidURL
    case noData
    case serverError(String)
    
    var localizedDescription: String {
        switch self {
        case .invalidURL:
            return "Geçersiz URL"
        case .noData:
            return "Veri bulunamadı"
        case .serverError(let message):
            return message
        }
    }
}
```

#### B. Para Transferi

```swift
// MARK: - Transfer Models
struct TransferRequest: Codable {
    let fromAccountId: Int
    let toAccountId: Int
    let amount: Double
    let description: String?
    
    enum CodingKeys: String, CodingKey {
        case fromAccountId = "FromAccountId"
        case toAccountId = "ToAccountId"
        case amount = "Amount"
        case description = "Description"
    }
}

struct TransferResult: Codable {
    let success: Bool
    let message: String
    let transactionId: Int?
    let convertedAmount: Double?
    
    enum CodingKeys: String, CodingKey {
        case success = "Success"
        case message = "Message"
        case transactionId = "TransactionId"
        case convertedAmount = "ConvertedAmount"
    }
}

struct TransferResponse: Codable {
    let success: Bool
    let data: TransferResult?
    let message: String?
}

// MARK: - Transfer API Extension
extension BankingAPIService {
    func transferMoney(request: TransferRequest, completion: @escaping (Result<TransferResult, Error>) -> Void) {
        let urlString = "\(baseURL)/Handlers/TransferHandler.ashx"
        
        guard let url = URL(string: urlString) else {
            completion(.failure(APIError.invalidURL))
            return
        }
        
        var urlRequest = URLRequest(url: url)
        urlRequest.httpMethod = "POST"
        urlRequest.setValue("application/json", forHTTPHeaderField: "Content-Type")
        
        do {
            let jsonData = try JSONEncoder().encode(request)
            urlRequest.httpBody = jsonData
        } catch {
            completion(.failure(error))
            return
        }
        
        URLSession.shared.dataTask(with: urlRequest) { data, response, error in
            if let error = error {
                completion(.failure(error))
                return
            }
            
            guard let data = data else {
                completion(.failure(APIError.noData))
                return
            }
            
            do {
                let response = try JSONDecoder().decode(TransferResponse.self, from: data)
                if response.success, let result = response.data {
                    completion(.success(result))
                } else {
                    completion(.failure(APIError.serverError(response.message ?? "Transfer başarısız")))
                }
            } catch {
                completion(.failure(error))
            }
        }.resume()
    }
}
```

#### C. Döviz Kurları

```swift
// MARK: - Exchange Rate Models
struct ExchangeRate: Codable {
    let fromCurrency: Int
    let toCurrency: Int
    let rate: Double
    let lastUpdated: String
    
    enum CodingKeys: String, CodingKey {
        case fromCurrency = "FromCurrency"
        case toCurrency = "ToCurrency"
        case rate = "Rate"
        case lastUpdated = "LastUpdated"
    }
}

struct ExchangeRateResponse: Codable {
    let success: Bool
    let data: [ExchangeRate]?
    let message: String?
}

// MARK: - Exchange Rate API Extension
extension BankingAPIService {
    func fetchExchangeRates(completion: @escaping (Result<[ExchangeRate], Error>) -> Void) {
        let urlString = "\(baseURL)/Handlers/ExchangeRateHandler.ashx"
        
        guard let url = URL(string: urlString) else {
            completion(.failure(APIError.invalidURL))
            return
        }
        
        var request = URLRequest(url: url)
        request.httpMethod = "GET"
        request.setValue("application/json", forHTTPHeaderField: "Content-Type")
        
        URLSession.shared.dataTask(with: request) { data, response, error in
            if let error = error {
                completion(.failure(error))
                return
            }
            
            guard let data = data else {
                completion(.failure(APIError.noData))
                return
            }
            
            do {
                let response = try JSONDecoder().decode(ExchangeRateResponse.self, from: data)
                if response.success, let rates = response.data {
                    completion(.success(rates))
                } else {
                    completion(.failure(APIError.serverError(response.message ?? "Döviz kurları alınamadı")))
                }
            } catch {
                completion(.failure(error))
            }
        }.resume()
    }
}
```

### 3. ViewController'da Kullanım Örneği

```swift
import UIKit

class BankingViewController: UIViewController {
    @IBOutlet weak var accountsTableView: UITableView!
    @IBOutlet weak var transferButton: UIButton!
    
    private var accounts: [AccountBalance] = []
    private let apiService = BankingAPIService.shared
    
    override func viewDidLoad() {
        super.viewDidLoad()
        setupUI()
        loadAccounts()
    }
    
    private func setupUI() {
        accountsTableView.delegate = self
        accountsTableView.dataSource = self
    }
    
    private func loadAccounts() {
        // Test kullanıcısı için hesap listesini yükle
        apiService.fetchAccounts(userId: 1) { [weak self] result in
            DispatchQueue.main.async {
                switch result {
                case .success(let accounts):
                    self?.accounts = accounts
                    self?.accountsTableView.reloadData()
                case .failure(let error):
                    self?.showAlert(title: "Hata", message: error.localizedDescription)
                }
            }
        }
    }
    
    @IBAction func transferButtonTapped(_ sender: UIButton) {
        // Transfer işlemi için örnek
        let transferRequest = TransferRequest(
            fromAccountId: 1,
            toAccountId: 2,
            amount: 1000.0,
            description: "Test transferi"
        )
        
        apiService.transferMoney(request: transferRequest) { [weak self] result in
            DispatchQueue.main.async {
                switch result {
                case .success(let result):
                    self?.showAlert(title: "Başarılı", message: result.message)
                    self?.loadAccounts() // Hesapları yenile
                case .failure(let error):
                    self?.showAlert(title: "Hata", message: error.localizedDescription)
                }
            }
        }
    }
    
    private func showAlert(title: String, message: String) {
        let alert = UIAlertController(title: title, message: message, preferredStyle: .alert)
        alert.addAction(UIAlertAction(title: "Tamam", style: .default))
        present(alert, animated: true)
    }
}

// MARK: - TableView DataSource
extension BankingViewController: UITableViewDataSource, UITableViewDelegate {
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return accounts.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "AccountCell", for: indexPath)
        let account = accounts[indexPath.row]
        
        cell.textLabel?.text = account.accountNumber
        cell.detailTextLabel?.text = "\(account.balance) \(account.currencySymbol)"
        
        return cell
    }
}
```

### 4. Test Endpoint'leri

#### A. Hesap Listesi Testi
```bash
curl -X GET "http://localhost:5000/Handlers/AccountHandler.ashx?userId=1" \
  -H "Content-Type: application/json"
```

#### B. Para Transferi Testi
```bash
curl -X POST "http://localhost:5000/Handlers/TransferHandler.ashx" \
  -H "Content-Type: application/json" \
  -d '{
    "FromAccountId": 1,
    "ToAccountId": 2,
    "Amount": 1000.00,
    "Description": "Test transferi"
  }'
```

#### C. Döviz Kurları Testi
```bash
curl -X GET "http://localhost:5000/Handlers/ExchangeRateHandler.ashx" \
  -H "Content-Type: application/json"
```

### 5. Önemli Notlar

#### 🔒 Güvenlik
- Production'da HTTPS kullanın
- API key authentication ekleyin
- Rate limiting uygulayın

#### 🌐 CORS Ayarları
- CORS headers zaten eklenmiş durumda
- iOS simulator için localhost çalışır
- Gerçek cihaz için IP adresi kullanın

#### 📱 iOS Özel Ayarlar
- `Info.plist`'e network security ayarları ekleyin:
```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <true/>
</dict>
```

#### 🔧 Debug İpuçları
- Xcode'da Network tab'ından istekleri izleyin
- Console'da API yanıtlarını kontrol edin
- Postman ile endpoint'leri test edin

### 6. Production Hazırlığı

1. **Domain ve SSL**: Gerçek domain ve SSL sertifikası
2. **Veritabanı**: SQL Server veya PostgreSQL entegrasyonu
3. **Authentication**: JWT token sistemi
4. **Logging**: Structured logging ekleyin
5. **Monitoring**: Health check endpoint'leri

Bu rehber ile iOS uygulamanızı backend API'nizle başarıyla entegre edebilirsiniz! 🚀
