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
        public void TestStrategy(BitfinexClient client)
        {
            decimal percentOfProfit = 0;
            var positions = new List<PositionInternal>();

            var candles = client.GetCandles(Bitfinex.Net.Objects.TimeFrame.ThirtyMinute, "tIOTUSD", 1000, null, DateTime.Now.ToUniversalTime());
            IEnumerable<BitfinexCandle> candlesData = candles.Data.ToList();

            for (int i = 0; i < 10; i++)
            {
                var data = candlesData.ToList();
                if (data.Count() != 0)
                {
                    var morecandles = client.GetCandles(Bitfinex.Net.Objects.TimeFrame.ThirtyMinute, "tIOTUSD", 1000, null, data.Last().Timestamp.AddMinutes(-5));
                    candlesData = data.Concat(morecandles.Data);
                }
            }

            decimal deposit = 100;
            Console.WriteLine($"Start Deposit : {deposit}$");
            for (int k = 1; k < 7; k++)
            {
                var k1 = k;
                var filteredData = candlesData.Where(c => c.Timestamp >= new DateTime(2018, k1, 1, 01, 00, 0) && c.Timestamp <= new DateTime(2018, k1 + 1, 01, 00, 45, 0));
                if (filteredData.Count() != 0)
                {

                    //var strategyPlayer = new StrategyPlayer(new MACDandMFIStrategy(new decimal(0.5), false, true));
                    var strategyPlayer = new HistoricalStrategyPlayer(new LastForwardThenPreviousStrategy(new decimal(0.5), true, true));

                    var lastPercentOfProfit = percentOfProfit;
                    strategyPlayer.Run(filteredData.ToList());
                    percentOfProfit += strategyPlayer.ProfitRate * 100;
                    positions.AddRange(strategyPlayer.PlayedPositions);
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
            Console.WriteLine($"Total PercentOfProfit : {percentOfProfit}%");
            Console.WriteLine($"Total Deposit : {deposit}$");
        }
    }
}
