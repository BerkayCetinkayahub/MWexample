# Logic Improvements

Bu branch'te yapılan logic iyileştirmeleri:

## 1. Validation İyileştirmeleri
- Minimum transfer tutarı kontrolü eklendi (0.01)
- Magic number'lar const değişkenlerine çevrildi
- Daha açıklayıcı hata mesajları

## 2. Transfer Kontrolleri
- Kaynak ve hedef hesap kontrolleri ayrıştırıldı
- Aktif hesap kontrolü eklendi
- Detaylı bakiye bilgisi hata mesajlarında

## 3. Günlük Transfer Limiti
- Günlük 50,000 TL transfer limiti eklendi
- Tüm para birimleri TL'ye çevrilerek hesaplanıyor
- Bugün yapılan transferlerin toplamı kontrol ediliyor

## 4. Yardımcı Metotlar
- `GetTodayTransfersTotal()`: Bugünkü transferleri hesaplar
- `ConvertToTRY()`: Para birimini TL'ye çevirir

CodeRabbit bu değişiklikleri analiz edebilir.
