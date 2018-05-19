using System;
using System.Collections.Generic;
using System.Text;

namespace TestingConsole
{
    /// <summary>
    /// Стратегия, основанная на комбинации MACD и MFI
    /// </summary>
    public class MACDandMFIStrategy : IStrategy
    {
        public MACDandMFIStrategy()
        {
        }

        public MACDandMFIStrategy(decimal maxLoosePercentage, bool allowLong, bool allowShort) : this()
        {
            MaxLoosePercentage = maxLoosePercentage;
            AllowLong = allowLong;
            AllowShort = allowShort;
        }

        /// <summary>
        /// Максимально допустимый процент потерь с позиции
        /// </summary>
        private decimal MaxLoosePercentage { get; set; }

        /// <summary>
        /// Разрешать лонги
        /// </summary>
        public bool AllowLong { get; set; }

        /// <summary>
        /// Разрешать шорты
        /// </summary>
        public bool AllowShort { get; set; }


        public bool BuySignal(IList<DataSample> samples, DataSample sample, decimal? lastSellPrice)
        {
            // Проверяем, не сработал ли кастомный стоплосс с учетом максимального процента потерь
            if (lastSellPrice != null && sample.Candle.Close > lastSellPrice + lastSellPrice * (MaxLoosePercentage / 100))
            {
                return true;
            }

            // Если MFI выходит из зоны перепроданности
            var mfiCheckPassed = false;
            var currentSampleIndex = samples.IndexOf(sample);
            if (currentSampleIndex != 0) // Будем сверяться с предыдущим сэмплом. Для самого первого сверять не с чем, поэтому нужна такая проверка
            {
                // Проверяем, что MFI пересек 20ку
                if (((MFIIndicator) samples[currentSampleIndex - 1].Indicators["mfi"]).MFI < 20 &&
                    ((MFIIndicator) sample.Indicators["mfi"]).MFI > 20)
                {
                    mfiCheckPassed = true;
                }
            }

            // Дополнительно проверяем, что MACD гистограмма выше, чем на прошлом сэмпле
            var macdCheckPassed = false;
            if (currentSampleIndex != 0
            ) // Будем сверяться с предыдущим сэмплом. Для самого первого сверять не с чем, поэтому нужна такая проверка
            {
                if (((MACDIndicator) samples[currentSampleIndex].Indicators["macd"]).Histogram >
                    ((MACDIndicator) samples[currentSampleIndex - 1].Indicators["macd"]).Histogram)
                {
                    macdCheckPassed = true;
                }
            }

            return mfiCheckPassed && macdCheckPassed;
        }

        public bool SellSignal(IList<DataSample> samples, DataSample sample, decimal? lastBuyPrice)
        {
            // Проверяем, не сработал ли кастомный стоплосс с учетом максимального процента потерь
            if (lastBuyPrice != null && sample.Candle.Close < lastBuyPrice - lastBuyPrice * (MaxLoosePercentage / 100))
            {
                return true;
            }

            // Если MFI выходит из зоны перепроданности
            var mfiCheckPassed = false;
            var currentSampleIndex = samples.IndexOf(sample);
            if (currentSampleIndex != 0) // Будем сверяться с предыдущим сэмплом. Для самого первого сверять не с чем, поэтому нужна такая проверка
            {
                // Проверяем, что MFI пересек 80ку
                if (((MFIIndicator)samples[currentSampleIndex - 1].Indicators["mfi"]).MFI > 80 &&
                    ((MFIIndicator)sample.Indicators["mfi"]).MFI < 80)
                {
                    mfiCheckPassed = true;
                }
            }

            // Дополнительно проверяем, что MACD гистограмма ниже, чем на прошлом сэмпле
            var macdCheckPassed = false;
            if (currentSampleIndex != 0
            ) // Будем сверяться с предыдущим сэмплом. Для самого первого сверять не с чем, поэтому нужна такая проверка
            {
                if (((MACDIndicator)samples[currentSampleIndex].Indicators["macd"]).Histogram <
                    ((MACDIndicator)samples[currentSampleIndex - 1].Indicators["macd"]).Histogram)
                {
                    macdCheckPassed = true;
                }
            }

            return mfiCheckPassed && macdCheckPassed;
        }
    }
}
