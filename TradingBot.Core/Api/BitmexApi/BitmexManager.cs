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
        public IList<Candle> GetData(string ticker, Timeframe timeFrame, int amount)
        {
            throw new NotImplementedException();
        }

        public IList<Candle> GetData(string ticker, Timeframe timeFrame, int amount, DateTime dateTo)
        {
            throw new NotImplementedException();
        }

        public bool Buy(string symbol, int amount, decimal price)
        {
            var result = this.BuyInternal(symbol, amount, price);
            result.Wait();
            return result.Result.Result.TransactTime != null;
        }

        private async Task<BitmexApiResult<OrderDto>> BuyInternal(string symbol, int amount, decimal price)
        {
            var posOrderParams = OrderPOSTRequestParams.CreateSimpleLimit(symbol, amount, price, OrderSide.Buy);
            return await _client.Execute(BitmexApiUrls.Order.PostOrder, posOrderParams);
        }

        private async Task<BitmexApiResult<OrderDto>> SellInternal(string symbol, int amount, decimal price)
        {
            var posOrderParams = OrderPOSTRequestParams.CreateSimpleLimit(symbol, amount, price, OrderSide.Sell);
            return await _client.Execute(BitmexApiUrls.Order.PostOrder, posOrderParams);
        }

        private async Task<BitmexApiResult<OrderDto>> BuyByMarketInternal(string symbol, int amount)
        {
            var posOrderParams = OrderPOSTRequestParams.CreateSimpleMarket(symbol, amount, OrderSide.Buy);
            return await _client.Execute(BitmexApiUrls.Order.PostOrder, posOrderParams);
        }

        private async Task<BitmexApiResult<OrderDto>> SellByMarketInternal(string symbol, int amount)
        {
            var posOrderParams = OrderPOSTRequestParams.CreateSimpleMarket(symbol, amount, OrderSide.Sell);
            return await _client.Execute(BitmexApiUrls.Order.PostOrder, posOrderParams);
        }

        private async Task<BitmexApiResult<List<InstrumentDto>>> GetDataInternal(string symbol)
        {
            var instrumentParams = new InstrumentGETRequestParams()
            {
                Symbol = symbol,
                Columns = "",
                Count = 1,
                Start = 30,
                Reverse = false,
                StartTime = DateTime.Now.AddDays(-1), //TODO убрать хардкод
                EndTime = DateTime.Now
            };

            return await _client.Execute(BitmexApiUrls.Instrument.GetInstrument, instrumentParams);
        }
    }
}
