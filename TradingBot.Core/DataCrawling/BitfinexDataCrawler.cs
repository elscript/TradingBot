using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects;
using Microsoft.EntityFrameworkCore;
using TradingBot.Core.Common;

namespace TradingBot.Core.DataCrawling
{
    public class BitfinexDataCrawler : IDataCrawler
    {
        private BitfinexManager _apiManager;
        private string _ticker;
        private Task _currentTask;
        private CancellationToken _cancelletionToken;
        CancellationTokenSource _cancelTokenSource;

        public BitfinexDataCrawler(BitfinexManager apiManager, string ticker)
        {
            _apiManager = apiManager;
            _ticker = ticker;
        }

        public void Run(TimeSpan period, int amountOfCandles)
        {
            _cancelTokenSource = new CancellationTokenSource();
            _cancelletionToken = _cancelTokenSource.Token;
            _currentTask = Task.Factory.StartNew(() => CrawlData(period, amountOfCandles, _apiManager, _ticker, _cancelletionToken), _cancelletionToken);
        }

        private static void CrawlData(TimeSpan period, int amountOfCandles, BitfinexManager apiManager, string ticker, CancellationToken cancelletionToken)
        {
            while (!cancelletionToken.IsCancellationRequested)
            {
                CrawlForTimeFrame(TimeFrame.OneMonth, ticker, amountOfCandles, apiManager);
                CrawlForTimeFrame(TimeFrame.FourteenDay, ticker, amountOfCandles, apiManager);
                CrawlForTimeFrame(TimeFrame.SevenDay, ticker, amountOfCandles, apiManager);
                CrawlForTimeFrame(TimeFrame.OneDay, ticker, amountOfCandles, apiManager);
                CrawlForTimeFrame(TimeFrame.TwelfHour, ticker, amountOfCandles, apiManager);
                CrawlForTimeFrame(TimeFrame.SixHour, ticker, amountOfCandles, apiManager);
                CrawlForTimeFrame(TimeFrame.ThreeHour, ticker, amountOfCandles, apiManager);
                CrawlForTimeFrame(TimeFrame.OneHour, ticker, amountOfCandles, apiManager);
                CrawlForTimeFrame(TimeFrame.ThirtyMinute, ticker, amountOfCandles, apiManager);
                CrawlForTimeFrame(TimeFrame.FiveteenMinute, ticker, amountOfCandles, apiManager);
                CrawlForTimeFrame(TimeFrame.FiveMinute, ticker, amountOfCandles, apiManager);
                CrawlForTimeFrame(TimeFrame.OneMinute, ticker, amountOfCandles, apiManager);
                Thread.Sleep(period);
            }
        }

        private static void CrawlForTimeFrame(TimeFrame timeFrame, string ticker, int amountOfCandles, BitfinexManager apiManager)
        {
            DateTime? lastTimestamp;
            using (var dbContext = new ApplicationContext())
            {
                lastTimestamp = GetLastTimestamp(dbContext.Candles, timeFrame, ticker);
                var lastRemoteCandle = apiManager.GetData(ticker, timeFrame, 1).Single();
                if (lastTimestamp == lastRemoteCandle.Timestamp)
                    UpdateLastCandle(new Candle(lastRemoteCandle, timeFrame, ticker), dbContext);
                else
                    CrawlNewCandles(timeFrame, ticker, amountOfCandles, apiManager, dbContext);
            }
        }

        private static void CrawlNewCandles(TimeFrame timeFrame, string ticker, int amountOfCandles, BitfinexManager apiManager, ApplicationContext dbContext)
        {
            // Get new candles
            var lastTimeStamp = GetLastTimestamp(dbContext.Candles, timeFrame, ticker);
            DateTime startDate;
            startDate = lastTimeStamp ?? DateTime.MinValue;
            var newCandles = apiManager.GetData(ticker, timeFrame, amountOfCandles, startDate, DateTime.Now.ToUniversalTime());
            
            // Save new data to store
            foreach (var candle in newCandles)
            {
                dbContext.Candles.Add(new Candle(candle, timeFrame, ticker));
            }

            dbContext.SaveChanges();
        }

        private static void UpdateLastCandle(Candle candle, ApplicationContext dbContext)
        {
            dbContext.Update(candle);
        }

        public void Terminate()
        {
            _cancelTokenSource.Cancel();
        }

        public static DateTime? GetLastTimestamp(DbSet<Candle> dbSet, TimeFrame timeFrame, string ticker)
        {
            return dbSet
                .Where(c => c.Ticker == ticker)
                .Where(c => c.TimeFrame == timeFrame)
                .OrderBy(c => c.Timestamp)
                .Select(c => c.Timestamp)
                .LastOrDefault();
        }
    }
}
