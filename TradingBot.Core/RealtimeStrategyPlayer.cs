using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net;
using TestingConsole;

namespace TradingBot.Core
{
    public class RealtimeStrategyPlayer : StrategyPlayer
    {
        private Action _openPositionAction;

        public RealtimeStrategyPlayer(IStrategy strategy, Action openPositionAction, Action closePositionAction) : base(strategy)
        {
            _openPositionAction = openPositionAction;
        }

        protected override void OnOpenPosition(PositionInternal position)
        {
            _openPositionAction();
        }

        protected override void OnClosePosition(PositionInternal position)
        {
            throw new NotImplementedException();
        }

        protected override void RunStrategy(CandlesDataProcessor processor)
        {
            DoMainLogic(processor.Samples, processor.Samples.Last());
        }

        protected override void CalculateProfitRate()
        {
            throw new NotImplementedException();
        }
    }
}
