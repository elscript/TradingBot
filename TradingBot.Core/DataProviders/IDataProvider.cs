using System;
using System.Collections.Generic;
using System.Text;
using TradingBot.Core.Common;

namespace TradingBot.Core.DataProviders
{
    public interface IDataProvider
    {
        /// <summary>
        /// Получение всех доступных данных по тикеру
        /// </summary>
        /// <param name="ticker">Тикер</param>
        /// <param name="timeframe">Таймфрейм</param>
        /// <returns>Список свеч</returns>
        IList<Candle> GetAllData(string ticker, Timeframe timeframe);

        /// <summary>
        /// Получение доступных данных по тикеру за указанный инетрвал времени
        /// </summary>
        /// <param name="ticker">Тикер</param>
        /// <param name="timeframe">Таймфрейм</param>
        /// <param name="fromDate">Таймштамп начала интервала</param>
        /// <param name="toDate">Таймштамп конца интервала</param>
        /// <returns></returns>
        IList<Candle> GetDataForPeriod(string ticker, Timeframe timeframe, DateTime fromDate, DateTime toDate);
    }
}
