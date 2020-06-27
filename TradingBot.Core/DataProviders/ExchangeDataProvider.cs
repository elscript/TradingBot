using System;
using System.Collections.Generic;
using System.Text;
using TradingBot.Core.Api;
using TradingBot.Core.Common;

namespace TradingBot.Core.DataProviders
{
    public class ExchangeDataProvider : IDataProvider
    {
        private readonly IExchangeApi _exchangeApi;

        public ExchangeDataProvider(IExchangeApi exchangeApi)
        {
            if (exchangeApi == null)
                throw new ArgumentNullException(nameof(exchangeApi), "Значение 'null' не допустимо для параметра");
            _exchangeApi = exchangeApi;
        }

        public IList<Candle> GetAllData(string ticker, Timeframe timeframe)
        {
            return _exchangeApi.GetData(ticker, timeframe, 1000); //Убрать хардкод
        }

        public IList<Candle> GetDataForPeriod(string ticker, Timeframe timeframe, DateTime fromDate, DateTime toDate)
        {
            return _exchangeApi.GetData(ticker, timeframe, 1000, fromDate, toDate); //Убрать хардкод
        }
    }
}
