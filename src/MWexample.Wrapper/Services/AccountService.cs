//
//  AccountService.cs
//  MWexample.Wrapper
//
//  Account operations with common mistakes
//

using System;
using System.Collections.Generic;
using System.Linq;
using MWexample.Helper.Models;

namespace MWexample.Wrapper.Services
{
    public class AccountService
    {
        private List<Account> _accounts;

        public AccountService()
        {
            // ❌ Nullable olmayan field ama null bırakılabilir
            _accounts = null;
        }

        // ❌ Null check eksik
        public Account GetAccountById(int accountId)
        {
            // _accounts null olabilir!
            var account = _accounts.FirstOrDefault(a => a.AccountId == accountId);
            
            // ❌ Null dönüş - caller'da NullReferenceException riski
            return account;
        }

        // ❌ Unsafe array access
        public Account GetAccountByIndex(int index)
        {
            // Index out of bounds check yok!
            return _accounts[index];
        }

        // ❌ Mutable collection döndürülüyor
        public List<Account> GetAllAccounts()
        {
            // ❌ Direct reference döndürülüyor - encapsulation bozuldu
            return _accounts;
        }

        // ❌ SQL Injection riski (simulated)
        public List<Account> SearchAccounts(string accountNumber)
        {
            // ❌ String concatenation ile query - SQL injection riski
            string query = "SELECT * FROM Accounts WHERE AccountNumber = '" + accountNumber + "'";
            
            // Simulated query execution
            return _accounts.Where(a => a.AccountNumber.Contains(accountNumber)).ToList();
        }

        // ❌ Race condition riski
        public void UpdateBalance(int accountId, decimal newBalance)
        {
            // ❌ Thread-safe değil!
            var account = _accounts.FirstOrDefault(a => a.AccountId == accountId);
            if (account != null)
            {
                // Başka bir thread aynı anda değiştirirse?
                account.Balance = newBalance;
            }
        }

        // ❌ Infinite loop riski
        public Account FindActiveAccount(int userId)
        {
            Account account = null;
            int index = 0;
            
            // ❌ index kontrolü yok - infinite loop olabilir
            while (account == null)
            {
                if (_accounts[index].UserId == userId && _accounts[index].IsActive)
                {
                    account = _accounts[index];
                }
                index++;
            }
            
            return account;
        }

        // ❌ Memory leak - event unsubscribe yok
        public event EventHandler<Account> AccountUpdated;
        
        public void SubscribeToUpdates()
        {
            // Event subscribe ediliyor ama unsubscribe yok
            AccountUpdated += OnAccountUpdated;
        }

        private void OnAccountUpdated(object sender, Account account)
        {
            // Handler logic
        }

        // ❌ Hardcoded path - configuration olmalı
        public void SaveAccountsToFile()
        {
            // ❌ Hardcoded path
            string filePath = "C:\\Users\\Admin\\accounts.txt";
            
            // ❌ File operations without proper error handling
            System.IO.File.WriteAllLines(filePath, 
                _accounts.Select(a => $"{a.AccountId},{a.Balance}"));
        }

        // ❌ Sensitive data logging
        public void LogAccountDetails(Account account)
        {
            // ❌ Sensitive data (bakiye, hesap no) log'a yazılıyor
            Console.WriteLine($"Account: {account.AccountNumber}, Balance: {account.Balance}");
        }

        // ❌ Password/sensitive data in plain text
        private const string ApiKey = "sk_live_123456789abcdef"; // ❌ Hardcoded API key!

        public bool AuthenticateAccount(string accountNumber, string password)
        {
            // ❌ Plain text password comparison
            var account = _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            
            // ❌ Timing attack vulnerability
            return account != null && password == "admin123"; // ❌ Hardcoded password!
        }

        // ❌ Missing disposal
        public void ProcessLargeDataSet()
        {
            // ❌ IDisposable ama using yok
            var stream = new System.IO.MemoryStream();
            var writer = new System.IO.StreamWriter(stream);
            
            foreach (var account in _accounts)
            {
                writer.WriteLine(account.AccountNumber);
            }
            
            // ❌ Dispose edilmedi - memory leak
        }

        // ❌ Recursive call without termination
        public decimal CalculateTotalBalance(int depth)
        {
            // ❌ Base case eksik - stack overflow riski
            if (_accounts.Count > 0)
            {
                return _accounts.Sum(a => a.Balance) + CalculateTotalBalance(depth + 1);
            }
            return 0;
        }

        // ❌ Boolean trap
        public void UpdateAccount(int accountId, decimal amount, bool flag)
        {
            // ❌ "flag" ne anlama geliyor? Anlaşılmaz boolean parameter
            var account = GetAccountById(accountId);
            
            if (flag)
            {
                account.Balance += amount;
            }
            else
            {
                account.Balance -= amount;
            }
        }

        // ❌ God method - too many responsibilities
        public bool ProcessAccountTransaction(int fromId, int toId, decimal amount, 
            bool checkBalance, bool checkLimit, bool sendEmail, bool logTransaction,
            string description, DateTime date, int userId, bool validateUser)
        {
            // ❌ Çok fazla parametre ve sorumluluk
            // ❌ Boolean parameters everywhere
            
            var fromAccount = GetAccountById(fromId);
            var toAccount = GetAccountById(toId);
            
            if (checkBalance && fromAccount.Balance < amount)
                return false;
                
            if (checkLimit && amount > 10000)
                return false;
                
            if (validateUser && fromAccount.UserId != userId)
                return false;
                
            fromAccount.Balance -= amount;
            toAccount.Balance += amount;
            
            if (sendEmail)
            {
                // Send email
            }
            
            if (logTransaction)
            {
                // Log
            }
            
            return true;
        }
    }
}
