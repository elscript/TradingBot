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

        public void Run(string ticker)
        {                      
            var fee = 0.5;
            decimal deposit = 100;
            string currency = "USD";
            Console.WriteLine($"Start Deposit : {deposit} {currency}");
            Console.WriteLine($"Ticker : {ticker}");

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
                    1500
                ),
                null
            );

            decimal percentOfProfit = 0;


            strategyPlayer.SetDateRange(
                new DateTime(2018, 6, 1, 0, 0, 0),
                new DateTime(2018, 6, 12, 0, 0, 0)
            );

            strategyPlayer.Run(ticker, deposit, currency);

            if (strategyPlayer.PlayedPositions.Count > 0)
            {
                CalculateCurrentDeposit(strategyPlayer, ref deposit);
                var lastPercentOfProfit = percentOfProfit;
                CalculatePercentOfProfit(ref percentOfProfit, strategyPlayer);
                WriteResult(percentOfProfit, lastPercentOfProfit, strategyPlayer, deposit, currency);
            }

            /*
            for (int i = 1; i < 12; i++)
            {
                strategyPlayer.SetDateRange(
                    new DateTime(2017, i, 1, 0, 0, 0),
                    new DateTime(2017, i + 1, 1, 0, 0, 0)
                );

                strategyPlayer.Run(ticker, deposit, currency);

                if (strategyPlayer.PlayedPositions.Count > 0)
                {
                    CalculateCurrentDeposit(strategyPlayer, ref deposit);
                    var lastPercentOfProfit = percentOfProfit;
                    CalculatePercentOfProfit(ref percentOfProfit, strategyPlayer);
                    WriteResult(percentOfProfit, lastPercentOfProfit, strategyPlayer, deposit, currency);
                }
            }

            for (int i = 1; i < 7; i++)
            {
                strategyPlayer.SetDateRange(
                    new DateTime(2018, i, 1, 0, 0, 0), 
                    new DateTime(2018, i + 1, 1, 0, 0, 0)
                );
                
                strategyPlayer.Run(ticker, deposit, currency);

                if (strategyPlayer.PlayedPositions.Count > 0)
                {
                    CalculateCurrentDeposit(strategyPlayer, ref deposit);
                    var lastPercentOfProfit = percentOfProfit;
                    CalculatePercentOfProfit(ref percentOfProfit, strategyPlayer);
                    WriteResult(percentOfProfit, lastPercentOfProfit, strategyPlayer, deposit, currency);
                }
                
            }   */      
        }

        private static void WriteResult(decimal percentOfProfit, decimal lastPercentOfProfit, StrategyPlayer strategyPlayer, decimal deposit, string currency)
        {
            Console.WriteLine(
                $"PercentOfProfit for period {strategyPlayer.PlayedPositions.First().OpenTimestamp} - {strategyPlayer.PlayedPositions.Last().CloseTimestamp} : {percentOfProfit - lastPercentOfProfit}%");;

            Console.WriteLine($"Deposit after this period : {Math.Round(deposit, 2)} {currency}");
            Console.WriteLine();
        }

        private static void CalculatePercentOfProfit(ref decimal percentOfProfit, StrategyPlayer strategyPlayer)
        {
            percentOfProfit += strategyPlayer.ProfitRate * 100;
        }

        private static void CalculateCurrentDeposit(StrategyPlayer strategyPlayer, ref decimal deposit)
        {
            foreach (var position in strategyPlayer.PlayedPositions)
            {
                if (position.Direction == PositionDirection.Long)
                    deposit += deposit * (position.ClosePrice - position.OpenPrice) / position.OpenPrice;
                else if (position.Direction == PositionDirection.Short)
                    deposit += deposit * (position.OpenPrice - position.ClosePrice) / position.OpenPrice;
            }
        }
    }
}
