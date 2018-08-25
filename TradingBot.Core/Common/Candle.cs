using System;
using System.Collections.Generic;
using System.Text;
using Bitfinex.Net.Objects;

namespace TradingBot.Core.Common
{
    public class Candle
    {
        public int Id { get; set; }
        public TimeFrame TimeFrame { get; set; }
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

        public Candle(BitfinexCandle bitfinexCandle, TimeFrame timeFrame, string ticker)
        {
            this.Timestamp = bitfinexCandle.Timestamp;
            this.Close = bitfinexCandle.Close;
            this.High = bitfinexCandle.High;
            this.Open = bitfinexCandle.Open;
            this.Low = bitfinexCandle.Low;
            this.Volume = bitfinexCandle.Volume;
            this.TimeFrame = timeFrame;
            this.Ticker = ticker;
        }
    }
}
