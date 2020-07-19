using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;
using TradingBot.Core;
using TradingBot.Core.DataCrawling;
using TradingBot.Core.Api;

namespace TestingConsole
{
    public class CrawlingTester
    {
        private IDataCrawler _dataCrawler;

        public CrawlingTester(IDataCrawler crawler)
        {
            _dataCrawler = crawler;
        }

        public void Run(string ticker)
        {
            _dataCrawler.Run(TimeSpan.FromSeconds(60), 100, ticker);
        }
    }
}
