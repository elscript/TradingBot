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
            var fee = 0.5;
            decimal deposit = 100;
            Console.WriteLine($"Start Deposit : {deposit}$");
                        
            var strategyPlayer = new HistoricalStrategyPlayer(
                new LastForwardThenPreviousStrategy(
                    new decimal(fee), 
                    true, 
                    true
                ),
                new HistoryDataProvider(
                    _bitfinexManager,
                    TimeFrame.ThirtyMinute,
                    100,
                    15000
                )
            );

            decimal percentOfProfit = 0;

            for (int i = 1; i < 12; i++)
            {
                strategyPlayer.SetDateRange(
                    new DateTime(2017, i, 1, 0, 0, 0),
                    new DateTime(2017, i + 1, 1, 0, 0, 0)
                );

                strategyPlayer.Run("tIOTUSD");

                if (strategyPlayer.PlayedPositions.Count > 0)
                    WriteResult(ref percentOfProfit, strategyPlayer, ref deposit);
            }

            for (int i = 1; i < 7; i++)
            {
                strategyPlayer.SetDateRange(
                    new DateTime(2018, i, 1, 0, 0, 0), 
                    new DateTime(2018, i + 1, 1, 0, 0, 0)
                );
                
                strategyPlayer.Run("tIOTUSD");

                if (strategyPlayer.PlayedPositions.Count > 0)
                    WriteResult(ref percentOfProfit, strategyPlayer, ref deposit);
            }         
        }

        private static void WriteResult(ref decimal percentOfProfit, HistoricalStrategyPlayer strategyPlayer,
            ref decimal deposit)
        {
            var lastPercentOfProfit = percentOfProfit;
            percentOfProfit += strategyPlayer.ProfitRate * 100;
            Console.WriteLine(
                $"PercentOfProfit for period {strategyPlayer.PlayedPositions.First().OpenTimestamp} - {strategyPlayer.PlayedPositions.Last().CloseTimestamp} : {percentOfProfit - lastPercentOfProfit}%");

            foreach (var position in strategyPlayer.PlayedPositions)
            {
                if (position.Direction == PositionDirection.Long)
                    deposit += deposit * (position.ClosePrice - position.OpenPrice) / position.OpenPrice;
                else if (position.Direction == PositionDirection.Short)
                    deposit += deposit * (position.OpenPrice - position.ClosePrice) / position.OpenPrice;
            }

            Console.WriteLine($"Deposit after this period : {deposit}$");
            Console.WriteLine();
        }
    }
}
