using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;
using TradingBot.Core.Common;

namespace TradingBot.Core
{
    public interface IDataProvider
    {
        IList<Candle> GetData(string ticker);

        IList<Candle> GetData(string ticker, DateTime dateFrom, DateTime dateTo);
    }
}
