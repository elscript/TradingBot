using System;
using System.Collections.Generic;
using System.Text;

namespace TradingBot.Core
{
    /// <summary>
    /// Торговая стратегия
    /// </summary>
    public interface IStrategy
    {
        /// <summary>
        /// Стратегия позволяет совершать Long-сделки
        /// </summary>
        bool AllowLong { get; }

        /// <summary>
        /// Стратегия позволяет совершать Short-сделки
        /// </summary>
        bool AllowShort { get; }

        /// <summary>
        /// Максимально допустимый процент потерь с позиции
        /// </summary>
        decimal MaxLoosePercentage { get; }

        /// <summary>
        /// Сигнал на покупку
        /// </summary>
        /// <param name="samples">Сэмплы данных</param>
        /// <param name="sample">Текущий сэмпл</param>
        /// <param name="lastSellPrice">Цена предыдущей предшествующией продажи, если была, иначе null</param>
        /// <returns></returns>
        SignalResult BuySignal(IList<DataSample> samples, DataSample sample, Position position);

        /// <summary>
        /// Сигнал на продажу
        /// </summary>
        /// <param name="samples">Сэмплы данных</param>
        /// <param name="sample">Текущий сэмпл</param>
        /// <param name="lastBuyPrice">Цена предыдущей предшествующией покупки, если была, иначе null</param>
        /// <returns></returns>
        SignalResult SellSignal(IList<DataSample> samples, DataSample sample, Position position);

        /// <summary>
        /// Получить цену для установки стопа
        /// </summary>
        /// <param name="samples">Сэмплы данных</param>
        /// <param name="sample">Текущий сэмпл</param>
        /// <param name="position">Текущая позиция</param>
        /// <returns></returns>
        decimal GetStopLossPrice(IList<DataSample> samples, DataSample sample, Position position);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopLossPrice"></param>
        /// <param name="openPrice"></param>
        /// <param name="currentBalance"></param>
        /// <param name="maximumLeverage"></param>
        /// <returns></returns>
        decimal GetAmountForPosition(decimal stopLossPrice, decimal openPrice, decimal currentBalance, int maximumLeverage);
    }
}
