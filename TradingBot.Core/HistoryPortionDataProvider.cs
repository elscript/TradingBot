using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;
using System.Linq;

namespace TradingBot.Core
{
    public class HistoryPortionDataProvider : IDataProvider
    {
        private BitfinexManager _bitfinexManager;
        private TimeFrame _timeFrame;
        private int _amountPerPortion;
        private int _totalAmount;
        private IList<BitfinexCandle> _cachedCandles;
        private int lastCandleIndex = 0;

        public HistoryPortionDataProvider(BitfinexManager bitfinexManager, TimeFrame timeFrame, int amountPerPortion, int totalAmount)
        {
            _bitfinexManager = bitfinexManager;
            _timeFrame = timeFrame;
            _amountPerPortion = amountPerPortion;
            _totalAmount = totalAmount;
        }

        public IList<BitfinexCandle> GetData(string ticker)
        {
            IList<BitfinexCandle> result = new List<BitfinexCandle>();
            if (_cachedCandles == null)
            {
                _cachedCandles = _bitfinexManager.GetData(ticker, _timeFrame, _totalAmount);
                result = _cachedCandles;
            }
            else
            {
                if (_cachedCandles.Count > lastCandleIndex + 1)
                {
                    result = _cachedCandles
                     .Skip(lastCandleIndex + 1)
                     .Take(_amountPerPortion)
                     .ToList();
                }
            }
            return result;
        }
    }
}
