using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingBot.Core.Common;

namespace TradingBot.Core.DataProviders
{
    public class StorageDataProvider : IDataProvider
    {
        public IList<Candle> GetAllData(string ticker, Timeframe timeframe)
        {
            IList<Candle> candles = new List<Candle>();

            using (ApplicationContext db = new ApplicationContext())
            {
                candles = db.Candles
                    .Where(c => c.Ticker == ticker && c.TimeFrame == timeframe)
                    .OrderBy(c => c.Timestamp)
                    .ToList();
            }

            return candles;
        }

        public IList<Candle> GetDataForPeriod(string ticker, Timeframe timeframe, DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }
    }
}
