using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;

namespace TradingBot.Core.Common
{
    public class Candle
    {
        public int Id { get; set; }
        public Timeframe TimeFrame { get; set; }
        public string Ticker { get; set; }
        public decimal Volume { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Close { get; set; }
        public DateTime Timestamp { get; set; }

        public Candle()
        {
            
        }
    }
}
