using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IEXApiHandler.IEXData.Stock
{
    /// <summary>
    /// Options
    /// Range Description Source
    /// 5y Five years Historically adjusted market-wide data
    /// 2y Two years Historically adjusted market-wide data
    /// 1y One year Historically adjusted market-wide data
    /// ytd Year-to-date Historically adjusted market-wide data
    /// 6m	Six months  Historically adjusted market-wide data
    /// 3m	Three months    Historically adjusted market-wide data
    /// 1m	One month(default) Historically adjusted market-wide data
    /// 1d	One day IEX-only data by minute
    /// date Specific date IEX-only data by minute for a specified date in the format YYYYMMDD if available.Currently supporting trailing 30 calendar days.
    /// dynamic One day Will return 1d or 1m data depending on the day or week and time of day.Intraday per minute data is only returned during market hours.
    /// </summary>
    public enum ChartOption
    {
        _5y,
        _2y,
        _1y,
        _ytd,
        _6m,
        _3m,
        _1m,
        _1d,
        _dyn
    }

    /*
     * Ref: https://iextrading.com/developer/docs/#chart
     * Response
        Key	Type	Availability
        minute	string	is only available on 1d chart.
        marketAverage	number	is only available on 1d chart. 15 minute delayed
        marketNotional	number	is only available on 1d chart. 15 minute delayed
        marketNumberOfTrades	number	is only available on 1d chart. 15 minute delayed
        marketOpen	number	is only available on 1d chart. 15 minute delayed
        marketClose	number	is only available on 1d chart. 15 minute delayed
        marketHigh	number	is only available on 1d chart. 15 minute delayed
        marketLow	number	is only available on 1d chart. 15 minute delayed
        marketVolume	number	is only available on 1d chart. 15 minute delayed
        marketChangeOverTime	number	is only available on 1d chart. Percent change of each interval relative to first value. 15 minute delayed
        average	number	is only available on 1d chart.
        notional	number	is only available on 1d chart.
        numberOfTrades	number	is only available on 1d chart.
        simplifyFactor	array	is only available on 1d chart, and only when chartSimplify is true. The first element is the original number of points. Second element is how many remain after simplification.
        high	number	is available on all charts.
        low	number	is available on all charts.
        volume	number	is available on all charts.
        label	number	is available on all charts. A variable formatted version of the date depending on the range. Optional convienience field.
        changeOverTime	number	is available on all charts. Percent change of each interval relative to first value. Useful for comparing multiple stocks.
        date	string	is available on all charts.
        open	number	is available on all charts.
        close	number	is available on all charts.
        unadjustedVolume	number	is not available on 1d chart.
        change	number	is not available on 1d chart.
        changePercent	number	is not available on 1d chart.
        vwap	number	is not available on 1d chart.
    */
    
    public class Chart
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public Guid _id { get; set; } = Guid.NewGuid();
        public Guid companyId { get; set; }
        public string date { get; set; }
        public float? open { get; set; }
        public float? high { get; set; }
        public float? low { get; set; }
        public float? close { get; set; }
        public int? volume { get; set; }
        public int? unadjustedVolume { get; set; }
        public float? change { get; set; }
        public float? changePercent { get; set; }
        public float? vwap { get; set; }
        public string label { get; set; }
        public float? changeOverTime { get; set; }
        public string minute { get; set; }
        public float? average { get; set; }
        public float? notional { get; set; }
        public int? numberOfTrades { get; set; }
        public float? marketHigh { get; set; }
        public float? marketLow { get; set; }
        public float? marketAverage { get; set; }
        public int? marketVolume { get; set; }
        public float? marketNotional { get; set; }
        public int? marketNumberOfTrades { get; set; }
        public int? marketOpen { get; set; }
        public float? marketClose { get; set; }
        public int? marketChangeOverTime { get; set; }
    }
}
