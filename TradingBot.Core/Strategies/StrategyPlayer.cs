﻿using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;
using TradingBot.Core;
using System.Linq;
using TradingBot.Core.Common;

namespace TradingBot.Core
{
    public abstract class StrategyPlayer
    {
        protected Position Position;
        private readonly IStrategy _strategy;
        protected IDataProducer Producer;

        public IList<Position> PlayedPositions { get; private set; }
        public decimal ProfitRate { get; protected set; }

        protected StrategyPlayer(IStrategy strategy, IDataProducer dataProducer, Position startPosition)
        {
            _strategy = strategy;
            Producer = dataProducer;
            PlayedPositions = new List<Position>();
            Position = startPosition;
        }

        protected abstract void OnOpenPosition(Position position);

        protected abstract void OnClosePosition(Position position);

        protected abstract void OnSetStopLoss(decimal price);

        protected abstract void OnStop();

        protected abstract bool ShouldContinue(string ticker, Timeframe timeframe);

        protected abstract IList<Candle> GetData(string ticker, Timeframe timeFrame);

        protected abstract decimal GetAmount(decimal initialAmount, string currency);

        protected abstract void SetCurrentPosition();

        private void Execute(IList<DataSample> samples, string ticker, decimal amount)
        {
            var sample = samples.Last();

            if (Position == null)
            {
                if (_strategy.BuySignal(samples, sample, null).SignalTriggered)
                {
                    OpenPosition(PositionDirection.Long, sample, ticker, amount);
                }
                else if (_strategy.SellSignal(samples, sample, null).SignalTriggered)
                {
                    OpenPosition(PositionDirection.Short, sample, ticker, amount);
                }
            }
            else if (Position.Direction == PositionDirection.Long)
            {
                var signalResult = _strategy.SellSignal(samples, sample, Position.OpenPrice);
                if (signalResult.SignalTriggered)
                {
                    ClosePosition(PositionDirection.Long, signalResult.ByStopLoss, sample);

                    if (_strategy.AllowShort)
                    {
                        OpenPosition(PositionDirection.Short, sample, ticker, amount);
                    }
                }
            }
            else if (Position.Direction == PositionDirection.Short)
            {
                var signalResult = _strategy.BuySignal(samples, sample, Position.OpenPrice);
                if (signalResult.SignalTriggered)
                {
                    ClosePosition(PositionDirection.Short, signalResult.ByStopLoss, sample);

                    if (_strategy.AllowLong)
                    {
                        OpenPosition(PositionDirection.Long, sample, ticker, amount);
                    }
                }
            }

            if (Position != null)
            {
                SetStopLoss(samples, sample, Position);
            }
        }

        private void OpenPosition(PositionDirection direction, DataSample sample, string ticker, decimal amount)
        {
            Position = new Position
            {
                OpenPrice = sample.Candle.Close,
                Direction = direction,
                OpenTimestamp = sample.Candle.Timestamp,
                Ticker = ticker,
                Amount = amount
            };
            this.OnOpenPosition(Position);
        }

        private void ClosePosition(PositionDirection direction, bool byStopLoss, DataSample sample)
        {
            Position.ClosePrice = sample.Candle.Close;
            Position.CloseTimestamp = sample.Candle.Timestamp;
            PlayedPositions.Add(Position);
            this.OnClosePosition(Position);
            Position = null;
        }

        /// <summary>
        /// Запуск стратегии
        /// </summary>
        /// <param name="ticker">Валютная пара</param>
        public void Run(string ticker, Timeframe timeFrame, decimal initialAmount, string currency)
        {
            ProfitRate = 0;
            PlayedPositions.Clear();
            SetCurrentPosition();

            while (ShouldContinue(ticker, timeFrame))
            {
                Execute(PrepareData(GetData(ticker, timeFrame)), ticker, GetAmount(initialAmount, currency));
            }

            this.ProfitRate = CalculateProfitRate();
            this.OnStop();
        }

        private IList<DataSample> PrepareData(IList<Candle> candles)
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

        private void SetStopLoss(IList<DataSample> samples, DataSample sample, Position position)
        {
            this.OnSetStopLoss(_strategy.GetStopLossPrice(samples, sample, position));    
        }
    }
}
