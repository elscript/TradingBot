using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;
using TradingBot.Core;
using TradingBot.Core.DataCrawling;

namespace TestingConsole
{
    public class CrawlingTester
    {
        private BitfinexManager _bitfinexManager;
        private BitfinexDataCrawler _dataCrawler;

        public CrawlingTester(BitfinexManager bitfinexManager)
        {
            _bitfinexManager = bitfinexManager;
        }

        public void Run(string ticker)
        {
            _dataCrawler = new BitfinexDataCrawler(_bitfinexManager, ticker);
            _dataCrawler.Run(TimeSpan.FromSeconds(60), 100);
        }
    }
}
