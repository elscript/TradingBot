using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net.Objects;
using TradingBot.Core.Common;

namespace TradingBot.Core
{
    public class HistoricalStrategyPlayer : StrategyPlayer
    {
        private DateTime _dateFrom;
        private DateTime _dateTo;
        private IList<Candle> _candlesCache;

        public HistoricalStrategyPlayer(IStrategy strategy, IDataProducer dataProvider, Position startPosition) : base(strategy, dataProvider, startPosition)
        {
        }

        protected override void OnOpenPosition(Position position)
        {
        
        }

        protected override void OnClosePosition(Position position)
        {
            
        }

        protected override bool ShouldContinue(string ticker, Timeframe timeFrame)
        {
            _candlesCache = GetDataInternal(ticker, timeFrame);
            return _candlesCache.Any();
        }

        private IList<Candle> GetDataInternal(string ticker, Timeframe timeFrame)
        {
            return Producer.GetData(ticker, timeFrame, _dateFrom, _dateTo);
        }

        protected override IList<Candle> GetData(string ticker, Timeframe timeFrame)
        {
            return _candlesCache;
        }

        protected override void OnStop()
        {
            ((HistoryDataProducer)Producer).ClearLastIndex();
        }

        protected override decimal GetAmount(decimal initialAmount, string currency)
        {
            return initialAmount;
        }

        protected override void SetCurrentPosition()
        {
            //TODO выставление позиции
        }

        public void SetDateRange(DateTime dateFrom, DateTime dateTo)
        {
            _dateFrom = dateFrom;
            _dateTo = dateTo;
        }

        protected override void OnSetStopLoss(decimal price)
        {
            //TODO выставить стоп
        }
    }
}
