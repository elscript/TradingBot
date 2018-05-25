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

            var tester = new StrategyTester();
            tester.TestStrategy(client);
        }

        
    }
}
