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
        private PositionInternal _position;
        private IStrategy _strategy;
        private IDataProvider _provider;
        
        public IList<PositionInternal> PlayedPositions { get; private set; }
        public decimal ProfitRate { get; protected set; }

        public StrategyPlayer(IStrategy strategy, IDataProvider dataProvider)
        {
            _strategy = strategy;
            _provider = dataProvider;
            PlayedPositions = new List<PositionInternal>();
        }

        protected abstract void OnOpenPosition(PositionInternal position);

        protected abstract void OnClosePosition(PositionInternal position);

        private void Execute(IList<DataSample> samples)
        {
            var sample = samples.Last();

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
                    ClosePosition(PositionDirection.Long, signalResult.ByStopLoss, sample);

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
                    ClosePosition(PositionDirection.Short, signalResult.ByStopLoss, sample);

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
        public void Run(string ticker)
        {
            ProfitRate = 0;
            PlayedPositions.Clear();

            while (true)
            {
                Execute(PrepareData(_provider.GetData(ticker)));
            }
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
        private void CalculateProfitRate()
        {
            ProfitRate = 0;
            foreach (var position in PlayedPositions)
            {
                if (position.Direction == PositionDirection.Long)
                    ProfitRate += (position.ClosePrice - position.OpenPrice) / position.OpenPrice;
                else if (position.Direction == PositionDirection.Short)
                    ProfitRate += (position.OpenPrice - position.ClosePrice) / position.OpenPrice;
            }
        }
    }
}
