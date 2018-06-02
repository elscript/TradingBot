using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;

namespace TradingBot.Core
{
    public interface IDataProvider
    {
        IList<BitfinexCandle> GetData(string ticker);

        IList<BitfinexCandle> GetData(string ticker, DateTime dateFrom, DateTime dateTo);
    }
}
