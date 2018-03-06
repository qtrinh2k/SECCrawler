using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;

namespace DataRepository
{
    public class StockDBRepository : MongoDBRepository
    {
        const string _databaseName = "StockDB";
        const string _collectionName = "SECReport";
        const string _connectionString = "mongodb://localhost";

        public StockDBRepository():
            base(_connectionString, _databaseName, _collectionName)
        {            
        }

#region Read from database
        public List<SECFilingInfo> SearchByTerm(string query)
        {
            List<SECFilingInfo> filterResults = new List<SECFilingInfo>();

            if (string.IsNullOrEmpty(query))
                return null;

            if (long.TryParse(query, out _) &&
                GetByCIK(query) is SECFilingInfo result)
            {
                filterResults.Add(result);
                return filterResults;
            }

            var collection = this.GetMongoCollection<SECFilingInfo>();
            if (collection == null)
                throw new NullReferenceException("Collection is NULL");

            var findResults = collection.AsQueryable()
                                    .Where(x => x.CompanyInfo.Ticker.ToLower() == query.ToLower());
            if (!findResults.Any())
            {
                findResults = collection.AsQueryable()
                                    .Where(x => x.CompanyInfo.Name.ToLower().Contains((query.ToLower())));
            }
            return findResults.ToList();

        }

        public SECFilingInfo GetByCIK(string cik)
        {
            var collection = this.GetMongoCollection<SECFilingInfo>();

            List<SECFilingInfo> filterResults = collection
                .Find(x => x.CompanyInfo.CIK == long.Parse(cik))
                .ToListAsync()
                .Result;

            return filterResults?.FirstOrDefault();
        }

        public List<SECFilingInfo> GetLatestCompanyFiling(DateTime startDate)
        {
            var collection = this.GetMongoCollection<SECFilingInfo>();
            List<SECFilingInfo> latestFiling = new List<SECFilingInfo>();
            foreach(var result in collection.AsQueryable())
            {
                DateTime resultDate;
                if (result.Filings.Any(x => DateTime.TryParse(x.FilingDate, out resultDate) && (resultDate >= startDate)))
                {
                    var filings = result.Filings.Where(x => DateTime.TryParse(x.FilingDate, out resultDate) && (resultDate >= startDate));
                    latestFiling.Add(new SECFilingInfo
                    {
                        CompanyInfo = result.CompanyInfo,
                        Filings = filings.ToList()
                    });
                }
            }

            return latestFiling;
        }
        #endregion

        #region Write to database

        public async Task MongoInsert(List<SECFilingInfo> filingInfos)
        {
            var database = GetMongoDatabase();

            foreach (SECFilingInfo fi in filingInfos)
            {
                var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(fi));
                var collection = database.GetCollection<BsonDocument>(_collectionName);
                await collection.InsertOneAsync(document);
            }
        }

        public async Task MongoInsertJsonAsync(string jsonContent)
        {
            var database = GetMongoDatabase();

            using (var jsonReader = new MongoDB.Bson.IO.JsonReader(jsonContent))
            {
                var serializer = new MongoDB.Bson.Serialization.Serializers.BsonArraySerializer();
                var rootReader = BsonDeserializationContext.CreateRoot(jsonReader);
                var bsonArray = serializer.Deserialize(rootReader);
                var collection = database.GetCollection<BsonDocument>(_collectionName);
                await collection.InsertOneAsync(bsonArray.AsBsonDocument);
            }
        }
        #endregion


    }
}
