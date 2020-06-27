using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Bitfinex.Net.Objects;
using TradingBot.Core;
using TradingBot.Core.Common;

namespace TestingConsole
{
    public class RealTradesLogic
    {
        private readonly BitfinexManager _bitfinexManager;

        Position lastPosition = null;

        public RealTradesLogic(BitfinexManager bitfinexManager)
        {
            _bitfinexManager = bitfinexManager;
        }

        public void Run(string currency, string ticker, Timeframe timeframe)
        {
            try
            {
                //var position = _bitfinexManager.GetActivePosition();
                var balance = _bitfinexManager.GetCurrentBalance(currency);
                var fee = 0.5;

                var strategyPlayer = new RealtimeStrategyPlayer(
                    new LastForwardThenPreviousStrategy(
                        0, 
                        true, 
                        true
                    ), 
                    new DelayedDataProducer(
                        _bitfinexManager,  
                        100,
                        60000
                    ), 
                    _bitfinexManager,
                    lastPosition);

                strategyPlayer.Run(ticker, timeframe, balance, currency);
                //var percentOfProfit = strategyPlayer.Run(data) * 100;
                //var positions = strategyPlayer.PlayedPositions;

                //var profitPositions = positions.Where(p =>
                //    (p.Direction == PositionDirection.Long && p.ClosePrice > p.ClosePrice) ||
                //    (p.Direction == PositionDirection.Short && p.ClosePrice < p.ClosePrice));

                //var losePositions = positions.Where(p =>
                //    (p.Direction == PositionDirection.Long && p.ClosePrice < p.ClosePrice) ||
                //    (p.Direction == PositionDirection.Short && p.ClosePrice > p.ClosePrice));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Run(currency, ticker, timeframe);
            }
        }
    }
}
