using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingBot.Core.Common;

namespace TradingBot.Core.DataProviders
{
    public class DbDataProvider : IDataProvider
    {
        private readonly Timeframe timeframe;

        public DbDataProvider(Timeframe timeframe)
        {
            this.timeframe = timeframe;
        }

        public IList<Candle> GetData(string ticker)
        {
            IList<Candle> candles = new List<Candle>();

            using (ApplicationContext db = new ApplicationContext())
            {
                candles = db.Candles.ToList();
            }

            return candles;
        }

        public IList<Candle> GetData(string ticker, DateTime dateFrom, DateTime dateTo)
        {
            IList<Candle> candles = new List<Candle>();

            using (ApplicationContext db = new ApplicationContext())
            {
                candles = db.Candles
                    .Where(c => c.Timestamp >= dateFrom && c.Timestamp <= dateTo)
                    .ToList();
            }

            return candles;
        }
    }
}
