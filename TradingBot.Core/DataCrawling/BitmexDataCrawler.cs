using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradingBot.Core.BitmexApi;

namespace TradingBot.Core.DataCrawling
{
    public class BitmexDataCrawler : AbstractDataCrawler
    {
        private BitmexManager _apiManager;
        private string _ticker;
        private Task _currentTask;
        private CancellationToken _cancelletionToken;
        CancellationTokenSource _cancelTokenSource;

        public BitmexDataCrawler(BitmexManager apiManager, string ticker)
        {
            _apiManager = apiManager;
            _ticker = ticker;
        }

        public void Run(TimeSpan period, int amountOfCandles)
        {
            throw new NotImplementedException();
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }
    }
}
