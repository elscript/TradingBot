using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bitfinex.Net;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Authentication;
using TradingBot.Core;
using TradingBot.Core.Api.Binance;
using TradingBot.Core.Common;
using TradingBot.Core.DataCrawling;

namespace TestingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
                //var candlesTillNow = binanceMananger.GetData(ticker, TradingBot.Core.Common.Timeframe.FiveteenMinute, 1000);
                //var candlesTillYesterday = binanceMananger.GetData(ticker, TradingBot.Core.Common.Timeframe.FiveteenMinute, 1000, DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-1));


                /*
                var pairs = binanceMananger.GetTradingPairs();
                foreach (var item in pairs)
                {
                    Console.WriteLine(item);
                }*/

                //var tester = new StrategyTester(bitfinexManager);
                //tester.Run(
                //    ticker,
                //    TimeFrame.ThirtyMinute,
                //    100,
                //    0.4,
                //    new DateTime(2017, 7, 1),
                //    new DateTime(2018, 7, 1)
                //    );


                /*
                var _client = new BitfinexClient(
                    new BitfinexClientOptions()
                    {
                        ApiCredentials = new ApiCredentials(
                            key: _accessKey,
                            secret: _accessSecret)
                    });
                */

                /*
                var tradeLogic = new RealTradesLogic(bitfinexManager);
                tradeLogic.Run(currency, ticker);
                */

                /*
                var crawlingTester = new CrawlingTester(new BitfinexDataCrawler(bitfinexManager));
                crawlingTester.Run(ticker);

                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(60));
                }
                */
            }      
    }
}
