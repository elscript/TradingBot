using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net.Objects;
using System.Threading;
using TradingBot.Core.Common;

namespace TradingBot.Core
{
    public class DelayedDataProvider : IDataProvider
    {
        private BitfinexManager _bitfinexManager;
        private TimeFrame _timeFrame;
        private int _amount;
        private int _delay;

        public DelayedDataProvider(BitfinexManager bitfinexManager, TimeFrame timeFrame, int amount, int delay)
        {
            _bitfinexManager = bitfinexManager;
            _timeFrame = timeFrame;
            _amount = amount;
            _delay = delay;
        }

        public IList<Candle> GetData(string ticker)
        {
            Thread.Sleep(_delay);
            return _bitfinexManager.GetData(ticker, _timeFrame, _amount).SkipLast(1).ToList();
        }

        public IList<Candle> GetData(string ticker, DateTime dateFrom, DateTime dateTo)
        {
            throw new NotImplementedException();
        }
    }
}
