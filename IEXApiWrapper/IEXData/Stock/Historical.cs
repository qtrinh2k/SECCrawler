using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IEXApiHandler.IEXData.Stock
{
    public class Historical
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public Guid _id { get; set; } = Guid.NewGuid();
        public Guid companyId { get; set; }
        public string date { get; set; }
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public int volume { get; set; }
        public int unadjustedVolume { get; set; }
        public double change { get; set; }
        public double changePercent { get; set; }
        public double vwap { get; set; }
        public string label { get; set; }
        public double changeOverTime { get; set; }
    }

}
