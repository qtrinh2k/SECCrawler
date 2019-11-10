using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IEXApiHandler.IEXData.Stock
{
    public class BatchReport
    {
        [JsonProperty("company")]
        public Company Company { get; set; }

        [JsonProperty("quote")]
        public Quote Quote { get; set; }

        [JsonProperty("news")]
        public List<News> NewsList { get; set; }

        [JsonProperty("chart")]
        public List<Chart> Charts { get; set; }

        [JsonProperty("stats")]
        public Stat Stat { get; set; }

        [JsonProperty("financials")]
        public Financials Financials { get; set; }
    }
}
