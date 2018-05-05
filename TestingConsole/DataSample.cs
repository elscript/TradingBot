using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Bitfinex.Net.Objects;

namespace TestingConsole
{
    /// <summary>
    /// Полный кусок данных, включая данные свечи, EMA, MACD, MFI
    /// </summary>
    public class DataSample
    {
        public DataSample()
        {
            Indicators = new Dictionary<string, IIndicator>();    
        }

        /// <summary>
        /// Экспоненциальная скользящая средняя меньшего периода
        /// </summary>
        public EMA EMAshort { get; set; }

        /// <summary>
        /// Экспоненциальная скользящая средняя большего периода
        /// </summary>
        public EMA EMAlong { get; set; }

        /// <summary>
        /// Данные свечи
        /// </summary>
        public BitfinexCandle candle { get; set; }

        /// <summary>
        /// Коллекция индикаторов
        /// </summary>
        public IDictionary<string, IIndicator> Indicators { get; }
    }
}
