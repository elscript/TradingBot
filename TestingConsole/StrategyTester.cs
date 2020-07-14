using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitfinex.Net;
using Bitfinex.Net.Objects;
using TradingBot.Core;
using TradingBot.Core.Common;
using TradingBot.Core.DataProviders;
using TradingBot.Core.Strategies;

namespace TestingConsole
{
    public class StrategyTester
    {
        public StrategyTester()
        {
        }

        public void Run(string ticker, Timeframe timeframe, decimal startDeposit, decimal fee, DateTime dateFrom, DateTime dateTo)
        {                      
            decimal deposit = startDeposit;
            string currency = "USD";
            Console.WriteLine($"Start Deposit : {deposit} {currency}");
            Console.WriteLine($"Ticker : {ticker}");

            var strategyPlayer = new HistoricalStrategyPlayer(
                new VolumeStrategy(
                    10,
                    3,
                    1
                ),
                new HistoryDataProducer(
                    new StorageDataProvider(),
                    50),
                null,
                fee,
                3
            );

            decimal percentOfProfit = 0;
            strategyPlayer.CurrentBalance = startDeposit;

            for (DateTime current = dateFrom; current < dateTo; current = current.AddMonths(1))
            {
                strategyPlayer.SetDateRange(current, current.AddMonths(1));
                
                strategyPlayer.Run(ticker, timeframe, strategyPlayer.CurrentBalance, currency);

                if (strategyPlayer.PlayedPositions.Count > 0)
                {
                    //CalculateCurrentDeposit(strategyPlayer, ref deposit, fee);
                    var lastPercentOfProfit = percentOfProfit;
                    CalculatePercentOfProfit(ref percentOfProfit, strategyPlayer);
                    WriteResult(percentOfProfit, lastPercentOfProfit, strategyPlayer, strategyPlayer.CurrentBalance, currency);
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

        private static void CalculateCurrentDeposit(StrategyPlayer strategyPlayer, ref decimal deposit, decimal fee)
        {
            foreach (var position in strategyPlayer.PlayedPositions)
            {
                if (position.Direction == PositionDirection.Long)
                    deposit += (deposit * (position.ClosePrice - position.OpenPrice) / position.OpenPrice) - position.OpenPrice * fee;
                else if (position.Direction == PositionDirection.Short)
                    deposit += deposit * (position.OpenPrice - position.ClosePrice) / position.OpenPrice - position.OpenPrice * fee;
            }
        }
    }
}
