using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;

namespace TestingConsole
{
    public abstract class StrategyPlayer
    {
        private PositionInternal _position;
        private IStrategy _strategy;
        public IList<PositionInternal> PlayedPositions { get; private set; }
        public decimal ProfitRate { get; protected set; }

        public StrategyPlayer(IStrategy strategy)
        {
            _strategy = strategy;
            PlayedPositions = new List<PositionInternal>();
        }

        protected abstract void OnOpenPosition(PositionInternal position);

        protected abstract void OnClosePosition(PositionInternal position);

        protected abstract void RunStrategy(CandlesDataProcessor processor);

        protected abstract void CalculateProfitRate();

        protected void DoMainLogic(IList<DataSample> samples, DataSample sample)
        {
            if (_position == null)
            {
                if (_strategy.BuySignal(samples, sample, null).SignalTriggered)
                {
                    OpenPosition(PositionDirection.Long, sample);
                }

                if (_strategy.SellSignal(samples, sample, null).SignalTriggered)
                {
                    OpenPosition(PositionDirection.Short, sample);
                }
            }
            else if (_position.Direction == PositionDirection.Long)
            {
                var signalResult = _strategy.SellSignal(samples, sample, _position.OpenPrice);
                if (signalResult.SignalTriggered)
                {
                    ClosePosition(PositionDirection.Long, signalResult, sample);

                    if (_strategy.AllowShort)
                    {
                        OpenPosition(PositionDirection.Short, sample);
                    }
                }
            }
            else if (_position.Direction == PositionDirection.Short)
            {
                var signalResult = _strategy.BuySignal(samples, sample, _position.OpenPrice);
                if (signalResult.SignalTriggered)
                {
                    ClosePosition(PositionDirection.Short, signalResult, sample);

                    if (_strategy.AllowLong)
                    {
                        OpenPosition(PositionDirection.Long, sample);
                    }
                }
            }
        }

        private void OpenPosition(PositionDirection direction, DataSample sample)
        {
            _position = new PositionInternal();
            _position.OpenPrice = sample.Candle.Close;
            _position.Direction = PositionDirection.Long;
            _position.OpenTimestamp = sample.Candle.Timestamp;
            this.OnOpenPosition(_position);
        }

        private void ClosePosition(PositionDirection direction, SignalResult signalResult, DataSample sample)
        {
            if (direction == PositionDirection.Long)
                _position.ClosePrice = signalResult.ByStopLoss ? _position.OpenPrice - _position.OpenPrice * _strategy.MaxLoosePercentage / 100 : sample.Candle.Close;
            else if (direction == PositionDirection.Short)
                _position.ClosePrice = signalResult.ByStopLoss ? _position.OpenPrice + _position.OpenPrice * _strategy.MaxLoosePercentage / 100 : sample.Candle.Close;
            _position.CloseTimestamp = sample.Candle.Timestamp;
            PlayedPositions.Add(_position);
            this.OnClosePosition(_position);
            _position = null;
        }

        /// <summary>
        /// Запуск стратегии
        /// </summary>
        /// <param name="candles">Свечи</param>
        /// <returns>Доля профита, полученного с использованием стратегии</returns>
        public void Run(IList<BitfinexCandle> candles)
        {
            var processor = new CandlesDataProcessor(candles);
            //processor.CalculateEMAs(12, 26);
            //processor.CalculateIndicators(9, 14);

            RunStrategy(processor);

            CalculateProfitRate();
        }
    }
}
