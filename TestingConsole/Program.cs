using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Net;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Authentication;
using TradingBot.Core;

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

            var tester = new StrategyTester();
            tester.TestStrategy(new BitfinexManager("", ""));
            tester.Run();
        }

        
    }
}
