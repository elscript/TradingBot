using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingBot.Core
{
    public class ExtremumArea
    {
        public DataSample LastLocalMinimum;
        public DataSample LastLocalMaximum;
        public DataSample CurrentExtremum;
        public ExtremumType Type;

        public static IList<ExtremumArea> FindLocalMinimums(IList<DataSample> samples)
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

        public static IList<ExtremumArea> FindLocalMaximums(IList<DataSample> samples)
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

        public static IList<ExtremumArea> FillMinimumsArea(IList<ExtremumArea> minimums, IList<ExtremumArea> maximums)
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

        public static IList<ExtremumArea> FillMaximumsArea(IList<ExtremumArea> maximums, IList<ExtremumArea> minimums)
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

        public static ExtremumArea GetLastMinimumBeforeSample(IList<ExtremumArea> extremums, DataSample sample)
        {
            return extremums.LastOrDefault(m => m.CurrentExtremum.Candle.Timestamp < sample.Candle.Timestamp);
        }

        public static ExtremumArea GetLastMaximumBeforeSample(IList<ExtremumArea> extremums, DataSample sample)
        {
            return extremums.LastOrDefault(m => m.CurrentExtremum.Candle.Timestamp > sample.Candle.Timestamp);
        }

        public static ExtremumArea GetLastExtremumForPriceBeforeSample(IList<ExtremumArea> extremums, DataSample sample, decimal price, PositionDirection direction)
        {
            return extremums.LastOrDefault(m =>
                m.CurrentExtremum.Candle.Timestamp < sample.Candle.Timestamp && direction == PositionDirection.Long ? m.CurrentExtremum.Candle.High < price : m.CurrentExtremum.Candle.Low < price);
        }
    }
}
