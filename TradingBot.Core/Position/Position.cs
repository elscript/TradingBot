using System;
using System.Collections.Generic;
using System.Text;

namespace TradingBot.Core
{
    public class Position
    {
        public int Id { get; set; }

        public decimal OpenPrice { get; set; }

        public decimal ClosePrice { get; set; }

        public DateTime? OpenTimestamp { get; set; }

        public DateTime? CloseTimestamp { get; set; }

        public PositionDirection Direction { get; set; }

        public string Ticker { get; set; }

        public decimal Amount { get; set; }
    }
}
