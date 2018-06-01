using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Authentication;
using TradingBot.Core;
using System.Threading;

namespace TradingBot.Core
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


        /// <summary>
        /// Получение данных
        /// </summary>
        /// <param name="ticker">Валютная пара</param>
        /// <param name="timeFrame">Таймфрейм</param>
        /// <param name="amount">Кол-во свечей</param>
        /// <returns>Список свечей</returns>
        public IList<BitfinexCandle> GetData(string ticker, TimeFrame timeFrame, int amount)
        {
            var candles = _client.GetCandles(timeFrame, ticker, amount, null, DateTime.Now.ToUniversalTime());
            IList<BitfinexCandle> candlesData = candles.Data.ToList();

            for (int i = 1000; i < amount; i+=1000)
            {
                var data = candlesData.ToList();
                if (data.Count() != 0)
                {
                    var morecandles = _client.GetCandles(timeFrame, ticker, amount, null, data.Last().Timestamp.AddMinutes(-5));
                    candlesData = data.Concat(morecandles.Data).ToList();
                }
            }

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

        /// <summary>
        /// Выполнение сделки по рыночной цене
        /// </summary>
        /// <param name="ticker">Пара</param>
        /// <param name="side">Направление сделки</param>
        /// <param name="amount">Количество</param>
        /// <returns>Цена, по которой отработал ордер</returns>
        public decimal ExecuteDealByMarket(string ticker, OrderSide side, decimal amount)
        {
            var order = _client.PlaceOrder(ticker, side, OrderTypeV1.Market, amount, price:0, hidden: true, useAllAvailable: true);

            decimal execPrice = 0;
            while (execPrice == 0)
            {
                var trade = _client.GetTradeHistory(ticker, DateTime.Now.AddSeconds(-20).ToUniversalTime(), DateTime.Now.ToUniversalTime(), 3);
                execPrice = trade.Data.SingleOrDefault(t => t.OrderId == order.Data.ClientOrderId).ExecutedPrice;
                Thread.Sleep(5000);
            }
            return execPrice;
        }
    }
}
