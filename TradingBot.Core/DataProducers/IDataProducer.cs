using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;
using TradingBot.Core.Common;

namespace TradingBot.Core
{
    public interface IDataProducer
    {
        IList<Candle> GetData(string ticker, Timeframe timeframe);

        IList<Candle> GetData(string ticker, Timeframe timeframe, DateTime dateFrom, DateTime dateTo);
    }
}
