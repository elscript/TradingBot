using System;
using System.Collections.Generic;
using System.Text;

namespace TradingBot.Core.DataCrawling
{
    public interface IDataCrawler
    {
        void Run(TimeSpan period, int amountOfCandles, string ticker);

        void Terminate();
    }
}
