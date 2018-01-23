namespace SECCrawler
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Bson;

    public class Filing
    {
        public Filing() { }

        public string AccessionNunber { get; set; }
        public string fileNumber { get; set; }
        public string fileNumberHref { get; set; }
        public string FilingDate { get; set; }
        public string FilingHref { get; set; }
        public string FilingType { get; set; }
        public string DownloadReportPath { get; set; }
        public string DownloadStatus { get; set; }
    }

    public class CIKInfo
    {
        public long CIK { get; set; }
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Exchange { get; set; }
        public string SIC { get; set; }
        public string Business { get; set; }
        public string Incorportated { get; set; }
        public string IRS { get; set; }

    }

    [JsonObject]
    public class SECFilingInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public CIKInfo CompanyInfo { get; set; }

        public List<Filing> Filings { get; set; }
    }
}