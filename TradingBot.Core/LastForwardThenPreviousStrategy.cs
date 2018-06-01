using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingBot.Core
{
    /// <summary>
    /// Стратегия, основанная на приниципе "каждый следующий максимум выше предыдущего и каждый следующий минимум выше предыдущего"
    /// </summary>
    public class LastForwardThenPreviousStrategy : IStrategy
    {
        public LastForwardThenPreviousStrategy(decimal maxLoosePercentage, bool allowLong, bool allowShort)
        {
            AllowLong = allowLong;
            AllowShort = allowShort;
            MaxLoosePercentage = maxLoosePercentage;
        }

        public SignalResult BuySignal(IList<DataSample> samples, DataSample sample, decimal? lastSellPrice)
        {
            var localMinimums = FindLocalMinimums(samples);
            var localMaximums = FindLocalMaximums(samples);
            localMinimums = FillMinimumsArea(localMinimums, localMaximums);
            localMaximums = FillMaximumsArea(localMaximums, localMinimums);

            // Проверяем, не сработал ли кастомный стоплосс с учетом максимального процента потерь
            if (lastSellPrice != null)
            {
                var lastExtremumForPrice = GetLastExtremumForPriceBeforeSample(localMaximums, sample, lastSellPrice.Value);
                if (lastExtremumForPrice != null)
                {
                    var stopLossExtremumPrice = lastExtremumForPrice.CurrentExtremum.Candle.High;

                    if (sample.Candle.Close > stopLossExtremumPrice + stopLossExtremumPrice * (MaxLoosePercentage / 100))
                    {    return new SignalResult()
                        {
                            SignalTriggered = true,
                            ByStopLoss = true
                        };

                    }
                }
            }

            var lastLocalMinimumPassed = false;
            var lastLocalMaximumPassed = false;
            var lastMaximumBeforeSample = GetLastExtremumBeforeSample(localMaximums, sample);
            var lastMinimumBeforeSample = GetLastExtremumBeforeSample(localMinimums, sample);

            if (lastMaximumBeforeSample != null && lastMinimumBeforeSample != null)
            {
                lastLocalMinimumPassed = lastMaximumBeforeSample.CurrentExtremum.Candle.Timestamp >
                    lastMinimumBeforeSample.CurrentExtremum.Candle.Timestamp;

                //lastLocalMaximumPassed = sample.Candle.Close > lastMaximumBeforeSample.CurrentExtremum.Candle.High;
                lastLocalMaximumPassed = sample.Candle.Close > lastMaximumBeforeSample.CurrentExtremum.Candle.Close;
            }

            return new SignalResult()
            {
                ByStopLoss = false,
                SignalTriggered = lastLocalMinimumPassed && lastLocalMaximumPassed
            };
        }

        public SignalResult SellSignal(IList<DataSample> samples, DataSample sample, decimal? lastBuyPrice)
        {
            var localMinimums = FindLocalMinimums(samples);
            var localMaximums = FindLocalMaximums(samples);
            localMinimums = FillMinimumsArea(localMinimums, localMaximums);
            localMaximums = FillMaximumsArea(localMaximums, localMinimums);

            // Проверяем, не сработал ли кастомный стоплосс с учетом максимального процента потерь
            if (lastBuyPrice != null)
            {
                var lastExtremumForPrice = GetLastExtremumForPriceBeforeSample(localMinimums, sample, lastBuyPrice.Value);
                if (lastExtremumForPrice != null)
                {
                    var stopLossExtremumPrice = lastExtremumForPrice.CurrentExtremum.Candle.Low;

                    if (sample.Candle.Close < stopLossExtremumPrice - stopLossExtremumPrice * (MaxLoosePercentage / 100))
                    {
                        return new SignalResult()
                        {
                            SignalTriggered = true,
                            ByStopLoss = true
                        };
                    }
                }
            }

            var lastLocalMinimumPassed = false;
            var lastLocalMaximumPassed = false;
            var lastMaximumBeforeSample = GetLastExtremumBeforeSample(localMaximums, sample);
            var lastMinimumBeforeSample = GetLastExtremumBeforeSample(localMinimums, sample);

            if (lastMaximumBeforeSample != null && lastMinimumBeforeSample != null)
            {
                lastLocalMaximumPassed = lastMinimumBeforeSample.CurrentExtremum.Candle.Timestamp >
                                         lastMaximumBeforeSample.CurrentExtremum.Candle.Timestamp;

                //lastLocalMinimumPassed = sample.Candle.Close < lastMinimumBeforeSample.CurrentExtremum.Candle.Low;
                lastLocalMinimumPassed = sample.Candle.Close < lastMinimumBeforeSample.CurrentExtremum.Candle.Close;
            }

            return new SignalResult()
            {
                ByStopLoss = false,
                SignalTriggered = lastLocalMaximumPassed && lastLocalMinimumPassed
            };
        }

        private IList<ExtremumArea> FindLocalMinimums(IList<DataSample> samples)
        {
            var result = new List<ExtremumArea>();
            for (int i = 1; i < samples.Count - 1; i++)
            {
                if (samples[i].Candle.Low < samples[i - 1].Candle.Low &&
                    samples[i].Candle.Low < samples[i + 1].Candle.Low)
                {
                    result.Add(
                        new ExtremumArea()
                        {
                            CurrentExtremum = samples[i],
                            Type = ExtremumType.Minimum
                        }
                    );
                }
            }
            return result;
        }

        private IList<ExtremumArea> FindLocalMaximums(IList<DataSample> samples)
        {
            var result = new List<ExtremumArea>();
            for (int i = 1; i < samples.Count - 1; i++)
            {
                if (samples[i].Candle.High > samples[i - 1].Candle.High &&
                    samples[i].Candle.High > samples[i + 1].Candle.High)
                {
                    result.Add(
                        new ExtremumArea()
                        {
                            CurrentExtremum = samples[i],
                            Type = ExtremumType.Maximum
                        }
                    );
                }
            }
            return result;
        }

        private IList<ExtremumArea> FillMinimumsArea(IList<ExtremumArea> minimums, IList<ExtremumArea> maximums)
        {
            // Для каждого минимума заполняем предыдущий минимум и максимум
            for (int i = 1; i < minimums.Count; i++)
            {
                minimums[i].LastLocalMinimum = minimums[i - 1].CurrentExtremum;
                minimums[i].LastLocalMaximum = maximums.LastOrDefault(m =>
                    m.CurrentExtremum.Candle.Timestamp < minimums[i].CurrentExtremum.Candle.Timestamp)
                    ?.CurrentExtremum;
            }
            return minimums;
        }

        private IList<ExtremumArea> FillMaximumsArea(IList<ExtremumArea> maximums, IList<ExtremumArea> minimums)
        {
            // Для каждого максимума заполняем предыдущий максимум и минимум
            for (int i = 1; i < maximums.Count; i++)
            {
                maximums[i].LastLocalMaximum = maximums[i - 1].CurrentExtremum;
                maximums[i].LastLocalMinimum = minimums.LastOrDefault(m =>
                    m.CurrentExtremum.Candle.Timestamp < maximums[i].CurrentExtremum.Candle.Timestamp)
                    ?.CurrentExtremum;
            }
            return maximums;
        }

        private ExtremumArea GetLastExtremumBeforeSample(IList<ExtremumArea> extremums, DataSample sample)
        {
            return extremums.LastOrDefault(m => m.CurrentExtremum.Candle.Timestamp < sample.Candle.Timestamp);
        }

        private ExtremumArea GetLastExtremumForPriceBeforeSample(IList<ExtremumArea> extremums, DataSample sample, decimal price)
        {
            return extremums.LastOrDefault(m =>
                m.CurrentExtremum.Candle.Timestamp < sample.Candle.Timestamp && m.CurrentExtremum.Candle.Low < price);
        }

        public bool AllowLong { get; }
        public bool AllowShort { get; }
        public decimal MaxLoosePercentage { get; }
    }
}
