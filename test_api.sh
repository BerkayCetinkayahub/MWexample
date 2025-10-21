#!/bin/bash

# Banking API Test Script
# Bu script backend API endpoint'lerini test eder

BASE_URL="http://localhost:5000"
USER_ID=1

echo "=== Banking API Test Script ==="
echo "Base URL: $BASE_URL"
echo ""

# Test 1: Hesap Listesi
echo "1. Hesap Listesi Testi..."
echo "GET $BASE_URL/Handlers/AccountHandler.ashx?userId=$USER_ID"
echo ""

response=$(curl -s -X GET "$BASE_URL/Handlers/AccountHandler.ashx?userId=$USER_ID" \
  -H "Content-Type: application/json")

if [ $? -eq 0 ]; then
    echo "✅ Başarılı!"
    echo "Yanıt: $response"
else
    echo "❌ Hata!"
fi

echo ""
echo "----------------------------------------"
echo ""

# Test 2: Döviz Kurları
echo "2. Döviz Kurları Testi..."
echo "GET $BASE_URL/Handlers/ExchangeRateHandler.ashx"
echo ""

response=$(curl -s -X GET "$BASE_URL/Handlers/ExchangeRateHandler.ashx" \
  -H "Content-Type: application/json")

if [ $? -eq 0 ]; then
    echo "✅ Başarılı!"
    echo "Yanıt: $response"
else
    echo "❌ Hata!"
fi

echo ""
echo "----------------------------------------"
echo ""

# Test 3: Para Transferi
echo "3. Para Transferi Testi..."
echo "POST $BASE_URL/Handlers/TransferHandler.ashx"
echo ""

transfer_data='{
  "FromAccountId": 1,
  "ToAccountId": 2,
  "Amount": 100.00,
  "Description": "Test transferi - Script"
}'

echo "Transfer Verisi: $transfer_data"
echo ""

response=$(curl -s -X POST "$BASE_URL/Handlers/TransferHandler.ashx" \
  -H "Content-Type: application/json" \
  -d "$transfer_data")

if [ $? -eq 0 ]; then
    echo "✅ Başarılı!"
    echo "Yanıt: $response"
else
    echo "❌ Hata!"
fi

echo ""
echo "----------------------------------------"
echo ""

# Test 4: Hesap Listesi (Transfer sonrası)
echo "4. Transfer Sonrası Hesap Listesi..."
echo "GET $BASE_URL/Handlers/AccountHandler.ashx?userId=$USER_ID"
echo ""

response=$(curl -s -X GET "$BASE_URL/Handlers/AccountHandler.ashx?userId=$USER_ID" \
  -H "Content-Type: application/json")

if [ $? -eq 0 ]; then
    echo "✅ Başarılı!"
    echo "Yanıt: $response"
else
    echo "❌ Hata!"
fi

echo ""
echo "=== Test Tamamlandı ==="
