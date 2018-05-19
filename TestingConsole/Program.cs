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

            var candles = client.GetCandles(Bitfinex.Net.Objects.TimeFrame.FiveMinute, "tIOTUSD", 1000, DateTime.Now.AddDays(-30), DateTime.Now);
            IEnumerable<BitfinexCandle> candlesData = candles.Data.ToList();
            for (int i = 0; i < 9; i++)
            {
                var data = candlesData.ToList();
                var morecandles = client.GetCandles(Bitfinex.Net.Objects.TimeFrame.FiveMinute, "tIOTUSD", 1000, null, data.Last().Timestamp.AddMinutes(-5));
                candlesData = data.Concat(morecandles.Data);
            }
            
            

            //foreach (var candle in candles.Data)
            //{
            //    Console.WriteLine($"Timestamp : {candle.Timestamp}; Open : {candle.Open}; Close : {candle.Close}; Low : {candle.Low}; High : {candle.High}; Volume : {candle.Volume}");
            //}


            var strategyPlayer = new StrategyPlayer(new MACDandMFIStrategy(new decimal(2), true, true));
            var percentOfProfit = strategyPlayer.Run(candlesData.ToList()) * 100;
            Console.WriteLine($"PercentOfProfit : {percentOfProfit}");
        }

        
    }
}
