using System;
using System.Collections.Generic;
using System.Text;

namespace TradingBot.Core
{
    public class ExtremumArea
    {
        public DataSample LastLocalMinimum;
        public DataSample LastLocalMaximum;
        public DataSample CurrentExtremum;
        public ExtremumType Type;
    }
}
