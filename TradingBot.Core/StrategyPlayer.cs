using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;
using TradingBot.Core;
using System.Linq;

namespace TradingBot.Core
{
    public abstract class StrategyPlayer
    {
        private Position _position;
        private readonly IStrategy _strategy;
        protected IDataProvider Provider;
        
        public IList<Position> PlayedPositions { get; private set; }
        public decimal ProfitRate { get; protected set; }

        protected StrategyPlayer(IStrategy strategy, IDataProvider dataProvider, Position startPosition)
        {
            _strategy = strategy;
            Provider = dataProvider;
            PlayedPositions = new List<Position>();
            _position = startPosition;
        }

        protected abstract void OnOpenPosition(Position position);

        protected abstract void OnClosePosition(Position position);

        protected abstract bool ShouldContinue(string ticker);

        protected abstract IList<BitfinexCandle> GetData(string ticker);

        protected abstract void OnStop();

        protected abstract decimal GetAmount(decimal initialAmount, string currency);

        private void Execute(IList<DataSample> samples, string ticker, decimal amount)
        {
            var sample = samples.Last();

            if (_position == null)
            {
                if (_strategy.BuySignal(samples, sample, null).SignalTriggered)
                {
                    OpenPosition(PositionDirection.Long, sample, ticker, amount);
                }

                if (_strategy.SellSignal(samples, sample, null).SignalTriggered)
                {
                    OpenPosition(PositionDirection.Short, sample, ticker, amount);
                }
            }
            else if (_position.Direction == PositionDirection.Long)
            {
                var signalResult = _strategy.SellSignal(samples, sample, _position.OpenPrice);
                if (signalResult.SignalTriggered)
                {
                    ClosePosition(PositionDirection.Long, signalResult.ByStopLoss, sample);

                    if (_strategy.AllowShort)
                    {
                        OpenPosition(PositionDirection.Short, sample, ticker, amount);
                    }
                }
            }
            else if (_position.Direction == PositionDirection.Short)
            {
                var signalResult = _strategy.BuySignal(samples, sample, _position.OpenPrice);
                if (signalResult.SignalTriggered)
                {
                    ClosePosition(PositionDirection.Short, signalResult.ByStopLoss, sample);

                    if (_strategy.AllowLong)
                    {
                        OpenPosition(PositionDirection.Long, sample, ticker, amount);
                    }
                }
            }
        }

        private void OpenPosition(PositionDirection direction, DataSample sample, string ticker, decimal amount)
        {
            _position = new Position
            {
                OpenPrice = sample.Candle.Close,
                Direction = direction,
                OpenTimestamp = sample.Candle.Timestamp,
                Ticker = ticker,
                Amount = amount
            };
            this.OnOpenPosition(_position);
        }

        private void ClosePosition(PositionDirection direction, bool byStopLoss, DataSample sample)
        {
            if (direction == PositionDirection.Long)
                _position.ClosePrice = byStopLoss ? _position.OpenPrice - _position.OpenPrice * _strategy.MaxLoosePercentage / 100 : sample.Candle.Close;
            else if (direction == PositionDirection.Short)
                _position.ClosePrice = byStopLoss ? _position.OpenPrice + _position.OpenPrice * _strategy.MaxLoosePercentage / 100 : sample.Candle.Close;
            _position.CloseTimestamp = sample.Candle.Timestamp;
            PlayedPositions.Add(_position);
            this.OnClosePosition(_position);
            _position = null;
        }

        /// <summary>
        /// Запуск стратегии
        /// </summary>
        /// <param name="ticker">Валютная пара</param>
        public void Run(string ticker, decimal initialAmount, string currency)
        {
            ProfitRate = 0;
            PlayedPositions.Clear();
            SetCurrentPosition();

            while (ShouldContinue(ticker))
            {
                Execute(PrepareData(GetData(ticker)), ticker, GetAmount(initialAmount, currency));
            }

            this.ProfitRate = CalculateProfitRate();
            this.OnStop();
        }

        private IList<DataSample> PrepareData(IList<BitfinexCandle> candles)
        {
            return new CandlesDataProcessor(candles).Samples;
            //processor.CalculateEMAs(12, 26);
            //processor.CalculateIndicators(9, 14);
        }

        /// <summary>
        /// Расчет доли прибыли
        /// </summary>
        private decimal CalculateProfitRate()
        {
            ProfitRate = 0;
            foreach (var position in PlayedPositions)
            {
                if (position.Direction == PositionDirection.Long)
                    ProfitRate += (position.ClosePrice - position.OpenPrice) / position.OpenPrice;
                else if (position.Direction == PositionDirection.Short)
                    ProfitRate += (position.OpenPrice - position.ClosePrice) / position.OpenPrice;
            }

            return ProfitRate;
        }

        private void SetCurrentPosition()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                _position = db.Positions.LastOrDefault(p => p.ClosePrice == 0);
            }
        }
    }
}
