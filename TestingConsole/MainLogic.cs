using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net.Objects;
using TradingBot.Core;

namespace TestingConsole
{
    public class MainLogic
    {
        private BitfinexManager _bitfinexManager;
        private StrategyPlayer _player;

        public MainLogic(BitfinexManager bitfinexManager)
        {
            _bitfinexManager = bitfinexManager;
        }

        public void Run()
        {
            try
            {
                var data = _bitfinexManager.GetData("tIOTUSD", TimeFrame.ThirtyMinute, 100);
                var position = _bitfinexManager.GetActivePosition();
                var balance = _bitfinexManager.GetCurrentBalance("usd");
                var fee = 0.5;

                var strategyPlayer = new RealtimeStrategyPlayer(new LastForwardThenPreviousStrategy(new decimal(fee), true, true), _bitfinexManager.OpenPosition, _bitfinexManager.ClosePosition);
                strategyPlayer.Run(data);
                //var percentOfProfit = strategyPlayer.Run(data) * 100;
                var positions = strategyPlayer.PlayedPositions;

                var profitPositions = positions.Where(p =>
                    (p.Direction == PositionDirection.Long && p.ClosePrice > p.ClosePrice) ||
                    (p.Direction == PositionDirection.Short && p.ClosePrice < p.ClosePrice));

                var losePositions = positions.Where(p =>
                    (p.Direction == PositionDirection.Long && p.ClosePrice < p.ClosePrice) ||
                    (p.Direction == PositionDirection.Short && p.ClosePrice > p.ClosePrice));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
