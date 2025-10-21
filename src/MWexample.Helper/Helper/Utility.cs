using System;
using System.Collections.Generic;
using MWexample.Helper.Models;

/// <summary>
/// Genel amaçlı utility fonksiyonları içeren statik sınıf
/// String işlemleri ve temel hesaplamalar için kullanılır
/// </summary>
public static class Utility
{
    /// <summary>
    /// String değeri temizler ve büyük harfe çevirir
    /// </summary>
    /// <param name="input">İşlenecek string değer</param>
    /// <returns>Temizlenmiş ve büyük harfe çevrilmiş string</returns>
    public static string FormatString(string input)
    {
        return input.Trim().ToUpper();
    }

    /// <summary>
    /// İki integer değerin toplamını hesaplar
    /// </summary>
    /// <param name="a">İlk sayı</param>
    /// <param name="b">İkinci sayı</param>
    /// <returns>Toplam değer</returns>
    public static int CalculateSum(int a, int b)
    {
        return a + b;
    }

    /// <summary>
    /// String değerin null veya boş olup olmadığını kontrol eder
    /// </summary>
    /// <param name="input">Kontrol edilecek string</param>
    /// <returns>Null veya boş ise true, değilse false</returns>
    public static bool IsNullOrEmpty(string input)
    {
        return string.IsNullOrEmpty(input);
    }
}

/// <summary>
/// Bankacılık işlemleri için özel utility fonksiyonları
/// Para birimi çevirimleri, validasyonlar ve hesap işlemleri içerir
/// </summary>
public static class BankingUtility
{
    /// <summary>
    /// Para birimi enum'ına göre sembol döndürür
    /// UI'da para birimi gösterimi için kullanılır
    /// </summary>
    /// <param name="currency">Para birimi türü</param>
    /// <returns>Para birimi sembolü (₺, $, €)</returns>
    public static string GetCurrencySymbol(CurrencyType currency)
    {
        return currency switch
        {
            CurrencyType.TRY => "₺",  // Türk Lirası sembolü
            CurrencyType.USD => "$",  // Amerikan Doları sembolü
            CurrencyType.EUR => "€",  // Euro sembolü
            _ => ""                   // Tanımsız para birimi için boş string
        };
    }

    /// <summary>
    /// Para birimi çevirimi yapar
    /// Kaynak para biriminden hedef para birimine çevrim oranı ile çarpar
    /// </summary>
    /// <param name="amount">Çevrilecek tutar</param>
    /// <param name="fromCurrency">Kaynak para birimi</param>
    /// <param name="toCurrency">Hedef para birimi</param>
    /// <param name="exchangeRate">Çevrim oranı</param>
    /// <returns>Çevrilmiş tutar</returns>
    public static decimal ConvertCurrency(decimal amount, CurrencyType fromCurrency, CurrencyType toCurrency, decimal exchangeRate)
    {
        // Aynı para birimi ise çevrim yapmaya gerek yok
        if (fromCurrency == toCurrency)
            return amount;

        // Çevrim oranı ile çarp
        return amount * exchangeRate;
    }

    /// <summary>
    /// Transfer tutarının geçerli olup olmadığını kontrol eder
    /// Pozitif değer ve maksimum limit kontrolü yapar
    /// </summary>
    /// <param name="amount">Kontrol edilecek tutar</param>
    /// <returns>Geçerli ise true, değilse false</returns>
    public static bool ValidateTransferAmount(decimal amount)
    {
        // Tutar pozitif olmalı ve maksimum 1 milyon limitini aşmamalı
        return amount > 0 && amount <= 1000000;
    }

    /// <summary>
    /// Hesap bakiyesinin transfer tutarı için yeterli olup olmadığını kontrol eder
    /// </summary>
    /// <param name="currentBalance">Mevcut hesap bakiyesi</param>
    /// <param name="transferAmount">Transfer edilecek tutar</param>
    /// <returns>Yeterli bakiye varsa true, yoksa false</returns>
    public static bool ValidateAccountBalance(decimal currentBalance, decimal transferAmount)
    {
        return currentBalance >= transferAmount;
    }

    /// <summary>
    /// Para birimine göre hesap numarası oluşturur
    /// Para birimi prefix'i + 6 haneli rastgele sayı formatında
    /// </summary>
    /// <param name="currency">Para birimi türü</param>
    /// <returns>Oluşturulan hesap numarası (örn: TR123456)</returns>
    public static string GenerateAccountNumber(CurrencyType currency)
    {
        // Para birimine göre prefix belirle
        var prefix = currency switch
        {
            CurrencyType.TRY => "TR",  // Türk Lirası için TR prefix
            CurrencyType.USD => "US",   // Amerikan Doları için US prefix
            CurrencyType.EUR => "EU",   // Euro için EU prefix
            _ => "XX"                   // Tanımsız para birimi için XX prefix
        };

        // 6 haneli rastgele sayı oluştur
        var random = new Random();
        var number = random.Next(100000, 999999);
        
        // Prefix + 6 haneli sayı formatında döndür
        return $"{prefix}{number:D6}";
    }
}