using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;

namespace SECCrawler
{

    public class DataRepository
    {
        const string _DatabaseName = "StockDB";
        const string _CollectionName = "SECReport";
        const string _connectionString = "mongodb://localhost";

        public async Task MongoInsert(List<SECFilingInfo> filingInfos)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_DatabaseName);

            foreach (SECFilingInfo fi in filingInfos)
            {
                var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(fi));
                var collection = database.GetCollection<BsonDocument>(_CollectionName);

                await collection.InsertOneAsync(document);
            }
        }

        public async Task MongoInsertJsonAsync(string jsonContent)
        {
            var connectionString = "mongodb://localhost";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(_DatabaseName);

            //var document = BsonSerializer.Deserialize<BsonDocument>(jsonContent);
            //var collection = database.GetCollection<BsonDocument>("SECReportDB");
            //await collection.InsertOneAsync(document);

            using (var jsonReader = new MongoDB.Bson.IO.JsonReader(jsonContent))
            {
                var serializer = new MongoDB.Bson.Serialization.Serializers.BsonArraySerializer();
                var rootReader = BsonDeserializationContext.CreateRoot(jsonReader);
                var bsonArray = serializer.Deserialize(rootReader);
                var collection = database.GetCollection<BsonDocument>(_CollectionName);
                await collection.InsertOneAsync(bsonArray.AsBsonDocument);
            }
        }
    }
}
