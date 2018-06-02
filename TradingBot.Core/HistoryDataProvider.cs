using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;
using System.Linq;

namespace TradingBot.Core
{
    public class HistoryDataProvider : IDataProvider
    {
        private BitfinexManager _bitfinexManager;
        private TimeFrame _timeFrame;
        private int _amountPerPortion;
        private int _totalAmount;
        private IList<BitfinexCandle> _cachedCandles;
        private int _lastCandleIndex = -1;

        public HistoryDataProvider(BitfinexManager bitfinexManager, TimeFrame timeFrame, int amountPerPortion, int totalAmount)
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
                _cachedCandles = GetAllData(ticker)
                    .ToList();

                result = _cachedCandles
                    .Take(1)
                    .ToList();

                _lastCandleIndex = _cachedCandles.IndexOf(result.Last());
            }
            else
            {
                if (_cachedCandles.Count > _lastCandleIndex + 1)
                {
                    if (_lastCandleIndex + 1 < _amountPerPortion)
                    {
                        result = _cachedCandles
                            .Take(_lastCandleIndex + 2)
                            .ToList();
                    }
                    else
                    {
                        result = _cachedCandles
                            .Skip(_lastCandleIndex - _amountPerPortion + 2)
                            .Take(_amountPerPortion)
                            .ToList();
                    }

                    _lastCandleIndex = _cachedCandles.IndexOf(result.Last());
                }
            }
            return result;
        }

        private IList<BitfinexCandle> GetAllData(string ticker)
        {
           return _bitfinexManager.GetData(ticker, _timeFrame, _totalAmount);
        }

        public IList<BitfinexCandle> GetData(string ticker, DateTime dateFrom, DateTime dateTo)
        {
            IList<BitfinexCandle> result = new List<BitfinexCandle>();
            if (_cachedCandles == null)
            {
                _cachedCandles = GetAllData(ticker)
                    .ToList();

                result = _cachedCandles
                    .Where(d => d.Timestamp >= dateFrom && d.Timestamp <= dateTo)
                    .Take(1)
                    .ToList();

                _lastCandleIndex = _cachedCandles.IndexOf(result.Last());
            }
            else
            {
                if (_cachedCandles.Count > _lastCandleIndex + 1)
                {
                    if (_lastCandleIndex + 1 < _amountPerPortion)
                    {
                        result = _cachedCandles
                            .Where(d => d.Timestamp >= dateFrom && d.Timestamp <= dateTo)
                            .Take(_lastCandleIndex + 2)
                            .ToList();
                    }
                    else // TODO решить проблему с выборкой
                    {
                        result = _cachedCandles
                            .Where(d => d.Timestamp >= dateFrom && d.Timestamp <= dateTo)
                            .Skip(_lastCandleIndex - _amountPerPortion + 2)
                            .Take(_amountPerPortion)
                            .ToList();
                    }

                    _lastCandleIndex = _cachedCandles.IndexOf(result.Last());
                }
            }
            return result;
        }

        public void ClearLastIndex()
        {
            _lastCandleIndex = -1;
        }
    }
}
