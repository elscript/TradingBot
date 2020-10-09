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

            /*
            var exchange = new BinanceManager(
                "",
                ""); 
            
            BinanceDataCrawler crawler = new BinanceDataCrawler(exchange);
            crawler.CrawlData("BTCUSDT", Timeframe.OneMinute);
            crawler.CrawlData("BTCUSDT", Timeframe.FiveMinute);
            crawler.CrawlData("BTCUSDT", Timeframe.FiveteenMinute);
            crawler.CrawlData("BTCUSDT", Timeframe.ThirtyMinute);
            crawler.CrawlData("BTCUSDT", Timeframe.OneHour);
            crawler.CrawlData("BTCUSDT", Timeframe.SixHour);
            crawler.CrawlData("BTCUSDT", Timeframe.TwelveHour);
            crawler.CrawlData("BTCUSDT", Timeframe.OneDay);
            crawler.CrawlData("BTCUSDT", Timeframe.SevenDay);
            crawler.CrawlData("BTCUSDT", Timeframe.OneMonth);
            */


            /*
            var pairs = binanceMananger.GetTradingPairs();
            foreach (var item in pairs)
            {
                Console.WriteLine(item);
            }*/


            
            var currentRunDateFrom = new DateTime(2020, 9, 1, 0, 0, 0);
            var currentRunDateTo = new DateTime(2020, 9, 3);
            var tester = new StrategyTester(false, new HistoryDataProducer(new StorageDataProvider(), 1440));
            while (currentRunDateFrom <= currentRunDateTo)
            {
                tester.Run(
                    "BTCUSDT",
                    Timeframe.OneMinute,
                    100,
                    0.004m,
                    currentRunDateFrom,
                    new DateTime(2020, 9, 3)
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
