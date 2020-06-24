using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;
using System.Linq;
using TradingBot.Core.Common;

namespace TradingBot.Core
{
    public class HistoryDataProvider : IDataProvider
    {
        private BitfinexManager _bitfinexManager;
        private Timeframe _timeFrame;
        private int _amountPerPortion;
        private int _totalAmount;
        private IList<Candle> _cachedCandles;
        private int _lastCandleIndex = -1;

        public HistoryDataProvider(BitfinexManager bitfinexManager, Timeframe timeFrame, int amountPerPortion, int totalAmount)
        {
            _bitfinexManager = bitfinexManager;
            _timeFrame = timeFrame;
            _amountPerPortion = amountPerPortion;
            _totalAmount = totalAmount;
        }

        public IList<Candle> GetData(string ticker)
        {
            throw new NotImplementedException();
            //return GetDataInternal(ticker, d => true);
        }

        public IList<Candle> GetData(string ticker, DateTime dateFrom, DateTime dateTo)
        {
            return GetDataInternal(ticker, d => d.Timestamp >= dateFrom && d.Timestamp <= dateTo, dateFrom, dateTo);
        }

        private IList<Candle> GetDataInternal(string ticker, Func<Candle, bool> predicate, DateTime dateFrom, DateTime dateTo)
        {
            List<Candle> result = new List<Candle>();
            Candle targetCandle = null;
            if (_cachedCandles == null) // данных в кеше нет
            {
                //_cachedCandles = GetAllData(ticker, dateFrom, dateTo)
                _cachedCandles = GetAllData(ticker) // читаем все данные без фильтрации в кеш
                    .ToList();

                targetCandle = _cachedCandles.FirstOrDefault(predicate); // берем первую свечку, удовлетворяющую условию фильтрации
            }
            else if (_cachedCandles.Count >= _lastCandleIndex + 2) // в коллекции есть следующая свеча
            {

                targetCandle = _cachedCandles.Skip(_lastCandleIndex + 1).FirstOrDefault(predicate); // берем ее
            }

            if (targetCandle == null)
                return result;

            _lastCandleIndex = _cachedCandles.IndexOf(targetCandle); // записываем ее индекс для последующей выборки

            // загрузка предшествующей области для целовой свечи
            if (_lastCandleIndex < _amountPerPortion - 1) // если индекс целевой свечки меньше порции загрузки
            {
                // значит данных, меньше, чем размер порции и требуется загрузить то, что есть
                result.AddRange(_cachedCandles.Take(_lastCandleIndex));
                // не забываем добавить в конец саму свечу
                result.Add(targetCandle);
            }
            else // если же данных больше или равно размеру порции
            {
                // то грузим кол-во, равное порции - 1, т.к. сама свеча тоже входит в порцию, дабавим ее в конец
                result.AddRange(
                    _cachedCandles
                        .Skip(_lastCandleIndex + 1 - _amountPerPortion)
                        .Take(_amountPerPortion - 1)
                    );
                result.Add(targetCandle);
            }

            return result;
        }

        private IList<Candle> GetAllData(string ticker)
        {
           return _bitfinexManager.GetData(ticker, _timeFrame, _totalAmount);
        }

        private IList<Candle> GetAllData(string ticker, DateTime dateTo)
        {
            return _bitfinexManager.GetData(ticker, _timeFrame, _totalAmount, dateTo);
        }

        public void ClearLastIndex()
        {
            _lastCandleIndex = -1;
        }
    }
}
