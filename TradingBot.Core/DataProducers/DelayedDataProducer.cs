using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net.Objects;
using System.Threading;
using TradingBot.Core.Common;

namespace TradingBot.Core
{
    public class DelayedDataProducer : IDataProducer
    {
        private BitfinexManager _bitfinexManager;
        private int _amount;
        private int _delay;

        public DelayedDataProducer(BitfinexManager bitfinexManager, int amount, int delay)
        {
            _bitfinexManager = bitfinexManager;
            _amount = amount;
            _delay = delay;
        }

        public IList<Candle> GetData(string ticker, Timeframe timeFrame)
        {
            Thread.Sleep(_delay);
            return _bitfinexManager.GetData(ticker, timeFrame, _amount).SkipLast(1).ToList();
        }

        public IList<Candle> GetData(string ticker, Timeframe timeFrame, DateTime dateFrom, DateTime dateTo)
        {
            throw new NotImplementedException();
        }
    }
}
