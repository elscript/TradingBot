using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net.Objects;

namespace TestingConsole
{
    /// <summary>
    /// Индикатор MACD
    /// </summary>
    class MACDIndicator : IIndicator
    {
        /// <summary>
        /// Индикатор MACD
        /// </summary>
        /// <param name="sample">Рассчитываемый сэмпл</param>
        /// <param name="samples">Все сэмплы</param>
        /// <param name="macdEMAPeriod">Период EMA для сглаживания индикатора</param>
        public MACDIndicator(DataSample sample, IList<DataSample> samples, int macdEMAPeriod)
        {
            Sample = sample;
            Samples = samples;
            MACDEmaPeriod = macdEMAPeriod;
            //EMAForMACD = new EMA(macdEMAPeriod, Samples, sample, EMAKind.Smoothing);
        }

        /// <summary>
        /// Экспоненциальная скользящая средняя малого периода для сглаживания MACD
        /// </summary>
        public EMA EMAForMACD;

        /// <summary>
        /// Рассчитываемый сэмпл
        /// </summary>
        public DataSample Sample { get; }

        /// <summary>
        /// Все сэмплы ряда
        /// </summary>
        public IList<DataSample> Samples { get; }

        /// <summary>
        /// Период EMA, сглаживающей MACD
        /// </summary>
        public int MACDEmaPeriod { get; }

        /// <summary>
        /// Линия MACD
        /// </summary>
        public double MACDLine { get; private set; }

        /// <summary>
        /// Значение сигнальной линии
        /// </summary>
        public double Signal { get; private set; }

        /// <summary>
        /// Значение гистограммы
        /// </summary>
        public double Histogram { get; private set; }

        public void Calculate()
        {
            double p1 = MACDEmaPeriod + 1;
            double MACDEMAMultiplier = Convert.ToDouble(2 / p1);

            MACDLine = (Sample.EMAshort.Value - Sample.EMAlong.Value); // default is 12EMA - 26EMA
            var previousCandlesCount = Samples.Count(a => a.Candle.Timestamp < Sample.Candle.Timestamp);


            if (previousCandlesCount == Sample.EMAlong.Period + MACDEmaPeriod - 1)
            {
                // Set this to SMA of MACDLine to seed it
                Signal = Samples.Where(a => a.Candle.Timestamp <= Sample.Candle.Timestamp).OrderByDescending(a => a.Candle.Timestamp).Take(MACDEmaPeriod).Average(a => (((MACDIndicator)a.Indicators["macd"]).MACDLine));
            }
            else if (previousCandlesCount > Sample.EMAlong.Period + MACDEmaPeriod - 1)
            {
                // We can calculate this EMA based off past Candle EMAs
                double? lastMACDSignalLine = ((MACDIndicator)Samples.Where(a => a.Candle.Timestamp < Sample.Candle.Timestamp).OrderByDescending(a => a.Candle.Timestamp).Take(1).FirstOrDefault().Indicators["macd"]).Signal;
                Signal = ((MACDLine - lastMACDSignalLine.Value) * MACDEMAMultiplier) + lastMACDSignalLine.Value;
            }
            Histogram = MACDLine - Signal;
        }
    }
}
