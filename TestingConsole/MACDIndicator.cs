using System;
using System.Collections.Generic;
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
        /// <param name="candle">Свеча</param>
        /// <param name="shortEMAPeriod">Период короткой EMA</param>
        /// <param name="longEMAPeriod">Период длинной EMA</param>
        /// <param name="macdEMAPeriod">Период EMA для сглаживания индикатора</param>
        public MACDIndicator(DataSample sample, IList<DataSample> samples, int macdEMAPeriod)
        {
            _emaShort = sample.EMAshort;
            _emaLong = sample.EMAlong;
            EMAForMACD = new EMA(macdEMAPeriod, samples, sample, EMAKind.Smoothing);
            Calculate();
        }

        /// <summary>
        /// Экспоненциальная скользящая средняя малого периода для расчета индикатора
        /// </summary>
        private EMA _emaShort;

        /// <summary>
        /// Экспоненциальная скользящая средняя большого периода для расчета индикатора
        /// </summary>
        private EMA _emaLong;

        /// <summary>
        /// Экспоненциальная скользящая средняя малого периода для сглаживания MACD
        /// </summary>
        public EMA EMAForMACD;

        public void Calculate()
        {
            throw new NotImplementedException();
        }
    }
}
