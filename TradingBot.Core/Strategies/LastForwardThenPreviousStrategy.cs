﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingBot.Core
{
    /// <summary>
    /// Стратегия, основанная на приниципе "каждый следующий максимум выше предыдущего и каждый следующий минимум ниже предыдущего"
    /// </summary>
    public class LastForwardThenPreviousStrategy : IStrategy
    {
        public bool AllowLong { get; }
        public bool AllowShort { get; }
        public decimal MaxLoosePercentage { get; }

        public LastForwardThenPreviousStrategy(decimal maxLoosePercentage, bool allowLong, bool allowShort)
        {
            AllowLong = allowLong;
            AllowShort = allowShort;
            MaxLoosePercentage = maxLoosePercentage;
        }

        public SignalResult BuySignal(IList<DataSample> samples, DataSample sample, Position position)
        {
            var localMinimums = ExtremumArea.FindLocalMinimums(samples);
            var localMaximums = ExtremumArea.FindLocalMaximums(samples);
            localMinimums = ExtremumArea.FillMinimumsArea(localMinimums, localMaximums);
            localMaximums = ExtremumArea.FillMaximumsArea(localMaximums, localMinimums);

            // Проверяем, не сработал ли кастомный стоплосс с учетом максимального процента потерь
            if (position != null)
            {
                var lastExtremumForPrice = ExtremumArea.GetLastExtremumForPriceBeforeSample(localMaximums, sample, position.OpenPrice, PositionDirection.Short);
                if (lastExtremumForPrice != null)
                {
                    var stopLossExtremumPrice = lastExtremumForPrice.CurrentExtremum.Candle.Close;

                    if (sample.Candle.Close > stopLossExtremumPrice + stopLossExtremumPrice * (MaxLoosePercentage / 100))
                    {
                        //Console.WriteLine($"##Buy Signal (stoploss) triggered for sample.Candle.Timestamp({sample.Candle.Timestamp}) because sample.Candle.Close({sample.Candle.Close}) > stopLossExtremumPrice({stopLossExtremumPrice}) + stopLossExtremumPrice({stopLossExtremumPrice}) * (MaxLoosePercentage({MaxLoosePercentage}) / 100)");
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
            var lastMaximumBeforeSample = ExtremumArea.GetLastMaximumBeforeAndWithSample(localMaximums, sample);
            var lastMinimumBeforeSample = ExtremumArea.GetLastMinimumBeforeAndWithSample(localMinimums, sample);

            if (lastMaximumBeforeSample != null && lastMinimumBeforeSample != null)
            {
                lastLocalMinimumPassed = lastMaximumBeforeSample.CurrentExtremum.Candle.Timestamp >
                    lastMinimumBeforeSample.CurrentExtremum.Candle.Timestamp;

                //lastLocalMaximumPassed = sample.Candle.Close > lastMaximumBeforeSample.CurrentExtremum.Candle.High;
                lastLocalMaximumPassed = sample.Candle.Close > lastMaximumBeforeSample.CurrentExtremum.Candle.High;
            }

            //if (lastLocalMinimumPassed && lastLocalMaximumPassed)
                //Console.WriteLine($"##Buy Signal triggered for sample.Candle.Timestamp({sample.Candle.Timestamp}) because lastLocalMinimumPassed(minimum.Low is {lastMinimumBeforeSample.CurrentExtremum.Candle.Low} maximum high is {lastMaximumBeforeSample.CurrentExtremum.Candle.High}) and lastLocalMaximumPassed(sample.Candle.Close({sample.Candle.Close}) > lastMaximumBeforeSample.CurrentExtremum.Candle.Close({lastMaximumBeforeSample.CurrentExtremum.Candle.Close}))");

            return new SignalResult()
            {
                ByStopLoss = false,
                SignalTriggered = lastLocalMinimumPassed && lastLocalMaximumPassed
            };
        }

        public SignalResult SellSignal(IList<DataSample> samples, DataSample sample, Position position)
        {
            var localMinimums = ExtremumArea.FindLocalMinimums(samples);
            var localMaximums = ExtremumArea.FindLocalMaximums(samples);
            localMinimums = ExtremumArea.FillMinimumsArea(localMinimums, localMaximums);
            localMaximums = ExtremumArea.FillMaximumsArea(localMaximums, localMinimums);

            // Проверяем, не сработал ли кастомный стоплосс с учетом максимального процента потерь
            if (position != null)
            {
                var lastExtremumForPrice = ExtremumArea.GetLastExtremumForPriceBeforeSample(localMinimums, sample, position.OpenPrice, PositionDirection.Long);
                if (lastExtremumForPrice != null)
                {
                    var stopLossExtremumPrice = lastExtremumForPrice.CurrentExtremum.Candle.Close;

                    if (sample.Candle.Close < stopLossExtremumPrice - stopLossExtremumPrice * (MaxLoosePercentage / 100))
                    {
                        //Console.WriteLine($"##Sell Signal triggered because sample.Candle.Close({sample.Candle.Close}) < stopLossExtremumPrice({stopLossExtremumPrice}) - stopLossExtremumPrice({stopLossExtremumPrice}) * (MaxLoosePercentage({MaxLoosePercentage}) / 100)");
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
            var lastMaximumBeforeSample = ExtremumArea.GetLastMaximumBeforeAndWithSample(localMaximums, sample);
            var lastMinimumBeforeSample = ExtremumArea.GetLastMinimumBeforeAndWithSample(localMinimums, sample);

            if (lastMaximumBeforeSample != null && lastMinimumBeforeSample != null)
            {
                lastLocalMaximumPassed = lastMinimumBeforeSample.CurrentExtremum.Candle.Timestamp >
                                         lastMaximumBeforeSample.CurrentExtremum.Candle.Timestamp;

                //lastLocalMinimumPassed = sample.Candle.Close < lastMinimumBeforeSample.CurrentExtremum.Candle.Low;
                lastLocalMinimumPassed = sample.Candle.Close < lastMinimumBeforeSample.CurrentExtremum.Candle.Low;
            }

            //if (lastLocalMaximumPassed && lastLocalMinimumPassed)
                //Console.WriteLine($"##Sell Signal (stoploss) triggered for sample.Candle.Timestamp({sample.Candle.Timestamp}) because lastLocalMaximumPassed(minimum.Low is {lastMaximumBeforeSample.CurrentExtremum.Candle.High} maximum high is {lastMinimumBeforeSample.CurrentExtremum.Candle.Low}) and lastLocalMinimumPassed(sample.Candle.Close({sample.Candle.Close}) < lastMinimumBeforeSample.CurrentExtremum.Candle.Close({lastMinimumBeforeSample.CurrentExtremum.Candle.Close}))");

            return new SignalResult()
            {
                ByStopLoss = false,
                SignalTriggered = lastLocalMaximumPassed && lastLocalMinimumPassed
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
                lastExtremum = ExtremumArea.GetLastMinimumBeforeAndWithSample(minimums, sample);
                stopLossPrice = lastExtremum.CurrentExtremum.Candle.Low - (position.OpenPrice - lastExtremum.CurrentExtremum.Candle.Low) * 10; //TODO убрать хардкод
            }
            else if (position.Direction == PositionDirection.Short)
            {
                lastExtremum = ExtremumArea.GetLastMaximumBeforeAndWithSample(maximums, sample);
                stopLossPrice = lastExtremum.CurrentExtremum.Candle.High + (lastExtremum.CurrentExtremum.Candle.High - position.OpenPrice) * 10; //TODO убрать хардкод
            }
            return stopLossPrice;
        }

        public decimal GetAmountForPosition(decimal stopLossPrice, decimal openPrice, decimal currentBalance, int maximumLeverage)
        {
            var multiplicator = (maximumLeverage / ((Math.Abs(openPrice - stopLossPrice) / openPrice) * 100));
            if (multiplicator > maximumLeverage)
                multiplicator = maximumLeverage;
            return multiplicator * currentBalance;
        }
    }
}
