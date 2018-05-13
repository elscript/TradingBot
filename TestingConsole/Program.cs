using System;
using Bitfinex.Net;
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

            var candles = client.GetCandles(Bitfinex.Net.Objects.TimeFrame.FiveMinute, "tNEOUSD", 1000, DateTime.Now.AddDays(-30), DateTime.Now);
            foreach (var candle in candles.Data)
            {
                Console.WriteLine($"Timestamp : {candle.Timestamp}; Open : {candle.Open}; Close : {candle.Close}; Low : {candle.Low}; High : {candle.High}; Volume : {candle.Volume}");
            }


            var strategyPlayer = new StrategyPlayer(new MACDandMFIStrategy(1, true, false));
            var percentOfProfit = strategyPlayer.Run(candles.Data);
            Console.WriteLine($"PercentOfProfit : {percentOfProfit}");
        }

        
    }
}
