using System;
using System.Collections.Generic;
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

            do
            {
                resultCandles = _binanceMananger.GetData(ticker, timeframe, 1000, fixedDateTime.AddDays(-10 * iteration), fixedDateTime);
                if (allCandles.Any() && resultCandles.Last().Timestamp == allCandles.Last().Timestamp)
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
