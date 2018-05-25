using System;
using System.Collections.Generic;
using System.Text;
using TestingConsole;

namespace TradingBot.Core
{
    public class HistoricalStrategyPlayer : StrategyPlayer
    {
        public HistoricalStrategyPlayer(IStrategy strategy) : base(strategy)
        {
        }

        protected override void OnOpenPosition(PositionInternal position)
        {
        
        }

        protected override void OnClosePosition(PositionInternal position)
        {
            
        }

        protected override void RunStrategy(CandlesDataProcessor processor)
        {
            ProfitRate = 0;
            PlayedPositions.Clear();

            foreach (var sample in processor.Samples)
            {
                DoMainLogic(processor.Samples, sample);
            }
        }

        protected override void CalculateProfitRate()
        {
            foreach (var position in PlayedPositions)
            {
                if (position.Direction == PositionDirection.Long)
                    ProfitRate += (position.ClosePrice - position.OpenPrice) / position.OpenPrice;
                else if (position.Direction == PositionDirection.Short)
                    ProfitRate += (position.OpenPrice - position.ClosePrice) / position.OpenPrice;
            }
        }
    }
}
