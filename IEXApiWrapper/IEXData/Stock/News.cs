using MongoDB.Bson.Serialization.Attributes;
using System;

namespace IEXApiHandler.IEXData.Stock
{
    /*
     * 	"datetime": "2018-09-07T17:42:00-04:00",
		"headline": "Report: Apple discussing subscriptions for biggest papers",
		"source": "SeekingAlpha",
		"url": "https://api.iextrading.com/1.0/stock/aapl/article/5740017789868538",
		"summary": "     Apple (NASDAQ: AAPL ) has been talking to the biggest U.S. newspapers about  joining its Texture app , Recode notes, in what would be the foundation of a major news subscription service.   More news on: Apple Inc., New York Times Co., News Corporation, Tech stocks news, Stocks on the move,  …",
		"related": "AAPL,Computer Hardware,CON31167138,NASDAQ01,New York,NWS,NWSA,NYT,Computing and Information Technology,WOMPOLIX",
		"image": "https://api.iextrading.com/1.0/stock/aapl/news-image/5740017789868538"
     */
    public class News
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public Guid _id { get; set; } = Guid.NewGuid();
        public Guid companyId { get; set; }
        public DateTime CreatedDate {get; set;}
        public string HeadLine { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public string Summary { get; set; }
        public string Related { get; set; }
        public string Image { get; set; }
    }
}