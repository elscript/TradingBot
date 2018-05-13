using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net.Objects;

namespace TestingConsole
{
    /// <summary>
    /// Экспоненциальная скользящая средняя
    /// </summary>
    public class EMA
    {
        /// <summary>
        /// Период
        /// </summary>
        public int Period { get; }

        /// <summary>
        /// Тип скользящей
        /// </summary>
        public EMAKind Kind { get; }

        /// <summary>
        /// Значение
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Экспоненциальная скользящая средняя
        /// </summary>
        /// <param name="period">Период</param>
        /// <param name="candles">Набор свечей для расчета</param>
        /// <param name="candle">Свеча, для которой рассчитываем EMA</param>
        /// <param name="seedEMA">Флаг, указывающий, что высчитываем первоначальный EMA</param>
        /// <param name="kind">Тип EMA</param>
        public EMA(int period, IList<DataSample> samples, DataSample sample, EMAKind kind)
        {
            Period = period;
            Kind = kind;
            Value = Calculate(samples, sample);
        }

        private double Calculate(IList<DataSample> samples, DataSample sample)
        {
            var result = 0d;
            double p1 = Period + 1;
            double EMAMultiplier = Convert.ToDouble(2 / p1);
            var indexOfCandle = samples.IndexOf(sample);

            if (indexOfCandle >= Period - 1)
            {
                EMA lastEMA = null;
                switch (Kind)
                {
                    case EMAKind.Short:
                        lastEMA = samples[indexOfCandle - 1].EMAshort;
                        break;
                    case EMAKind.Long:
                        lastEMA = samples[indexOfCandle - 1].EMAlong;
                        break;
                    case EMAKind.Smoothing:
                        lastEMA = ((MACDIndicator) samples[indexOfCandle - 1].Indicators["macd"]).EMAForMACD;
                        break;
                }

                if (indexOfCandle == Period - 1)
                {
                    // This is our seed EMA, using SMA of EMA1 Period for EMA 1
                    result = samples.Where(a => a.Candle.Timestamp <= sample.Candle.Timestamp).OrderByDescending(a => a.Candle.Timestamp)
                        .Take(Period).Average(a => Convert.ToDouble(a.Candle.Close));
                }
                else
                {
                    result = (Convert.ToDouble(sample.Candle.Close) - lastEMA.Value) * EMAMultiplier + lastEMA.Value;
                }
            }

            return result;
        }
    }
}
