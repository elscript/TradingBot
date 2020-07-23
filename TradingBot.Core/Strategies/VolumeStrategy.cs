using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingBot.Core.Strategies
{
    public class VolumeStrategy : StrategyBase
    {
        public VolumeStrategy(int stopLossAdditionalValuePercentage, int minimumProfitMultiplicator, decimal maxLoosePercentage, bool allowLong, bool allowShort) : base (stopLossAdditionalValuePercentage, minimumProfitMultiplicator, maxLoosePercentage, allowLong, allowShort)
        {
        }

        public override SignalResult BuySignal(IList<DataSample> samples, DataSample sample, Position position)
        {
            var indexOfSample = samples.IndexOf(sample);
            if (samples.Count() <= 1)
                return new SignalResult()
                {
                    ByStopLoss = false,
                    SignalTriggered = false
                };

            var previousSample = samples[indexOfSample - 1];
            var signalTriggered = false;
            if (samples.Count() > 1
                && sample.CandleColor == Common.CandleColor.Green
                && (previousSample.CandleColor == Common.CandleColor.Red || previousSample.CandleColor == Common.CandleColor.Grey)
                && sample.Candle.Volume * Math.Abs(sample.Candle.Close - sample.Candle.Open) > previousSample.Candle.Volume * Math.Abs(previousSample.Candle.Close - previousSample.Candle.Open)
                && IsLastAreaVolumeMoreThanPrevious(samples, sample, 50, 40, 3))
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
            var indexOfSample = samples.IndexOf(sample);
            if (samples.Count() <= 1)
                return new SignalResult()
                {
                    ByStopLoss = false,
                    SignalTriggered = false
                };

            var previousSample = samples[indexOfSample - 1];
            var signalTriggered = false;
            if (samples.Count() > 1
                && (sample.CandleColor == Common.CandleColor.Red)
                && (previousSample.CandleColor == Common.CandleColor.Green || previousSample.CandleColor == Common.CandleColor.Grey)
                && sample.Candle.Volume * Math.Abs(sample.Candle.Close - sample.Candle.Open) > previousSample.Candle.Volume * Math.Abs(previousSample.Candle.Close - previousSample.Candle.Open)
                && IsLastAreaVolumeMoreThanPrevious(samples, sample, 50, 40, 3))
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
    }
}
