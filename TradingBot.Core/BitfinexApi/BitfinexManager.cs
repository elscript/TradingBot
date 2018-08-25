﻿using System;
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
            var portionCount = amount > 1000 ? 1000 : amount;

            var candles = _client.GetCandles(timeFrame, ticker, portionCount, null, DateTime.Now.ToUniversalTime());
            IList<BitfinexCandle> candlesData = candles.Data.ToList();

            if (portionCount == 1000)
            {
                for (int i = 1000; i < amount; i += 1000)
                {
                    var data = candlesData.ToList();
                    if (data.Count() != 0)
                    {
                        var morecandles = _client.GetCandles(timeFrame, ticker, 1000, null,
                            data.Last().Timestamp.AddMinutes(-5));
                        candlesData = data.Concat(morecandles.Data).ToList();
                    }
                }
            }

            return candlesData.OrderBy(d => d.Timestamp).ToList();
        }

        /// <summary>
        /// Получение данных
        /// </summary>
        /// <param name="ticker">Валютная пара</param>
        /// <param name="timeFrame">Таймфрейм</param>
        /// <param name="amount">Кол-во свечей</param>
        /// <param name="dateFrom">Начало диапазона</param>
        /// <param name="dateTo">Конец диапазона</param>
        /// <returns>Список свечей</returns>
        public IList<BitfinexCandle> GetData(string ticker, TimeFrame timeFrame, int amount, DateTime dateFrom, DateTime dateTo)
        {
            var portionCount = amount > 1000 ? 1000 : amount;

            var candles = _client.GetCandles(timeFrame, ticker, portionCount, dateFrom, dateTo);
            IList<BitfinexCandle> candlesData = candles.Data.ToList();

            if (portionCount == 1000)
            {
                for (int i = 1000; i < amount; i += 1000)
                {
                    var data = candlesData.ToList();
                    if (data.Count() != 0)
                    {
                        var morecandles = _client.GetCandles(timeFrame, ticker, 1000, null,
                            data.Last().Timestamp.AddMinutes(-5));
                        candlesData = data.Concat(morecandles.Data).ToList();
                    }
                }
            }

            return candlesData.OrderBy(d => d.Timestamp).ToList();
        }

        public BitfinexPosition GetActivePosition(string symbol)
        {
            return _client.GetActivePositions().Data.First(p => p.Symbol == symbol);
        }

        public decimal GetCurrentBalance(string currency)
        {
            return Math.Round(_client.GetWallets().Data.Single(d => d.Currency == currency && d.Type == WalletType.Margin).Balance, 2);
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
            var order = _client.PlaceOrder(ticker.TrimStart('t'), side, OrderTypeV1.Market, amount, price: 1, hidden: true, useAllAvailable: true);

            decimal execPrice = 0;
            while (execPrice == 0)
            {
                Thread.Sleep(5000);
                var trades = _client.GetTradesForOrder(ticker, order.Data.Id);
                if (trades.Data != null && trades.Data.Length > 0)
                    execPrice = trades.Data.Average(t => t.ExecutedPrice);
                Thread.Sleep(3000);
            }
            return execPrice;
        }
    }
}