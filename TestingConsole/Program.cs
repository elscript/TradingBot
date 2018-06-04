using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            var ticker = "tIOTUSD";
            var currency = "USD";
            var _accessKey = "";
            var _accessSecret = "";
            var bitfinexManager = new BitfinexManager(
                _accessKey,
                _accessSecret
            );

            /*
            var tester = new StrategyTester(bitfinexManager);
            tester.Run(ticker);
            */

            /*
            var _client = new BitfinexClient(
                new BitfinexClientOptions()
                {
                    ApiCredentials = new ApiCredentials(
                        key: _accessKey,
                        secret: _accessSecret)
                });
            */

            var tradeLogic = new RealTradesLogic(bitfinexManager);
            tradeLogic.Run(currency, ticker);
        }

        
    }
}
