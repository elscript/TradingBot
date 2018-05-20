using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Authentication;

namespace TestingConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            var client = new BitfinexClient(
                new BitfinexClientOptions() {
                    ApiCredentials = new ApiCredentials(
                        key: "", 
                        secret: "")
                });

            /*
            var alerts = client.GetAlertList();
            foreach (var alert in alerts.Data)
            {
                Console.WriteLine($"AlertKey : {alert.AlertKey}; AlertType : {alert.AlertType}; Symbol : {alert.Symbol}; Price : {alert.Price}; T : {alert.T}");
            }

            var orders = client.GetActiveOrders();
            foreach (var order in orders.Data)
            {
                Console.WriteLine($"Amount : {order.Amount}; AmountOriginal : {order.AmountOriginal}; Symbol : {order.Symbol}; Price : {order.Price}; Type : {order.Type}; Status : {order.Status}");
            }
            */

            decimal percentOfProfit = 0;

            var candles = client.GetCandles(Bitfinex.Net.Objects.TimeFrame.FiveteenMinute, "tIOTUSD", 1000, DateTime.Now.AddDays(-30), DateTime.Now.ToUniversalTime());
            IEnumerable<BitfinexCandle> candlesData = candles.Data.ToList();

            for (int i = 0; i < 15; i++)
            {
                var data = candlesData.ToList();
                var morecandles = client.GetCandles(Bitfinex.Net.Objects.TimeFrame.FiveteenMinute, "tIOTUSD", 1000, null, data.Last().Timestamp.AddMinutes(-5));
                candlesData = data.Concat(morecandles.Data);
            }

            for (int k = 1; k < 6; k++)
            {
                var k1 = k;
                var filteredData = candlesData.Where(c => c.Timestamp >= new DateTime(2018, k1, 1, 12, 50, 0) && c.Timestamp <= new DateTime(2018, k1 + 1, 1, 12, 50, 0));

                //var strategyPlayer = new StrategyPlayer(new MACDandMFIStrategy(new decimal(0.5), false, true));
                var strategyPlayer = new StrategyPlayer(new LastForwardThenPreviousStrategy(new decimal(0), true, true));

                percentOfProfit += strategyPlayer.Run(filteredData.ToList()) * 100;
            }

            Console.WriteLine($"PercentOfProfit : {percentOfProfit}");
        }

        
    }
}
