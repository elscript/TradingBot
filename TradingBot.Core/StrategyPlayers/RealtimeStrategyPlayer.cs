using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net;
using Bitfinex.Net.Objects;
using TradingBot.Core.Common;

namespace TradingBot.Core
{
    public class RealtimeStrategyPlayer : StrategyPlayer
    {
        private readonly BitfinexManager _bitfinexManager;

        public RealtimeStrategyPlayer(IStrategy strategy, IDataProducer dataProvider, BitfinexManager bitfinexManager, Position startPosition) : base(strategy, dataProvider, startPosition)
        {
            _bitfinexManager = bitfinexManager;
        }

        protected override void OnOpenPosition(Position position)
        {
            position.OpenPrice = _bitfinexManager.ExecuteDealByMarket(
                position.Ticker,
                position.Direction == PositionDirection.Long ? OrderSide.Buy : OrderSide.Sell,
                position.Amount
            );

            Console.WriteLine(
                $"##OnOpenPosition for Ticker {position.Ticker} with OpenPrice is {position.OpenPrice}. Details --- OpenTimestamp: {position.OpenTimestamp}, Direction: {position.Direction}, Amount: {position.Amount}");

            using (ApplicationContext db = new ApplicationContext())
            {
                db.Positions.Add(position);
                db.SaveChanges();
            }
        }

        protected override void OnClosePosition(Position position)
        {
            position.ClosePrice = _bitfinexManager.ExecuteDealByMarket(
                position.Ticker,
                position.Direction == PositionDirection.Long ? OrderSide.Sell : OrderSide.Buy,
                position.Amount
            );

            Console.WriteLine($"##OnClosePosition for Ticker {position.Ticker} with ClosePrice is {position.ClosePrice}. Details --- OpenTimestamp: {position.OpenTimestamp}, OpenPrice: {position.OpenPrice}, CloseTimestamp: {position.CloseTimestamp}, Direction: {position.Direction}, Amount: {position.Amount}");

            using (ApplicationContext db = new ApplicationContext())
            {
                db.Positions.Update(position);
                db.SaveChanges();
            }
        }

        protected override bool ShouldContinue(string ticker, Timeframe timeframe)
        {
            return true;
        }

        protected override IList<Candle> GetData(string ticker, Timeframe timeFrame)
        {
            var result = Producer.GetData(ticker, timeFrame);
            //Console.WriteLine($"##GetData Timestamp: {result.Last().Timestamp}, Volume: {result.Last().Volume}, Low: {result.Last().Low}, High: {result.Last().High}, Close: {result.Last().Close}");
            return result;
        }

        protected override void OnStop()
        {
            Console.WriteLine($"##OnStop");
        }

        protected override decimal GetAmount(decimal initialAmount, string currency)
        {
            return _bitfinexManager.GetCurrentBalance(currency);
        }

        protected override void SetCurrentPosition()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Position = db.Positions.LastOrDefault(p => p.ClosePrice == 0);
            }
        }

        protected override void OnSetStopLoss(decimal price)
        {
            throw new NotImplementedException();
        }
    }
}
