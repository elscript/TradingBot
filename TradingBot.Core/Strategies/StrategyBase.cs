using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingBot.Core.Strategies
{
    public abstract class StrategyBase : IStrategy
    {
        public bool AllowLong { get; }
        public bool AllowShort { get; }
        public decimal MaxLoosePercentage { get; }
        public int StopLossAdditionalValuePercentage { get; }
        public int MinimumProfitMultiplicator { get; }

        public StrategyBase(int stopLossAdditionalValuePercentage, int minimumProfitMultiplicator, decimal maxLoosePercentage, bool allowLong, bool allowShort)
        {
            StopLossAdditionalValuePercentage = stopLossAdditionalValuePercentage;
            MinimumProfitMultiplicator = minimumProfitMultiplicator;
            MaxLoosePercentage = maxLoosePercentage;
            AllowLong = allowLong;
            AllowShort = allowShort;
        }

        public abstract SignalResult BuySignal(IList<DataSample> samples, DataSample sample, Position position);
        public abstract SignalResult SellSignal(IList<DataSample> samples, DataSample sample, Position position);

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

        public decimal GetStopLossPrice(IList<DataSample> samples, DataSample sample, Position position)
        {
            var maximums = ExtremumArea.FindLocalMaximums(samples);
            var minimums = ExtremumArea.FindLocalMinimums(samples);
            ExtremumArea lastExtremum;
            decimal stopLossPrice = 0;

            if (position.Direction == PositionDirection.Long)
            {
                lastExtremum = ExtremumArea.GetLastMinimumBeforeAndWithSample(minimums, sample);
                if (lastExtremum == null)
                {
                    stopLossPrice = sample.Candle.Low - (position.OpenPrice - sample.Candle.Low) * StopLossAdditionalValuePercentage / 100;
                }
                else
                {
                    stopLossPrice = lastExtremum.CurrentExtremum.Candle.Low - (position.OpenPrice - lastExtremum.CurrentExtremum.Candle.Low) * StopLossAdditionalValuePercentage / 100;
                }
            }
            else if (position.Direction == PositionDirection.Short)
            {
                lastExtremum = ExtremumArea.GetLastMaximumBeforeAndWithSample(maximums, sample);
                if (lastExtremum == null)
                {
                    stopLossPrice = sample.Candle.High + (sample.Candle.High - position.OpenPrice) * StopLossAdditionalValuePercentage / 100;
                }
                else
                {
                    stopLossPrice = lastExtremum.CurrentExtremum.Candle.High + (lastExtremum.CurrentExtremum.Candle.High - position.OpenPrice) * StopLossAdditionalValuePercentage / 100;
                }
            }
            return stopLossPrice;
        }

        protected bool IsLastAreaVolumeMoreThanPrevious(IList<DataSample> samples, DataSample sample, decimal percentage, int samplesPerPrevArea, int samplesPerLastArea)
        {
            var indexOfSample = samples.IndexOf(sample);
            var indexOfFirstSampleInLastArea = indexOfSample - samplesPerLastArea + 1;
            if (indexOfFirstSampleInLastArea < 0)
                return false;

            IList<DataSample> targetLastSamples = new List<DataSample>();
            for (int i = indexOfFirstSampleInLastArea; i <= indexOfSample; i++)
            {
                targetLastSamples.Add(samples[i]);
            }

            var indexOfFirstSampleInPrevArea = indexOfFirstSampleInLastArea - samplesPerPrevArea;
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
