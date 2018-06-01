using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net;
using Bitfinex.Net.Objects;
using TradingBot.Core;

namespace TestingConsole
{
    public class StrategyTester
    {
        private BitfinexManager _bitfinexManager;

        public StrategyTester(BitfinexManager bitfinexManager)
        {
            _bitfinexManager = bitfinexManager;
        }

        public void Run()
        {
            decimal percentOfProfit = 0;
            var positions = new List<PositionInternal>();
                        
            var fee = 0.5;
            decimal deposit = 100;
            Console.WriteLine($"Start Deposit : {deposit}$");
                        
            var strategyPlayer = new HistoricalStrategyPlayer(
                new LastForwardThenPreviousStrategy(
                    new decimal(fee), 
                    true, 
                    true
                ),
                new HistoryPortionDataProvider(
                    _bitfinexManager,
                    TimeFrame.ThirtyMinute,
                    100,
                    20000
                )
            );

            strategyPlayer.Run("tIOTUSD");;
        }
    }
}
