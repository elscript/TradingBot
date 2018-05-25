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
                    _position = new PositionInternal();
                    _position.OpenPrice = sample.Candle.Close;
                    _position.Direction = PositionDirection.Long;
                    _position.OpenTimestamp = sample.Candle.Timestamp;
                    this.OnOpenPosition(_position);
                }

                if (_strategy.SellSignal(samples, sample, null).SignalTriggered)
                {
                    _position = new PositionInternal();
                    _position.OpenPrice = sample.Candle.Close;
                    _position.Direction = PositionDirection.Short;
                    _position.OpenTimestamp = sample.Candle.Timestamp;
                    this.OnOpenPosition(_position);
                }
            }
            else if (_position.Direction == PositionDirection.Long)
            {
                var signalResult = _strategy.SellSignal(samples, sample, _position.OpenPrice);
                if (signalResult.SignalTriggered)
                {
                    _position.ClosePrice = signalResult.ByStopLoss ? _position.OpenPrice - _position.OpenPrice * _strategy.MaxLoosePercentage / 100 : sample.Candle.Close;
                    _position.CloseTimestamp = sample.Candle.Timestamp;
                    PlayedPositions.Add(_position);
                    this.OnClosePosition(_position);
                    _position = null;

                    if (_strategy.AllowShort)
                    {
                        _position = new PositionInternal();
                        _position.OpenPrice = sample.Candle.Close;
                        _position.Direction = PositionDirection.Short;
                        _position.OpenTimestamp = sample.Candle.Timestamp;
                        this.OnOpenPosition(_position);
                    }
                }
            }
            else if (_position.Direction == PositionDirection.Short)
            {
                var signalResult = _strategy.BuySignal(samples, sample, _position.OpenPrice);
                if (signalResult.SignalTriggered)
                {
                    _position.ClosePrice = signalResult.ByStopLoss ? _position.OpenPrice + _position.OpenPrice * _strategy.MaxLoosePercentage / 100 : sample.Candle.Close;
                    _position.CloseTimestamp = sample.Candle.Timestamp;
                    PlayedPositions.Add(_position);
                    this.OnClosePosition(_position);
                    _position = null;

                    if (_strategy.AllowLong)
                    {
                        _position = new PositionInternal();
                        _position.OpenPrice = sample.Candle.Close;
                        _position.Direction = PositionDirection.Long;
                        _position.OpenTimestamp = sample.Candle.Timestamp;
                        this.OnOpenPosition(_position);
                    }
                }
            }
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
