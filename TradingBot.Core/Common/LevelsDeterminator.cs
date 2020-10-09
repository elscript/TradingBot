using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingBot.Core.Common
{
    public class LevelsDeterminator
    {
        public static IList<Level> DeterminateLevels(IList<DataSample> samples)
        {
            var result = new List<Level>();

            var localMinimums = ExtremumArea.FindLocalMinimums(samples);
            var localMaximums = ExtremumArea.FindLocalMaximums(samples);
            localMinimums = ExtremumArea.FillMinimumsArea(localMinimums, localMaximums);
            localMaximums = ExtremumArea.FillMaximumsArea(localMaximums, localMinimums);

            foreach (var extremum in localMinimums)
            {
                foreach (var nextExtremum in localMinimums)
                {
                    var isNearByLow = IsNear(nextExtremum.CurrentExtremum.Candle.Low,
                        extremum.CurrentExtremum.Candle.Low);
                    var isNearByOpen = IsNear(nextExtremum.CurrentExtremum.Candle.Open,
                        extremum.CurrentExtremum.Candle.Open);
                    var isNearByClose = IsNear(nextExtremum.CurrentExtremum.Candle.Close,
                        extremum.CurrentExtremum.Candle.Close);

                    if (nextExtremum.CurrentExtremum.CandleColor == CandleColor.Green 
                        && (isNearByLow || isNearByOpen))
                    {
                        var levelAlreadyExistingInCollection = result.Where(e =>
                            (isNearByLow && IsNear(e.Price, nextExtremum.CurrentExtremum.Candle.Low))
                            || (isNearByOpen && IsNear(e.Price, nextExtremum.CurrentExtremum.Candle.Open)));

                        if (levelAlreadyExistingInCollection.Any())
                        {
                            var level = levelAlreadyExistingInCollection.Single(lvl =>
                                lvl.TouchCount == levelAlreadyExistingInCollection.Max(lv => lv.TouchCount));
                            if (level != null) 
                                level.TouchCount++;
                        }
                        else
                        {
                            result.Add(new Level()
                            {
                                Price = isNearByLow ? nextExtremum.CurrentExtremum.Candle.Low : isNearByOpen ? nextExtremum.CurrentExtremum.Candle.Open : 0,
                                TouchCount = 1
                            });
                        }
                    }
                    else if (nextExtremum.CurrentExtremum.CandleColor == CandleColor.Red
                             && (isNearByLow || isNearByClose))
                    {
                        var levelAlreadyExistingInCollection = result.Where(e =>
                            (isNearByLow && IsNear(e.Price, nextExtremum.CurrentExtremum.Candle.Low))
                            || (isNearByClose && IsNear(e.Price, nextExtremum.CurrentExtremum.Candle.Close)));

                        if (levelAlreadyExistingInCollection.Any())
                        {
                            var level = levelAlreadyExistingInCollection.Single(lvl =>
                                lvl.TouchCount == levelAlreadyExistingInCollection.Max(lv => lv.TouchCount));
                            if (level != null)
                                level.TouchCount++;
                        }
                        else
                        {
                            result.Add(new Level()
                            {
                                Price = isNearByLow ? nextExtremum.CurrentExtremum.Candle.Low : isNearByClose ? nextExtremum.CurrentExtremum.Candle.Close : 0,
                                TouchCount = 1
                            });
                        }
                    }
                }
            }

            foreach (var extremum in localMaximums)
            {
                foreach (var nextExtremum in localMaximums)
                {
                    var isNearByHigh = IsNear(nextExtremum.CurrentExtremum.Candle.High,
                        extremum.CurrentExtremum.Candle.High);
                    var isNearByOpen = IsNear(nextExtremum.CurrentExtremum.Candle.Open,
                        extremum.CurrentExtremum.Candle.Open);
                    var isNearByClose = IsNear(nextExtremum.CurrentExtremum.Candle.Close,
                        extremum.CurrentExtremum.Candle.Close);

                    if (nextExtremum.CurrentExtremum.CandleColor == CandleColor.Green
                        && (isNearByHigh || isNearByClose))
                    {
                        var levelAlreadyExistingInCollection = result.Where(e =>
                            (isNearByHigh && IsNear(e.Price, nextExtremum.CurrentExtremum.Candle.High))
                            || (isNearByClose && IsNear(e.Price, nextExtremum.CurrentExtremum.Candle.Close)));

                        if (levelAlreadyExistingInCollection.Any())
                        {
                            var level = levelAlreadyExistingInCollection.Single(lvl =>
                                lvl.TouchCount == levelAlreadyExistingInCollection.Max(lv => lv.TouchCount));
                            if (level != null)
                                level.TouchCount++;
                        }
                        else
                        {
                            result.Add(new Level()
                            {
                                Price = isNearByHigh ? nextExtremum.CurrentExtremum.Candle.High : isNearByClose ? nextExtremum.CurrentExtremum.Candle.Close : 0,
                                TouchCount = 1
                            });
                        }
                    }
                    else if (nextExtremum.CurrentExtremum.CandleColor == CandleColor.Red
                             && (isNearByHigh || isNearByOpen))
                    {
                        var levelAlreadyExistingInCollection = result.Where(e =>
                            (isNearByHigh && IsNear(e.Price, nextExtremum.CurrentExtremum.Candle.High))
                            || (isNearByOpen && IsNear(e.Price, nextExtremum.CurrentExtremum.Candle.Open)));

                        if (levelAlreadyExistingInCollection.Any())
                        {
                            var level = levelAlreadyExistingInCollection.Single(lvl =>
                                lvl.TouchCount == levelAlreadyExistingInCollection.Max(lv => lv.TouchCount));
                            if (level != null)
                                level.TouchCount++;
                        }
                        else
                        {
                            result.Add(new Level()
                            {
                                Price = isNearByHigh ? nextExtremum.CurrentExtremum.Candle.High : isNearByOpen ? nextExtremum.CurrentExtremum.Candle.Open : 0,
                                TouchCount = 1
                            });
                        }
                    }
                }
            }

            return result;
        }

        private static bool IsNear(decimal firstPrice, decimal secondPrice)
        {
            var result = firstPrice == secondPrice;
            if (!result)
            {
                result = Math.Abs(firstPrice - secondPrice) <= (((firstPrice + secondPrice) / 2) / 100) / ((firstPrice + secondPrice) / 2);
            }

            return result;
        }
    }
}
