using System;
using System.Collections.Generic;
using System.Text;

namespace TradingBot.Core
{
    public class SignalResult
    {
        public bool ByStopLoss { get; set; }

        public bool SignalTriggered { get; set; }

        public SignalPurpose Purpose { get; set; }
    }
}
