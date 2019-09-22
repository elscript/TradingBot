using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Bitfinex.Net.Objects;
using TradingBot.Core.Common;

namespace TradingBot.Core
{
    /// <summary>
    /// Полный кусок данных, включая данные свечи, EMA, MACD, MFI
    /// </summary>
    public class DataSample
    {
        public DataSample(Candle candle)
        {
            Candle = candle;
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
        public Candle Candle { get; private set; }

        /// <summary>
        /// Типичная цена
        /// </summary>
        public decimal TypicalPrice
        {
            get
            {
                return (Candle.Low + Candle.High + Candle.Close) / 3;
            }
        }

        /// <summary>
        /// Чистый денежный поток
        /// </summary>
        public decimal RawMoneyFlow
        {
            get { return (TypicalPrice * Candle.Volume); } // 0 if null
        }

        /// <summary>
        /// Коллекция индикаторов
        /// </summary>
        public IDictionary<string, IIndicator> Indicators { get; }
    }
}
