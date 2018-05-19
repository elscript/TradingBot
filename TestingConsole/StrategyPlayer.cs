using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;

namespace TestingConsole
{
    public class StrategyPlayer
    {
        private PositionInternal _position;
        private IStrategy _strategy;
        public IList<PositionInternal> PlayedPositions { get; private set; }

        public StrategyPlayer(IStrategy strategy)
        {
            _strategy = strategy;
            PlayedPositions = new List<PositionInternal>();
        }

        /// <summary>
        /// Запуск стратегии
        /// </summary>
        /// <param name="candles">Свечи</param>
        /// <returns>Доля профита, полученного с использованием стратегии</returns>
        public decimal Run(IList<BitfinexCandle> candles)
        {
            PlayedPositions.Clear();
            decimal profitRate = 0;
            var processor = new CandlesDataProcessor(candles);
            processor.CalculateEMAs(12, 26);
            processor.CalculateIndicators(9, 14);

            foreach (var sample in processor.Samples)
            {
                if (_position == null)
                {
                    if (_strategy.BuySignal(processor.Samples, sample, null))
                    {
                        _position = new PositionInternal();
                        _position.OpenPrice = sample.Candle.Close;
                        _position.Direction = PositionDirection.Long;
                        _position.OpenTimestamp = sample.Candle.Timestamp;
                    }
                }
                else if (_position.Direction == PositionDirection.Long)
                {
                    if (_strategy.SellSignal(processor.Samples, sample, _position.OpenPrice))
                    {
                        _position.ClosePrice = sample.Candle.Close;
                        _position.CloseTimestamp = sample.Candle.Timestamp;
                        PlayedPositions.Add(_position);
                        _position = null;
                    }

                    if (_strategy.AllowShort)
                    {
                        _position = new PositionInternal();
                        _position.OpenPrice = sample.Candle.Close;
                        _position.Direction = PositionDirection.Short;
                        _position.OpenTimestamp = sample.Candle.Timestamp;
                    }
                }
                else if (_position.Direction == PositionDirection.Short)
                {
                    if (_strategy.BuySignal(processor.Samples, sample, _position.OpenPrice))
                    {
                        _position.ClosePrice = sample.Candle.Close;
                        _position.CloseTimestamp = sample.Candle.Timestamp;
                        PlayedPositions.Add(_position);
                        _position = null;
                    }

                    if (_strategy.AllowLong)
                    {
                        _position = new PositionInternal();
                        _position.OpenPrice = sample.Candle.Close;
                        _position.Direction = PositionDirection.Long;
                        _position.OpenTimestamp = sample.Candle.Timestamp;
                    }
                }
            }

            foreach (var position in PlayedPositions)
            {
                if (position.Direction == PositionDirection.Long)
                    profitRate += (position.ClosePrice - position.OpenPrice) / position.OpenPrice;
                else if (position.Direction == PositionDirection.Short)
                    profitRate += (position.OpenPrice - position.ClosePrice) / position.OpenPrice;
            }

            return profitRate;
        }
    }
}
