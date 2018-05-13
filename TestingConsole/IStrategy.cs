﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TestingConsole
{
    /// <summary>
    /// Торговая стратегия
    /// </summary>
    public interface IStrategy
    {
        /// <summary>
        /// Сигнал на покупку
        /// </summary>
        /// <param name="samples">Сэмплы данных</param>
        /// <param name="sample">Текущий сэмпл</param>
        /// <param name="lastSellPrice">Цена предыдущей предшествующией продажи, если была, иначе null</param>
        /// <returns></returns>
        bool BuySignal(IList<DataSample> samples, DataSample sample, decimal? lastSellPrice);

        /// <summary>
        /// Сигнал на продажу
        /// </summary>
        /// <param name="samples">Сэмплы данных</param>
        /// <param name="sample">Текущий сэмпл</param>
        /// <param name="lastBuyPrice">Цена предыдущей предшествующией покупки, если была, иначе null</param>
        /// <returns></returns>
        bool SellSignal(IList<DataSample> samples, DataSample sample, decimal? lastBuyPrice);

        /// <summary>
        /// Стратегия позволяет совершать Long-сделки
        /// </summary>
        bool AllowLong { get; }

        /// <summary>
        /// Стратегия позволяет совершать Short-сделки
        /// </summary>
        bool AllowShort { get; }
    }
}
