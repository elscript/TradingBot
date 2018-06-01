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
        private BitfinexManager _bitfinexManager;

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
    }
}
