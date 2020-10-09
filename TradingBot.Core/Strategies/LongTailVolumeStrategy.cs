using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingBot.Core.Common;

namespace TradingBot.Core.Strategies
{
    public class LongTailVolumeStrategy : StrategyBase
    {
        public LongTailVolumeStrategy(int stopLossAdditionalValuePercentage, int minimumProfitMultiplicator, decimal maxLoosePercentage, bool allowLong, bool allowShort) : base(stopLossAdditionalValuePercentage, minimumProfitMultiplicator, maxLoosePercentage, allowLong, allowShort)
        {
        }

        public override SignalResult BuySignal(IList<DataSample> samples, DataSample sample, Position position)
        {
            var levels = LevelsDeterminator.DeterminateLevels(samples).Where(lv => lv.TouchCount > 1);

            var indexOfSample = samples.IndexOf(sample);
            if (samples.Count() <= 3)
                return new SignalResult()
                {
                    ByStopLoss = false,
                    SignalTriggered = false
                };

            var previousSample = samples[indexOfSample - 1];
            var prevPrevSample = samples[indexOfSample - 2];
            var signalTriggered = false;
            if (samples.Count() > 2
                && sample.CandleColor == Common.CandleColor.Green
                && (prevPrevSample.CandleColor == Common.CandleColor.Red)
                //&& (previousSample.CandleColor == Common.CandleColor.Red || previousSample.CandleColor == Common.CandleColor.Grey)
                //&& sample.Candle.Volume * Math.Abs(sample.Candle.Close - sample.Candle.Open) > previousSample.Candle.Volume * Math.Abs(previousSample.Candle.Close - previousSample.Candle.Open)
                && IsLastAreaVolumeMoreThanPrevious(samples, previousSample, 50, 10, 1)
                && IsSampleWithLongTail(previousSample, PositionDirection.Long, 0.3m))
            {
                if (position != null)
                {
                    if (Math.Abs(sample.Candle.Close - position.OpenPrice) > Math.Abs(position.OpenPrice - position.StopLossPrice) * MinimumProfitMultiplicator)
                    {
                        signalTriggered = true;
                    }
                }
                else
                {
                    signalTriggered = true;
                }
            }
            else
            {
                signalTriggered = false;
            }

            return new SignalResult()
            {
                ByStopLoss = false,
                SignalTriggered = signalTriggered
            };
        }

        public override SignalResult SellSignal(IList<DataSample> samples, DataSample sample, Position position)
        {
            var levels = LevelsDeterminator.DeterminateLevels(samples).Where(lv => lv.TouchCount > 1);

            var indexOfSample = samples.IndexOf(sample);
            if (samples.Count() <= 3)
                return new SignalResult()
                {
                    ByStopLoss = false,
                    SignalTriggered = false
                };

            var previousSample = samples[indexOfSample - 1];
            var prevPrevSample = samples[indexOfSample - 2];
            var signalTriggered = false;
            if (samples.Count() > 2
                && (sample.CandleColor == Common.CandleColor.Red)
                && (prevPrevSample.CandleColor == Common.CandleColor.Green)
                //&& (previousSample.CandleColor == Common.CandleColor.Green || previousSample.CandleColor == Common.CandleColor.Grey)
                //&& sample.Candle.Volume * Math.Abs(sample.Candle.Close - sample.Candle.Open) > previousSample.Candle.Volume * Math.Abs(previousSample.Candle.Close - previousSample.Candle.Open)
                && IsLastAreaVolumeMoreThanPrevious(samples, previousSample, 50, 10, 1)
                && IsSampleWithLongTail(previousSample, PositionDirection.Short, 0.3m))
            {
                if (position != null)
                {
                    if (Math.Abs(sample.Candle.Close - position.OpenPrice) > Math.Abs(position.OpenPrice - position.StopLossPrice) * MinimumProfitMultiplicator)
                    {
                        signalTriggered = true;
                    }
                }
                else
                {
                    signalTriggered = true;
                }
            }
            else
            {
                signalTriggered = false;
            }

            return new SignalResult()
            {
                ByStopLoss = false,
                SignalTriggered = signalTriggered
            };
        }

        private bool IsSampleWithLongTail(DataSample sample, PositionDirection direction, decimal deltaPercentage)
        {
            var result = false;
            if (direction == PositionDirection.Long && sample.CandleColor == Common.CandleColor.Green)
            {
                result = Math.Abs(sample.Candle.Open - sample.Candle.Low) / Math.Abs(sample.Candle.Low - sample.Candle.High) > deltaPercentage;
            }
            else if (direction == PositionDirection.Long && sample.CandleColor == Common.CandleColor.Red)
            {
                result = Math.Abs(sample.Candle.Close - sample.Candle.Low) / Math.Abs(sample.Candle.Low - sample.Candle.High) > deltaPercentage;
            }
            else if (direction == PositionDirection.Short && sample.CandleColor == Common.CandleColor.Green)
            {
                result = Math.Abs(sample.Candle.Close - sample.Candle.High) / Math.Abs(sample.Candle.Low - sample.Candle.High) > deltaPercentage;
            }
            else if (direction == PositionDirection.Short && sample.CandleColor == Common.CandleColor.Red)
            {
                result = Math.Abs(sample.Candle.Open - sample.Candle.High) / Math.Abs(sample.Candle.Low - sample.Candle.High) > deltaPercentage;
            }
            else
            {
                result = false;
            }
            return result;
        }
    }
}
