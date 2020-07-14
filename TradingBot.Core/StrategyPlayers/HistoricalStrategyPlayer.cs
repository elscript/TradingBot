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

        public HistoricalStrategyPlayer(IStrategy strategy, IDataProducer dataProducer, Position startPosition, decimal fee, int maximumLeverage) : base(strategy, dataProducer, startPosition, fee, maximumLeverage)
        {
        }

        protected override void OnOpenPosition(Position position)
        {
            CurrentBalance = CurrentBalance - (position.Amount * position.OpenPrice * Fee);
        }

        protected override void OnClosePosition(Position position)
        {
            if (position.Direction == PositionDirection.Long)
                CurrentBalance = CurrentBalance + (position.Amount * (position.ClosePrice - position.OpenPrice)) - (position.Amount * position.ClosePrice) * Fee;
            else if (position.Direction == PositionDirection.Short)
                CurrentBalance = CurrentBalance + (position.Amount * (position.OpenPrice - position.ClosePrice)) - (position.Amount * position.ClosePrice) * Fee;
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
            return CurrentBalance;
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

        protected override void OnClosePositionByStopLoss(Position position)
        {
            CurrentBalance = CurrentBalance - (position.Amount * Math.Abs(position.OpenPrice - position.StopLossPrice) - (position.Amount * position.StopLossPrice * Fee));
        }
    }
}
