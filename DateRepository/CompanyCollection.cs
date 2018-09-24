namespace DataRepository
{
    using IEXApiHandler.IEXData.Stock;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public class CompanyCollection : MongoDBRepository<Company>
    {
        const string _collectionName = "Company";

        public CompanyCollection()
            : base(_collectionName)
        {
        }

        public async Task MongoUpsert(Company company)
        {
            var collection = MongoDbInstance.GetCollection<BsonDocument>(_collectionName);
            var filter = Builders<Company>.Filter.Eq(x => x.symbol, company.symbol);

            UpdateOptions updateOp = new UpdateOptions
            {
                IsUpsert = true
            };
            var updateData = Builders<Company>.Update.Set(x => x.description, company.description);

            await collection.FindOneAndUpdateAsync(filter.ToBsonDocument(), updateData.ToBsonDocument());            
        }

        public Dictionary<string, Guid> GetCompanyId(List<string> symbols)
        {
            Dictionary<string, Guid> dictSymbolCompId = new Dictionary<string, Guid>();
            foreach (var result in DbCollection.AsQueryable())
            {
                if(!dictSymbolCompId.ContainsKey(result.symbol))
                {
                    dictSymbolCompId.Add(result.symbol, result._id);
                }                                   
            }
            return dictSymbolCompId;
        }

    }
}
