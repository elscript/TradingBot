using System;
using System.Collections.Generic;
using System.Text;
using Binance.Net;
using Bitfinex.Net.Objects;
using TradingBot.Core.Common;
using Binance.Net.Objects.Spot;
using CryptoExchange.Net.Authentication;
using System.IO;
using CryptoExchange.Net.Logging;
using Binance.Net.Enums;
using CryptoExchange.Net.Objects;
using Binance.Net.Objects.Spot.MarketData;

namespace TradingBot.Core.Api.Binance
{
    public class BinanceManager : IExchangeApi
    {
        private string _accessKey;
        private string _accessSecret;
        private BinanceClient _client;

        public BinanceManager(string accessKey, string accessSecret)
        {
            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(accessKey, accessSecret),
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { Console.Out }
            });
        }

        public bool Buy(string symbol, int amount, decimal price)
        {
            using (var client = new BinanceClient())
            {
                throw new NotImplementedException();
            }
        }

        public IList<Candle> GetData(string ticker, Timeframe timeFrame, int amount)
        {
            WebCallResult<IEnumerable<BinanceKline>> klines;
            using (var client = new BinanceClient())
            {
                klines = client.GetKlines(ticker, MapTimeFrame(timeFrame), startTime: DateTime.UtcNow.AddHours(-24), endTime: DateTime.UtcNow, limit: 1000);
            }
            if (klines.Success)
                return ExtractCandles(klines, ticker, timeFrame);
            else
                return new List<Candle>();
        }

        public IList<Candle> GetData(string ticker, Timeframe timeFrame, int amount, DateTime dateTo)
        {
            WebCallResult<IEnumerable<BinanceKline>> klines;
            using (var client = new BinanceClient())
            {
                klines = client.GetKlines(ticker, MapTimeFrame(timeFrame), startTime: DateTime.UtcNow.AddHours(-24), endTime: dateTo, limit: 1000);
            }
            if (klines.Success)
                return ExtractCandles(klines, ticker, timeFrame);
            else
                return new List<Candle>();
        }

        private KlineInterval MapTimeFrame(Timeframe timeFrame)
        {
            var result = new KlineInterval();
            switch (timeFrame)
            {
                case Timeframe.OneMinute:
                    result = KlineInterval.OneMinute;
                    break;
                case Timeframe.FiveMinute:
                    result = KlineInterval.FiveMinutes;
                    break;
                case Timeframe.FiveteenMinute:
                    result = KlineInterval.FifteenMinutes;
                    break;
                case Timeframe.ThirtyMinute:
                    result = KlineInterval.ThirtyMinutes;
                    break;
                case Timeframe.OneHour:
                    result = KlineInterval.OneHour;
                    break;
                case Timeframe.SixHour:
                    result = KlineInterval.SixHour;
                    break;
                case Timeframe.TwelveHour:
                    result = KlineInterval.TwelveHour;
                    break;
                case Timeframe.OneDay:
                    result = KlineInterval.OneDay;
                    break;
                case Timeframe.SevenDay:
                    result = KlineInterval.OneWeek;
                    break;
                case Timeframe.OneMonth:
                    result = KlineInterval.OneMonth;
                    break;
                default:
                    break;
            }
            return result;
        }

        private IList<Candle> ExtractCandles(WebCallResult<IEnumerable<BinanceKline>> klines, string ticker, Timeframe timeFrame)
        {
            List<Candle> internalCandles = new List<Candle>();
            foreach (var kline in klines.Data)
            {
                internalCandles.Add(new Candle()
                    {
                        Timestamp = kline.OpenTime,
                        Close = kline.Close,
                        High = kline.High,
                        Open = kline.Open,
                        Low = kline.Low,
                        Volume = kline.Volume,
                        TimeFrame = timeFrame,
                        Ticker = ticker
                    }
                );
            }
            return internalCandles;
        }
    }
}
