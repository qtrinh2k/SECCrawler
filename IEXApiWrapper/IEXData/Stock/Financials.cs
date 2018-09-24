using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IEXApiHandler.IEXData.Stock
{
    public enum Period
    {
        Quarter,
        Annual
    }

    public class Financials
    {
        public string symbol { get; set; }
        public List<Financial> financials { get; set; }
    }

    public class Financial
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public Guid _id { get; set; } = Guid.NewGuid();
        public Guid companyId { get; set; }
        public double? cashChange { get; set; }
        public double? shareholderEquity { get; set; }
        public double? totalDebt { get; set; }
        public double? totalCash { get; set; }
        public double? currentDebt { get; set; }
        public double? currentCash { get; set; }
        public double? totalLiabilities { get; set; }
        public double? totalAssets { get; set; }
        public double? cashFlow { get; set; }
        public double? currentAssets { get; set; }
        public double? researchAndDevelopment { get; set; }
        public double? netIncome { get; set; }
        public double? operatingIncome { get; set; }
        public double totalRevenue { get; set; }
        public double? operatingRevenue { get; set; }
        public double? costOfRevenue { get; set; }
        public double? grossProfit { get; set; }
        public string reportDate { get; set; }
        public double? operatingExpense { get; set; }
        public string operatingGainsLosses { get; set; }
    }
}
