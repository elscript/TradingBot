using System;
using System.Collections.Generic;
using System.Text;

namespace TradingBot.Core
{
    public class HistoricalStrategyPlayer : StrategyPlayer
    {
        public HistoricalStrategyPlayer(IStrategy strategy, IDataProvider dataProvider) : base(strategy, dataProvider)
        {
        }

        protected override void OnOpenPosition(PositionInternal position)
        {
        
        }

        protected override void OnClosePosition(PositionInternal position)
        {
            
        }
    }
}
