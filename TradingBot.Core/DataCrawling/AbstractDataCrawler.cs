using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects;
using Microsoft.EntityFrameworkCore;
using TradingBot.Core.Api;
using TradingBot.Core.Common;

namespace TradingBot.Core.DataCrawling
{
    public class AbstractDataCrawler : IDataCrawler
    {
        protected IExchangeApi _apiManager;
        protected string _ticker;
        private Task _currentTask;
        private CancellationToken _cancelletionToken;
        CancellationTokenSource _cancelTokenSource;

        public void Run(TimeSpan period, int amountOfCandles, string ticker)
        {
            _cancelTokenSource = new CancellationTokenSource();
            _cancelletionToken = _cancelTokenSource.Token;
            _ticker = ticker;
            _currentTask = Task.Factory.StartNew(() => CrawlData(period, amountOfCandles, _apiManager, _ticker, _cancelletionToken), _cancelletionToken);
        }

        public void Terminate()
        {
            _cancelTokenSource.Cancel();
        }

        private static void CrawlData(TimeSpan period, int amountOfCandles, IExchangeApi apiManager, string ticker, CancellationToken cancelletionToken)
        {
            while (!cancelletionToken.IsCancellationRequested)
            {
                CrawlForTimeFrame(Timeframe.FiveMinute, ticker, amountOfCandles, apiManager);
                Thread.Sleep(100);

                CrawlForTimeFrame(Timeframe.FiveMinute, ticker, amountOfCandles, apiManager);
                Thread.Sleep(100);

                CrawlForTimeFrame(Timeframe.FiveMinute, ticker, amountOfCandles, apiManager);
                Thread.Sleep(100);

                CrawlForTimeFrame(Timeframe.FiveMinute, ticker, amountOfCandles, apiManager);
                Thread.Sleep(period);

                CrawlForTimeFrame(Timeframe.OneMinute, ticker, amountOfCandles, apiManager);
                Thread.Sleep(period);

                CrawlForTimeFrame(Timeframe.OneMonth, ticker, amountOfCandles, apiManager);
                Thread.Sleep(period);

                CrawlForTimeFrame(Timeframe.SevenDay, ticker, amountOfCandles, apiManager);
                Thread.Sleep(period);

                CrawlForTimeFrame(Timeframe.OneDay, ticker, amountOfCandles, apiManager);
                Thread.Sleep(period);

                CrawlForTimeFrame(Timeframe.TwelveHour, ticker, amountOfCandles, apiManager);
                Thread.Sleep(period);

                CrawlForTimeFrame(Timeframe.SixHour, ticker, amountOfCandles, apiManager);
                Thread.Sleep(period);

                CrawlForTimeFrame(Timeframe.OneHour, ticker, amountOfCandles, apiManager);
                Thread.Sleep(period);

                CrawlForTimeFrame(Timeframe.ThirtyMinute, ticker, amountOfCandles, apiManager);
                Thread.Sleep(period);

                CrawlForTimeFrame(Timeframe.FiveteenMinute, ticker, amountOfCandles, apiManager);
                Thread.Sleep(period);


            }
        }

        private static void CrawlForTimeFrame(Timeframe timeFrame, string ticker, int amountOfCandles, IExchangeApi apiManager)
        {
            DateTime? lastTimestamp;
            using (var dbContext = new ApplicationContext())
            {
                lastTimestamp = GetLastTimestamp(dbContext.Candles, timeFrame, ticker);
                var newCandles = apiManager.GetData(ticker, timeFrame, amountOfCandles);
                foreach (var candle in newCandles)
                    CreateOrUpdate(candle, dbContext);
                dbContext.SaveChanges();
                CrawlNewCandles(timeFrame, ticker, amountOfCandles, apiManager, dbContext);
                dbContext.SaveChanges();
            }
        }

        private static void CrawlNewCandles(Timeframe timeFrame, string ticker, int amountOfCandles, IExchangeApi apiManager, ApplicationContext dbContext)
        {
            // Get new candles
            var firstTimeStamp = GetFirstTimestamp(dbContext.Candles, timeFrame, ticker);
            DateTime endDate = firstTimeStamp.Value != DateTime.MinValue ? firstTimeStamp.Value : DateTime.Now.ToUniversalTime();
            var newCandles = apiManager.GetData(ticker, timeFrame, amountOfCandles, endDate).Take(amountOfCandles - 1);

            // Save new data to store
            foreach (var candle in newCandles)
            {
                dbContext.Candles.Add(candle);
            }
        }

        public static DateTime? GetLastTimestamp(DbSet<Candle> dbSet, Timeframe timeFrame, string ticker)
        {
            return dbSet
                .Where(c => c.Ticker == ticker)
                .Where(c => c.TimeFrame == timeFrame)
                .OrderBy(c => c.Timestamp)
                .Select(c => c.Timestamp)
                .LastOrDefault();
        }

        public static DateTime? GetFirstTimestamp(DbSet<Candle> dbSet, Timeframe timeFrame, string ticker)
        {
            return dbSet
                .Where(c => c.Ticker == ticker)
                .Where(c => c.TimeFrame == timeFrame)
                .OrderBy(c => c.Timestamp)
                .Select(c => c.Timestamp)
                .FirstOrDefault();
        }

        private static void CreateOrUpdate(Candle candle, ApplicationContext dbContext)
        {
            var candleToUpdate = dbContext.Candles.Where(c => c.Timestamp == candle.Timestamp).FirstOrDefault();
            candleToUpdate.High = candle.High;
            candleToUpdate.Low = candle.Low;
            candleToUpdate.Open = candle.Open;
            candleToUpdate.Close = candle.Close;
            candleToUpdate.Volume = candle.Volume;

            if (candleToUpdate != null)
                dbContext.Update(candle);
            else
                dbContext.Add(candle);
        }
    }
}
