using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net.Objects;

namespace TestingConsole
{
    public class CandlesDataProcessor
    {
        private IList<DataSample> samples;
        public List<BitfinexCandle> Candles { get; }

        public CandlesDataProcessor(IList<BitfinexCandle> candles)
        {
            Candles = candles.OrderByDescending(t => t.Timestamp).ToList<BitfinexCandle>();

            samples = new List<DataSample>();
            foreach (var candle in candles)
            {
                samples.Add(new DataSample());
            }
        }

        /// <summary>
        /// Расчет скользящих средних
        /// </summary>
        /// <param name="shortEMAPeriod"></param>
        /// <param name="longEMAPeriod"></param>
        /// <param name="macdEMAPeriod"></param>
        public void CalculateEMAs(int shortEMAPeriod, int longEMAPeriod)
        {
            foreach (var sample in samples)
            {
                sample.EMAshort = new EMA(shortEMAPeriod, samples, sample, EMAKind.Short);
                sample.EMAlong = new EMA(longEMAPeriod, samples, sample, EMAKind.Long);
            }
        }

        /// <summary>
        /// Инициализация и расчет индикаторов
        /// </summary>
        public void CalculateIndicators(int macdEMAPeriod)
        {
            foreach (var sample in samples)
            {
                var macd = new MACDIndicator(sample, samples, macdEMAPeriod);
                sample.Indicators.Add("macd", macd);

                foreach (var indicator in sample.Indicators.Values)
                {
                    indicator.Calculate();
                }
            }
        }
    }
}
