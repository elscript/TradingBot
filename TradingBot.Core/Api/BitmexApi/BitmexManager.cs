using Bitmex.NET;
using Bitmex.NET.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Objects;
using Bitmex.NET.Dtos;
using TradingBot.Core.Api;
using TradingBot.Core.Common;
using OrderSide = Bitmex.NET.Models.OrderSide;

namespace TradingBot.Core.BitmexApi
{
    public class BitmexManager : IExchangeApi
    {
        private string _accessKey;
        private string _accessSecret;
        private BitmexApiService _client;

        public BitmexManager(string accessKey, string accessSecret, bool isTestEnvironment)
        {
            _accessKey = accessKey;
            _accessSecret = accessSecret;

            var bitmexAuthorization = new BitmexAuthorization()
            {
                BitmexEnvironment = isTestEnvironment ? BitmexEnvironment.Test : BitmexEnvironment.Prod,
                Key = accessKey,
                Secret = accessSecret
            };
            var _client = BitmexApiService.CreateDefaultApi(bitmexAuthorization);
        }

        public async Task<OrderDto> Buy(string symbol, int amount, decimal price)
        {
            var posOrderParams = OrderPOSTRequestParams.CreateSimpleLimit(symbol, amount, price, OrderSide.Buy);
            return await _client.Execute(BitmexApiUrls.Order.PostOrder, posOrderParams);
        }

        public async Task<OrderDto> Sell(string symbol, int amount, decimal price)
        {
            var posOrderParams = OrderPOSTRequestParams.CreateSimpleLimit(symbol, amount, price, OrderSide.Sell);
            return await _client.Execute(BitmexApiUrls.Order.PostOrder, posOrderParams);
        }

        public async Task<OrderDto> BuyByMarket(string symbol, int amount)
        {
            var posOrderParams = OrderPOSTRequestParams.CreateSimpleMarket(symbol, amount, OrderSide.Buy);
            return await _client.Execute(BitmexApiUrls.Order.PostOrder, posOrderParams);
        }

        public async Task<OrderDto> SellByMarket(string symbol, int amount)
        {
            var posOrderParams = OrderPOSTRequestParams.CreateSimpleMarket(symbol, amount, OrderSide.Sell);
            return await _client.Execute(BitmexApiUrls.Order.PostOrder, posOrderParams);
        }

        public async Task<List<InstrumentDto>> GetData(string symbol)
        {
            var instrumentParams = new InstrumentGETRequestParams()
            {
                Symbol = symbol,
                Columns = "",
                Count = 1,
                Start = 30,
                Reverse = false,
                StartTime = DateTime.Now.AddDays(-1),
                EndTime = DateTime.Now
            };

            return await _client.Execute(BitmexApiUrls.Instrument.GetInstrument, instrumentParams);
        }

        public IList<Candle> GetData(string ticker, TimeFrame timeFrame, int amount)
        {
            throw new NotImplementedException();
        }

        public IList<Candle> GetData(string ticker, TimeFrame timeFrame, int amount, DateTime dateTo)
        {
            throw new NotImplementedException();
        }
    }
}
