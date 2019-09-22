using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Net.Objects;
using Microsoft.EntityFrameworkCore;
using TradingBot.Core.Api;
using TradingBot.Core.Common;

namespace TradingBot.Core.DataCrawling
{
    public class BitfinexDataCrawler : AbstractDataCrawler
    {
        public BitfinexDataCrawler(BitfinexManager apiManager, string ticker)
        {
            _apiManager = apiManager;
            _ticker = ticker;
        }
    }
}
