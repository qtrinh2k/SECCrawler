namespace IEXApiHandler
{
    using NetworkCommon;
    using IEXApiHandler.IEXData;
    using IEXApiHandler.IEXData.Stock;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public enum IEXRequestType
    {
        Quote,
        Stats,
        Chart,
        CompanyInfo,
        Financial,
        BatchReportForAStock,
        BatchReportMarketForMultiStocks,
        BatchReportFinancialForMultiStocks
    }

    public class IEXClientHandler : HttpClientWrapper
    {
        const string _iexTradingBaseUri = "https://cloud.iexapis.com/stable";
        readonly string _privateToken;
        const string _IEXTokenFileName = "config\\token.dat";

        readonly IDictionary<IEXRequestType, string> _dictRequestUrl;

        public IEXClientHandler()
        {
            _privateToken = this.GetIEXToken(_IEXTokenFileName);
            this._dictRequestUrl = LoadRequestUrl();
        }

        private IDictionary<IEXRequestType, string> LoadRequestUrl()
        {
            IDictionary<IEXRequestType, string> dictRequestUrl = new Dictionary<IEXRequestType, string>()
            {
                { IEXRequestType.Quote, this.CreateIEXBaseUri("/stock/{0}/quote") },
                { IEXRequestType.Stats, this.CreateIEXBaseUri("/stock/{0}/stats") },
                { IEXRequestType.Chart, this.CreateIEXBaseUri("/stock/{0}/chart/{1}") },
                { IEXRequestType.CompanyInfo, this.CreateIEXBaseUri("/stock/{0}/company") },
                { IEXRequestType.Financial,  this.CreateIEXBaseUri("/stock/{0}/financials", "period={1}") },
                { IEXRequestType.BatchReportForAStock, this.CreateIEXBaseUri("/stock/{0}/batch", "types={1}&range={2}&last={3}") },
                { IEXRequestType.BatchReportFinancialForMultiStocks, this.CreateIEXBaseUri("/stock/market/batch", "symbols={0}&types=financials&period={1}") },
                { IEXRequestType.BatchReportMarketForMultiStocks, this.CreateIEXBaseUri("/stock/market/batch", "symbols={0}&types={1}&range={2}&last={3}") },
            };

            return dictRequestUrl;
        }

        private string GetIEXToken(string tokenFile)
        {
            string tokenFilePath = string.Empty;
            var currentDir = Environment.CurrentDirectory;
            tokenFilePath = File.Exists(tokenFile) ? tokenFile : Path.Combine(currentDir, tokenFile);

            if (!File.Exists(tokenFilePath))
            {
                throw new FileNotFoundException($"IEXToken filePath:{tokenFilePath} not found.");
            }
            string token = File.ReadAllText(tokenFilePath).Trim();
            return token;
        }

        public string GetIEXBaseRequestUri(IEXRequestType requestType)
        {
            return this._dictRequestUrl[requestType];
        }

        public string CreateIEXBaseUri(string iexRoute, string queryStr = "")
        {
            string tokenStr = string.Format($"?token={_privateToken}");
            string postRequestStrWithToken = string.IsNullOrEmpty(queryStr) ? tokenStr : string.Concat(tokenStr, "&" ,queryStr);

            return string.Concat(_iexTradingBaseUri, iexRoute, postRequestStrWithToken);
        }

        public BatchReport GetBatchReport(string symbol, IEnumerable<IEXDataType> types, ChartOption range, int itemCount)
        {
            string rangeStr = range.ToString().StartsWith("_") ? range.ToString().Remove(0, 1) : range.ToString();
            string baseRequestUri = GetIEXBaseRequestUri(IEXRequestType.BatchReportForAStock);
            string requestUrl = string.Format(baseRequestUri, symbol, types.ToStringDelimeter(), rangeStr, itemCount.LimitToMax());
            Console.WriteLine(requestUrl);

            string jsonstring = base.GetString(requestUrl).Result;
            return SerializerHandler.DeserializeObj<BatchReport>(jsonstring);            
        }

        public Dictionary<string, BatchReport> GetBatchReport(string[] symbols, IEnumerable<IEXDataType> types, ChartOption range, int itemCount, Period period = Period.Annual)
        {
            string rangeStr = range.ToString().StartsWith("_") ? range.ToString().Remove(0, 1) : range.ToString();
            //string requestUrl = string.Format(_batchReportMultiSymbolsUrlFormat, symbols.ToStringDelimeter(), types.ToStringDelimeter(), rangeStr, itemCount.LimitToMax());
            string baseRequestUri = GetIEXBaseRequestUri(IEXRequestType.BatchReportMarketForMultiStocks);
            string requestUrl = string.Format(
                baseRequestUri,
                symbols.ToStringDelimeter(),
                types.ToStringDelimeter(),
                rangeStr,
                itemCount.LimitToMax());

            if (types.Contains(IEXDataType.financials))
            {
                requestUrl = requestUrl + $"&period={period}";
            }
            requestUrl = requestUrl.ToLower();
            Console.WriteLine(requestUrl);

            string jsonstring = base.GetString(requestUrl.ToLower()).Result;
            return SerializerHandler.DeserializeObj<Dictionary<string, BatchReport>>(jsonstring);
        }
        
        /// <summary>
        /// BatchReport of financial reports as annually or quarterly
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public List<Chart> GetBatchFinancials(string[] symbols, Period period = Period.Annual)
        {
            //string requestUrl = string.Format(_batchFinancialUrlFormat, symbols, period);
            string requestUrl = string.Format(this._dictRequestUrl[IEXRequestType.BatchReportFinancialForMultiStocks], symbols, period);
            string jsonstring = base.GetString(requestUrl).Result;

            List<Chart> result = SerializerHandler.DeserializeObj<List<Chart>>(jsonstring);
            return result;
        }

        public string GetParams(IDictionary<string, string> dictParams)
        {
            return string.Join("&", dictParams.Select(x => x.Key + "=" + x.Value).ToArray());
        }


        public Company GetCompany(string symbol)
        {
            //string requestUrl = string.Format(_companyUrlFormat, symbol);
            string requestUrl = string.Format(this.GetIEXBaseRequestUri(IEXRequestType.CompanyInfo), symbol);
            string jsonstring = base.GetString(requestUrl).Result;
            return SerializerHandler.DeserializeObj<Company>(jsonstring);
        }

        public Financials GetFinancialReport(string symbol, Period period)
        {
            //string reportUrl = string.Format(_reportUrlFormat, symbol, period.ToString());
            string reportUrl = string.Format(this.GetIEXBaseRequestUri(IEXRequestType.Financial), symbol, period.ToString());
            string jsonstring = base.GetString(reportUrl).Result;

            Financials result = SerializerHandler.DeserializeObj<Financials>(jsonstring);
            return result;
        }

        public List<Chart> GetHistoricData(string symbol, ChartOption chartOption)
        {
            string optionStr = chartOption.ToString().StartsWith("_") ? chartOption.ToString().Remove(0, 1) : chartOption.ToString();
            string requestUrl = string.Format(this.GetIEXBaseRequestUri(IEXRequestType.Chart), symbol, optionStr);
            string jsonstring = base.GetString(requestUrl).Result;

            List<Chart> result = SerializerHandler.DeserializeObj<List<Chart>>(jsonstring);
            return result;
        }

        public Stat GetKeyStats(string symbol)
        {
            string reportUrl = string.Format(this.GetIEXBaseRequestUri(IEXRequestType.Stats), symbol);
            string jsonstring = base.GetString(reportUrl).Result;
            return SerializerHandler.DeserializeObj<Stat>(jsonstring);
        }

        public Quote GetQuote(string symbol)
        {
            //string requestUrl = string.Format(_quoteUrlFormat, symbol);
            string requestUrl = string.Format(this.GetIEXBaseRequestUri(IEXRequestType.Quote), symbol);
            string jsonstring = base.GetString(requestUrl).Result;
            return SerializerHandler.DeserializeObj<Quote>(jsonstring);
        }       
    }
}
