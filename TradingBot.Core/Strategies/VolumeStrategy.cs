using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingBot.Core.Strategies
{
    public class VolumeStrategy : IStrategy
    {
        public bool AllowLong { get; }
        public bool AllowShort { get; }
        public decimal MaxLoosePercentage { get; }

        private readonly int _stopLossAdditionalValuePercentage;
        private readonly int _minimumProfitMultiplicator;

        public VolumeStrategy(int stopLossAdditionalValuePercentage, int minimumProfitMultiplicator, decimal maxLoosePercentage, bool allowLong, bool allowShort)
        {
            _stopLossAdditionalValuePercentage = stopLossAdditionalValuePercentage;
            _minimumProfitMultiplicator = minimumProfitMultiplicator;
            MaxLoosePercentage = maxLoosePercentage;
            AllowLong = allowLong;
            AllowShort = allowShort;
        }

        public SignalResult BuySignal(IList<DataSample> samples, DataSample sample, Position position)
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
                    if (Math.Abs(sample.Candle.Close - position.OpenPrice) > Math.Abs(position.OpenPrice - position.StopLossPrice) * _minimumProfitMultiplicator)
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

        public SignalResult SellSignal(IList<DataSample> samples, DataSample sample, Position position)
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
                    if (Math.Abs(sample.Candle.Close - position.OpenPrice) > Math.Abs(position.OpenPrice - position.StopLossPrice) * _minimumProfitMultiplicator)
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

        public decimal GetStopLossPrice(IList<DataSample> samples, DataSample sample, Position position)
        {
            var maximums = ExtremumArea.FindLocalMaximums(samples);
            var minimums = ExtremumArea.FindLocalMinimums(samples);
            ExtremumArea lastExtremum;
            decimal stopLossPrice = 0;

            if (position.Direction == PositionDirection.Long)
            {
                lastExtremum = ExtremumArea.GetLastMinimumBeforeSample(minimums, sample);
                if (lastExtremum == null)
                {
                    stopLossPrice = sample.Candle.Low - (position.OpenPrice - sample.Candle.Low) * _stopLossAdditionalValuePercentage / 100;
                }
                else
                {
                    stopLossPrice = lastExtremum.CurrentExtremum.Candle.Low - (position.OpenPrice - lastExtremum.CurrentExtremum.Candle.Low) * _stopLossAdditionalValuePercentage / 100;
                }
            }
            else if (position.Direction == PositionDirection.Short)
            {
                lastExtremum = ExtremumArea.GetLastMaximumBeforeSample(maximums, sample);
                if (lastExtremum == null)
                {
                    stopLossPrice = sample.Candle.High + (sample.Candle.High - position.OpenPrice) * _stopLossAdditionalValuePercentage / 100;
                }
                else
                {
                    stopLossPrice = lastExtremum.CurrentExtremum.Candle.High + (lastExtremum.CurrentExtremum.Candle.High - position.OpenPrice) * _stopLossAdditionalValuePercentage / 100;
                }
            }
            return stopLossPrice;
        }

        public decimal GetAmountForPosition(decimal stopLossPrice, decimal openPrice, decimal currentBalance, int maximumLeverage)
        {
            var delta = Math.Abs(openPrice - stopLossPrice);
            if (delta == 0)
            {
                delta = 1; // TODO хак, надо разобраться почему иногда ноль
            }

            var multiplicator = (MaxLoosePercentage / (delta / openPrice));
            if (multiplicator > maximumLeverage)
                multiplicator = maximumLeverage;
            return multiplicator * currentBalance;
        }

        private bool IsLastAreaVolumeMoreThanPrevious(IList<DataSample> samples, DataSample sample, decimal percentage, int samplesPerPrevArea, int samplesPerLastArea)
        {
            var indexOfSample = samples.IndexOf(sample);
            var indexOfFirstSampleInLastArea = indexOfSample - samplesPerLastArea;
            if (indexOfFirstSampleInLastArea < 0)
                return false;

            IList<DataSample> targetLastSamples = new List<DataSample>();
            for (int i = indexOfFirstSampleInLastArea; i <= indexOfSample; i++)
            {
                targetLastSamples.Add(samples[i]);
            }

            var indexOfFirstSampleInPrevArea = indexOfFirstSampleInLastArea - 1 - samplesPerPrevArea;
            if (indexOfFirstSampleInPrevArea < 0)
                return false;

            IList<DataSample> targetPrevSamples = new List<DataSample>();
            for (int i = indexOfFirstSampleInPrevArea; i < indexOfFirstSampleInLastArea; i++)
            {
                targetPrevSamples.Add(samples[i]);
            }

            return (targetLastSamples.Average(s => s.Candle.Volume) / (1 + percentage / 100)) > targetPrevSamples.Average(s => s.Candle.Volume);
        }
    }
}
