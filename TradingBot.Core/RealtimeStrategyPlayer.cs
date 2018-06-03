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
        private PositionInternal _lastNotFinishedPosition;

        public RealtimeStrategyPlayer(IStrategy strategy, IDataProvider dataProvider, BitfinexManager bitfinexManager, PositionInternal startPosition, PositionInternal lastPosition) : base(strategy, dataProvider, startPosition)
        {
            _bitfinexManager = bitfinexManager;
            _lastNotFinishedPosition = lastPosition;
        }

        protected override void OnOpenPosition(PositionInternal position)
        {
            position.OpenPrice = _bitfinexManager.ExecuteDealByMarket(
                position.Ticker,
                position.Direction == PositionDirection.Long ? OrderSide.Buy : OrderSide.Sell,
                position.Amount    
            );

            _lastNotFinishedPosition = position;

            Console.WriteLine($"##OnOpenPosition for Ticker {position.Ticker} with OpenPrice is {position.OpenPrice}. Details --- OpenTimestamp: {position.OpenTimestamp}, Direction: {position.Direction}, Amount: {position.Amount}");

            //TODO запись позиции в БД
        }

        protected override void OnClosePosition(PositionInternal position)
        {
            position.ClosePrice = _bitfinexManager.ExecuteDealByMarket(
                position.Ticker,
                position.Direction == PositionDirection.Long ? OrderSide.Sell : OrderSide.Buy,
                position.Amount
            );

            Console.WriteLine($"##OnClosePosition for Ticker {position.Ticker} with ClosePrice is {position.ClosePrice}. Details --- OpenTimestamp: {position.OpenTimestamp}, OpenPrice: {position.OpenPrice}, CloseTimestamp: {position.CloseTimestamp}, Direction: {position.Direction}, Amount: {position.Amount}");
            
            //TODO запись позиции в БД
        }

        protected override bool ShouldContinue(string ticker)
        {
            return true;
        }

        protected override IList<BitfinexCandle> GetData(string ticker)
        {
            var result = Provider.GetData(ticker);
            Console.WriteLine($"##GetData Timestamp: {result.Last().Timestamp}, Volume: {result.Last().Volume}, Low: {result.Last().Low}, High: {result.Last().High}, Close: {result.Last().Close}");
            return result;
        }

        protected override void OnStop()
        {
            
        }

        protected override decimal GetAmount(decimal initialAmount)
        {
            return _bitfinexManager.GetCurrentBalance("USD");
        }
    }
}
