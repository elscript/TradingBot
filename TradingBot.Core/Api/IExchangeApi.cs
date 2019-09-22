using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;
using TradingBot.Core.Common;

namespace TradingBot.Core.Api
{
    public interface IExchangeApi
    {
        /// <summary>
        /// Получение данных
        /// </summary>
        /// <param name="ticker">Валютная пара</param>
        /// <param name="timeFrame">Таймфрейм</param>
        /// <param name="amount">Кол-во свечей</param>
        /// <returns>Список свечей</returns>
        IList<Candle> GetData(string ticker, TimeFrame timeFrame, int amount);

        /// <summary>
        /// Получение данных
        /// </summary>
        /// <param name="ticker">Валютная пара</param>
        /// <param name="timeFrame">Таймфрейм</param>
        /// <param name="amount">Кол-во свечей</param>
        /// <param name="dateTo">Конец диапазона</param>
        /// <returns>Список свечей</returns>
        IList<Candle> GetData(string ticker, TimeFrame timeFrame, int amount, DateTime dateTo);
    }
}
