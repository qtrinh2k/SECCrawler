using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Serializers;

namespace SECCrawler
{
    public class DataRepository
    {
        public async Task MongoInsertJsonAsync(string jsonContent)
        {
            var connectionString = "mongodb://localhost";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("StockDB");

            //var document = BsonSerializer.Deserialize<BsonDocument>(jsonContent);
            //var collection = database.GetCollection<BsonDocument>("SECReportDB");
            //await collection.InsertOneAsync(document);

            using (var jsonReader = new JsonReader(jsonContent))
            {
                var serializer = new BsonArraySerializer();
                var rootReader = BsonDeserializationContext.CreateRoot(jsonReader);
                var bsonArray = serializer.Deserialize(rootReader);
                var collection = database.GetCollection<BsonDocument>("SECReportDB");
                await collection.InsertOneAsync(bsonArray.AsBsonDocument);
            }
        }
    }
}
