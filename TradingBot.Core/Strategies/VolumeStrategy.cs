using System;
using System.Collections.Generic;
using System.Text;

namespace TradingBot.Core.Strategies
{
    public class VolumeStrategy : IStrategy
    {
        public bool AllowLong { get; }
        public bool AllowShort { get; }
        public decimal MaxLoosePercentage { get; }

        public SignalResult BuySignal(IList<DataSample> samples, DataSample sample, decimal? lastSellPrice)
        {
            throw new NotImplementedException();
        }

        public decimal GetStopLossPrice(IList<DataSample> samples, DataSample sample, Position position)
        {
            throw new NotImplementedException();
        }

        public SignalResult SellSignal(IList<DataSample> samples, DataSample sample, decimal? lastBuyPrice)
        {
            throw new NotImplementedException();
        }
    }
}
