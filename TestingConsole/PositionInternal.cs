using System;
using System.Collections.Generic;
using System.Text;

namespace TestingConsole
{
    public class PositionInternal
    {
        public decimal OpenPrice { get; set; }

        public decimal ClosePrice { get; set; }

        public DateTime OpenTimestamp { get; set; }

        public DateTime CloseTimestamp { get; set; }

        public PositionDirection Direction { get; set; }
    }
}
