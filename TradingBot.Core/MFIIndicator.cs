using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestingConsole
{
    public class MFIIndicator : IIndicator
    {
        public MFIIndicator(DataSample sample, IList<DataSample> samples, int mfiPeriod)
        {
            Sample = sample;
            Samples = samples;
            MFIPeriod = mfiPeriod;
        }

        /// <summary>
        /// Рассчитываемый сэмпл
        /// </summary>
        public DataSample Sample { get; }

        /// <summary>
        /// Все сэмплы ряда
        /// </summary>
        public IList<DataSample> Samples { get; }

        public int MFIPeriod { get; }

        public decimal MoneyFlowChange { get; private set; }

        public decimal? MFI { get; private set; }

        public decimal? MoneyFlowRatio { get; private set; }

        public void Calculate()
        {
            //var currentSampleCounter = Samples.IndexOf(Sample);

            var previousCandlesCount = Samples.Count(a => a.Candle.Timestamp < Sample.Candle.Timestamp);

            // For MFI
            if (previousCandlesCount > 0)
            {
                // This line uses a function in the candle class to set the Money Flow Change by passing the previous typical price
                var previousTypicalPrice = Samples.Where(a => a.Candle.Timestamp < Sample.Candle.Timestamp).OrderByDescending(a => a.Candle.Timestamp).Take(1).FirstOrDefault().TypicalPrice;

                MoneyFlowChange = Sample.TypicalPrice - previousTypicalPrice;
            }
            else
            {
                MoneyFlowChange = 0;
            }

            if (previousCandlesCount >= MFIPeriod - 1) // We have enough candles we can actually start calculating the MFI
            {
                // Have to start with MoneyFlowRatio
                // Positive flow gets the sum of all the raw money flow on days where money flow change was positive, negative flow is opposite.
                decimal? PositiveFlow = Samples.Where(a => a.Candle.Timestamp <= Sample.Candle.Timestamp).OrderByDescending(a => a.Candle.Timestamp).Take(MFIPeriod).Where(a => ((MFIIndicator)a.Indicators["mfi"]).MoneyFlowChange > 0).Sum(a => a.RawMoneyFlow);
                decimal? NegativeFlow = Samples.Where(a => a.Candle.Timestamp <= Sample.Candle.Timestamp).OrderByDescending(a => a.Candle.Timestamp).Take(MFIPeriod).Where(a => ((MFIIndicator)a.Indicators["mfi"]).MoneyFlowChange < 0).Sum(a => a.RawMoneyFlow);

                if (NegativeFlow != 0)
                    MoneyFlowRatio = PositiveFlow / NegativeFlow;

                MFI = 100 - (100 / (1 + MoneyFlowRatio));
            }
        }
    }
}
