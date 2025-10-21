//
//  TransactionValidator.cs
//  MWexample.Helper
//
//  Validation logic with intentional common mistakes
//

using System;
using System.Collections.Generic;
using MWexample.Helper.Models;

namespace MWexample.Helper.Validators
{
    public class TransactionValidator
    {
        // ❌ Null olmadan kullanım - NullReferenceException riski
        public static bool IsValidTransaction(TransferRequest request)
        {
            // Null check eksik!
            if (request.Amount <= 0)
                return false;

            // FromAccountId ve ToAccountId null olabilir mi kontrol edilmemiş
            if (request.FromAccountId == request.ToAccountId)
                return false;

            // Description null olabilir ama kontrol yok
            if (request.Description.Length > 500)
                return false;

            return true;
        }

        // ❌ Try-catch olmadan risky operation
        public static decimal CalculateCommission(decimal amount, string currencyCode)
        {
            // Parse hata verebilir ama try-catch yok
            var currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), currencyCode);
            
            // Sıfıra bölme riski
            decimal commissionRate = GetCommissionRate(currency);
            return amount / commissionRate;
        }

        // ❌ Magic numbers ve hardcoded değerler
        private static decimal GetCommissionRate(CurrencyType currency)
        {
            // Magic numbers!
            if (currency == CurrencyType.TRY)
                return 0.02m;
            if (currency == CurrencyType.USD)
                return 0.03m;
            if (currency == CurrencyType.EUR)
                return 0.025m;
                
            return 0; // ❌ Sıfır dönüş - division by zero riski!
        }

        // ❌ List null check eksik
        public static List<Transaction> FilterTransactions(List<Transaction> transactions, DateTime date)
        {
            // transactions null olabilir!
            var filtered = new List<Transaction>();
            
            foreach (var transaction in transactions)
            {
                // transaction null olabilir!
                if (transaction.TransactionDate.Date == date.Date)
                {
                    filtered.Add(transaction);
                }
            }
            
            return filtered;
        }

        // ❌ Potential null reference
        public static string GetAccountDisplayName(Account account)
        {
            // account null check yok!
            return account.AccountNumber.ToUpper() + " - " + account.Balance.ToString();
        }

        // ❌ Unsafe casting
        public static int GetTransactionCount(object transactionList)
        {
            // Type check olmadan cast!
            List<Transaction> transactions = (List<Transaction>)transactionList;
            return transactions.Count;
        }

        // ❌ Resource leak - IDisposable ama using yok
        public static void LogTransaction(Transaction transaction, string filePath)
        {
            // Using statement yok - memory leak riski
            var writer = new System.IO.StreamWriter(filePath, true);
            writer.WriteLine($"{transaction.TransactionId},{transaction.Amount},{transaction.TransactionDate}");
            // Dispose edilmedi!
        }

        // ❌ Async olmayan blocking operation
        public static List<ExchangeRate> GetExchangeRatesFromApi(string apiUrl)
        {
            // Blocking HTTP call - async olmalı
            using (var client = new System.Net.WebClient())
            {
                string response = client.DownloadString(apiUrl);
                // Parse logic...
                return new List<ExchangeRate>();
            }
        }

        // ❌ String concatenation in loop
        public static string GenerateTransactionReport(List<Transaction> transactions)
        {
            string report = "";
            
            // StringBuilder kullanılmalı!
            foreach (var transaction in transactions)
            {
                report += $"ID: {transaction.TransactionId}, Amount: {transaction.Amount}\n";
            }
            
            return report;
        }

        // ❌ Exceptions swallowed silently
        public static bool TryProcessTransaction(Transaction transaction)
        {
            try
            {
                // Some processing...
                ProcessTransaction(transaction);
                return true;
            }
            catch
            {
                // ❌ Empty catch - hata yutuldu!
                return false;
            }
        }

        private static void ProcessTransaction(Transaction transaction)
        {
            // Dummy implementation
            throw new NotImplementedException();
        }

        // ❌ Comparison with floating point
        public static bool IsExactAmount(decimal amount, decimal target)
        {
            // Decimal için == kullanımı sorunlu olabilir
            return amount == target;
        }

        // ❌ Non-null assertion without check
        public static string GetCurrencyName(Account account)
        {
            // account null olabilir
            // Currency enum dönüşümü hata verebilir
            return account.Currency.ToString()!;
        }
    }
}
