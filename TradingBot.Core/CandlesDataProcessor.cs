using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net.Objects;

namespace TestingConsole
{
    public class CandlesDataProcessor
    {
        public IList<DataSample> Samples { get; private set; }

        public CandlesDataProcessor(IList<BitfinexCandle> candles)
        {
            var orderedCandles = candles.OrderBy(t => t.Timestamp).ToList<BitfinexCandle>();

            Samples = new List<DataSample>();
            foreach (var candle in orderedCandles)
            {
                Samples.Add(new DataSample(candle));
            }
        }

        /// <summary>
        /// Расчет скользящих средних
        /// </summary>
        /// <param name="shortEMAPeriod"></param>
        /// <param name="longEMAPeriod"></param>
        public void CalculateEMAs(int shortEMAPeriod, int longEMAPeriod)
        {
            foreach (var sample in Samples)
            {
                sample.EMAshort = new EMA(shortEMAPeriod, Samples, sample, EMAKind.Short);
                sample.EMAlong = new EMA(longEMAPeriod, Samples, sample, EMAKind.Long);
            }
        }

        /// <summary>
        /// Инициализация и расчет индикаторов
        /// </summary>
        /// <param name="macdEMAPeriod">Период для сглаживающей EMA</param>
        public void CalculateIndicators(int macdEMAPeriod, int mfiPeriod)
        {
            foreach (var sample in Samples)
            {
                var macd = new MACDIndicator(sample, Samples, macdEMAPeriod);
                sample.Indicators.Add("macd", macd);

                var mfi = new MFIIndicator(sample, Samples, mfiPeriod);
                sample.Indicators.Add("mfi", mfi);

                foreach (var indicator in sample.Indicators.Values)
                {
                    indicator.Calculate();
                }
            }
        }
    }
}
