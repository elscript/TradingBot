using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net;
using Bitfinex.Net.Objects;

namespace TradingBot.Core
{
    public class RealtimeStrategyPlayer : StrategyPlayer
    {
        private readonly BitfinexManager _bitfinexManager;

        public RealtimeStrategyPlayer(IStrategy strategy, IDataProvider dataProvider, BitfinexManager bitfinexManager) : base(strategy, dataProvider)
        {
            _bitfinexManager = bitfinexManager;
        }

        protected override void OnOpenPosition(PositionInternal position)
        {
            position.OpenPrice = _bitfinexManager.ExecuteDealByMarket(
                position.Ticker,
                position.Direction == PositionDirection.Long ? OrderSide.Buy : OrderSide.Sell,
                position.Amount    
            );
        }

        protected override void OnClosePosition(PositionInternal position)
        {
            position.ClosePrice = _bitfinexManager.ExecuteDealByMarket(
                position.Ticker,
                position.Direction == PositionDirection.Long ? OrderSide.Sell : OrderSide.Buy
                ,
                position.Amount
            );
        }

        protected override bool ShouldContinue(string ticker)
        {
            return true;
        }

        protected override IList<BitfinexCandle> GetData(string ticker)
        {
            return Provider.GetData(ticker);
        }

        protected override void OnStop()
        {
            
        }
    }
}
