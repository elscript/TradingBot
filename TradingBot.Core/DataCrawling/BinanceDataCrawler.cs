using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using TradingBot.Core.Api.Binance;
using TradingBot.Core.Common;

namespace TradingBot.Core.DataCrawling
{
    public class BinanceDataCrawler
    {
        BinanceManager _binanceMananger;

        public BinanceDataCrawler(BinanceManager binanceMananger)
        {
            _binanceMananger = binanceMananger;
        }

        public void CrawlData(string ticker, Timeframe timeframe)
        {
            IList<Candle> resultCandles = new List<Candle>();
            IList<Candle> allCandles = new List<Candle>();
            var iteration = 1;
            var fixedDateTime = DateTime.UtcNow;
            var needToTerminate = false;
            var dateTimeFrom = fixedDateTime;

            do
            {
                switch (timeframe)
                {
                    case Timeframe.OneMinute:
                        dateTimeFrom = fixedDateTime.AddDays(-10 * iteration);
                        break;
                    case Timeframe.FiveMinute:
                        dateTimeFrom = fixedDateTime.AddDays(-50 * iteration);
                        break;
                    case Timeframe.FiveteenMinute:
                        dateTimeFrom = fixedDateTime.AddDays(-150 * iteration);
                        break;
                    case Timeframe.ThirtyMinute:
                        dateTimeFrom = fixedDateTime.AddDays(-300 * iteration);
                        break;
                    case Timeframe.OneHour:
                        dateTimeFrom = fixedDateTime.AddDays(-600 * iteration);
                        break;
                    case Timeframe.SixHour:
                        dateTimeFrom = fixedDateTime.AddDays(-3600 * iteration);
                        break;
                    case Timeframe.TwelveHour:
                        dateTimeFrom = fixedDateTime.AddDays(-7200 * iteration);
                        break;
                    case Timeframe.OneDay:
                        dateTimeFrom = fixedDateTime.AddDays(-14400 * iteration);
                        break;
                    case Timeframe.SevenDay:
                        dateTimeFrom = fixedDateTime.AddDays(-2000 * iteration);
                        break;
                    case Timeframe.OneMonth:
                        dateTimeFrom = fixedDateTime.AddDays(-2000 * iteration);
                        break;

                }

                resultCandles = _binanceMananger.GetData(ticker, timeframe, 1000, dateTimeFrom, fixedDateTime);
                if (allCandles.Any() && resultCandles.Any() && resultCandles.Last().Timestamp == allCandles.Last().Timestamp)
                {
                    needToTerminate = true;
                    break;
                }
                else if (!resultCandles.Any())
                {
                    needToTerminate = true;
                    break;
                }

                foreach (var item in resultCandles)
                {
                    allCandles.Add(item);
                }

                iteration++;
                fixedDateTime = allCandles.First().Timestamp;
            }
            while (!needToTerminate);

            using (ApplicationContext db = new ApplicationContext())
            {
                db.AddRange(allCandles);
                db.SaveChanges();
            }
        }
    }
}
