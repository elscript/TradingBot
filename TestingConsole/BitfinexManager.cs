using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Authentication;
using TradingBot.Core;

namespace TestingConsole
{
    public class BitfinexManager
    {
        private string _accessKey;
        private string _accessSecret;
        private BitfinexClient _client;

        public BitfinexManager(string accessKey, string accessSecret)
        {
            _accessKey = accessKey;
            _accessSecret = accessSecret;

            _client = new BitfinexClient(
                new BitfinexClientOptions()
                {
                    ApiCredentials = new ApiCredentials(
                        key: _accessKey,
                        secret: _accessSecret)
                });
        }

        public IList<BitfinexCandle> GetData(string ticker, TimeFrame timeFrame, int amount)
        {
            var candles = _client.GetCandles(timeFrame, ticker, amount, null, DateTime.Now.ToUniversalTime());
            IList<BitfinexCandle> candlesData = candles.Data.ToList();
            return candlesData;
        }

        public BitfinexPosition GetActivePosition()
        {
            return _client.GetActivePositions().Data.First();
        }

        public decimal GetCurrentBalance(string currency)
        {
            return _client.GetWallets().Data.Single(d => d.Currency == currency && d.Type == WalletType.Margin).Balance;
        }

        public void OpenPosition()
        {

        }

        public void ClosePosition()
        {

        }
    }
}
