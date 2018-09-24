namespace IEXApiHandler
{
    using IEXApiHandler.IEXData;
    using IEXApiHandler.IEXData.Stock;
    using System;
    using System.Collections.Generic;

    public class IEXClientHandler : HttpClientHandler
    {
        readonly string _batchReportUrlFormat = "https://api.iextrading.com/1.0/stock/{0}/batch?types={1}&range={2}&last={3}";
        readonly string _batchReportMultiSymbolsUrlFormat = "https://api.iextrading.com/1.0/stock/market/batch?symbols={0}&types={1}&range={2}&last={3}";
        readonly string _reportUrlFormat = "https://api.iextrading.com/1.0/stock/{0}/financials?period={1}";
        readonly string _companyUrlFormat = "https://api.iextrading.com/1.0/stock/{0}/company";
        readonly string _chartUrlFormat = "https://api.iextrading.com/1.0/stock/{0}/chart/{1}";
        readonly string _keyStatsUrlFormat = "https://api.iextrading.com/1.0/stock/{0}/stats";
        readonly string _quoteUrlFormat = "https://api.iextrading.com/1.0/stock/{0}/quote";

        public BatchReport GetBatchReport(string symbol, IEnumerable<IEXDataType> types, ChartOption range, int itemCount)
        {
            string rangeStr = range.ToString().StartsWith("_") ? range.ToString().Remove(0, 1) : range.ToString();
            string requestUrl = string.Format(_batchReportUrlFormat, symbol, types.ToStringDelimeter(), rangeStr, itemCount.LimitToMax());
            Console.WriteLine(requestUrl);

            string jsonstring = base.GetString(requestUrl).Result;
            return SerializerHandler.DeserializeObj<BatchReport>(jsonstring);
        }

        public Dictionary<string, BatchReport> GetBatchReport(string[] symbols, IEnumerable<IEXDataType> types, ChartOption range, int itemCount)
        {
            string rangeStr = range.ToString().StartsWith("_") ? range.ToString().Remove(0, 1) : range.ToString();
            string requestUrl = string.Format(_batchReportMultiSymbolsUrlFormat, symbols.ToStringDelimeter(), types.ToStringDelimeter(), rangeStr, itemCount.LimitToMax());
            Console.WriteLine(requestUrl);

            string jsonstring = base.GetString(requestUrl).Result;
            return SerializerHandler.DeserializeObj<Dictionary<string, BatchReport>>(jsonstring);
        }
        
        public Company GetCompany(string symbol)
        {
            string requestUrl = string.Format(_companyUrlFormat, symbol);
            string jsonstring = base.GetString(requestUrl).Result;
            return SerializerHandler.DeserializeObj<Company>(jsonstring);
        }

        public Financials GetReport(string symbol, Period period)
        {
            string reportUrl = string.Format(_reportUrlFormat, symbol, period.ToString());

            string jsonstring = base.GetString(reportUrl).Result;

            Financials result = SerializerHandler.DeserializeObj<Financials>(jsonstring);
            return result;
        }

        public List<Chart> GetHistoricData(string symbol, ChartOption chartOption)
        {
            string optionStr = chartOption.ToString().StartsWith("_") ? chartOption.ToString().Remove(0, 1) : chartOption.ToString();
            string requestUrl = string.Format(_chartUrlFormat, symbol, optionStr);

            string jsonstring = base.GetString(requestUrl).Result;

            List<Chart> result = SerializerHandler.DeserializeObj<List<Chart>>(jsonstring);
            return result;
        }

        public Stat GetKeyStats(string symbol)
        {
            string reportUrl = string.Format(_keyStatsUrlFormat, symbol);
            string jsonstring = base.GetString(reportUrl).Result;
            return SerializerHandler.DeserializeObj<Stat>(jsonstring);
        }

        public Quote GetQuote(string symbol)
        {
            string requestUrl = string.Format(_quoteUrlFormat, symbol);
            string jsonstring = base.GetString(requestUrl).Result;
            return SerializerHandler.DeserializeObj<Quote>(jsonstring);
        }
    }
}
