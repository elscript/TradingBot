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
using TradingBot.Core.DataProviders;

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

            /*
            BinanceDataCrawler crawler = new BinanceDataCrawler();
            crawler.CrawlData();
            */

            var currentRunDateFrom = new DateTime(2019, 1, 1);
            var currentRunDateTo = new DateTime(2020, 7, 20);
            var tester = new StrategyTester(false, new HistoryDataProducer(new StorageDataProvider(), 50));
            while (currentRunDateFrom <= currentRunDateTo)
            {
                tester.Run(
                    "BTCUSDT",
                    Timeframe.FiveteenMinute,
                    100,
                    0.004m,
                    currentRunDateFrom,
                    new DateTime(2020, 7, 20)
                    );
                currentRunDateFrom = currentRunDateFrom.AddDays(1);
            }


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
