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
        IList<Candle> GetData(string ticker, Timeframe timeFrame, int amount);

        /// <summary>
        /// Получение данных
        /// </summary>
        /// <param name="ticker">Валютная пара</param>
        /// <param name="timeFrame">Таймфрейм</param>
        /// <param name="amount">Кол-во свечей</param>
        /// <param name="dateTo">Начало диапазона</param>
        /// <param name="dateTo">Конец диапазона</param>
        /// <returns>Список свечей</returns>
        IList<Candle> GetData(string ticker, Timeframe timeFrame, int amount, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// Покупка по инструменту
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="amount"></param>
        /// <param name="price"></param>
        /// <returns>Успешность операции</returns>
        bool Buy(string symbol, int amount, decimal price);
    }
}
