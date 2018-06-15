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

        public void Run(string ticker, TimeFrame timeframe, decimal startDeposit, double fee, DateTime dateFrom, DateTime dateTo)
        {                      
            decimal deposit = startDeposit;
            string currency = "USD";
            Console.WriteLine($"Start Deposit : {deposit} {currency}");
            Console.WriteLine($"Ticker : {ticker}");

            var strategyPlayer = new HistoricalStrategyPlayer(
                new LastForwardThenPreviousStrategy(
                    new decimal(2),
                    true, 
                    true
                ),
                new HistoryDataProvider(
                    _bitfinexManager,
                    timeframe,
                    100,
                    15000
                ),
                null
            );

            decimal percentOfProfit = 0;

            for (DateTime current = dateFrom; current < dateTo; current = current.AddMonths(1))
            {
                strategyPlayer.SetDateRange(current, current.AddMonths(1));
                
                strategyPlayer.Run(ticker, deposit, currency);

                if (strategyPlayer.PlayedPositions.Count > 0)
                {
                    CalculateCurrentDeposit(strategyPlayer, ref deposit, new decimal(fee));
                    var lastPercentOfProfit = percentOfProfit;
                    CalculatePercentOfProfit(ref percentOfProfit, strategyPlayer);
                    WriteResult(percentOfProfit, lastPercentOfProfit, strategyPlayer, deposit, currency);
                }
            }
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

        private static void CalculateCurrentDeposit(StrategyPlayer strategyPlayer, ref decimal deposit, decimal feePercentage)
        {
            foreach (var position in strategyPlayer.PlayedPositions)
            {
                if (position.Direction == PositionDirection.Long)
                    deposit += (deposit * (position.ClosePrice - position.OpenPrice) / position.OpenPrice) - position.OpenPrice * feePercentage / 100;
                else if (position.Direction == PositionDirection.Short)
                    deposit += deposit * (position.OpenPrice - position.ClosePrice) / position.OpenPrice - position.OpenPrice * feePercentage / 100;
            }
        }
    }
}
