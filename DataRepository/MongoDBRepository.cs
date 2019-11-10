using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace DataRepository
{
    public class MongoDBRepository<T>
        where T : class, new()
    {
        const string _connectionString = "mongodb://localhost";
        const string _databaseName = "StockDB";

        public MongoDBRepository(string collectionName)
        {
            ConnectionString = _connectionString;
            DatabaseName = _databaseName;
            CollectionName = collectionName;
        }

        public string ConnectionString { get; private set; }
        public string DatabaseName { get; private set; }
        public string CollectionName { get; set; }

        public IMongoDatabase MongoDbInstance
        {
            get
            {
                var client = new MongoClient(ConnectionString);
                var database = client.GetDatabase(DatabaseName);

                return database;
            }
        }

        public IMongoCollection<T> DbCollection
        {
            get
            {
                return MongoDbInstance.GetCollection<T>(CollectionName);
            }
        }


        public List<T> MongoGet(FilterDefinition<T> filter)
        {
            var results = this.DbCollection.Find(filter).ToListAsync().Result;
            return results;
        }

        public async Task MongoInsert(T item)
        {
            await this.DbCollection.InsertOneAsync(item);
        }

        //public async Task MongoUpset<T>(T item, Func<T>(FilterDefinition<T>, T) filter)
        //{
        //    var database = GetMongoDatabase();
        //    var collection = database.GetCollection<T>(this.CollectionName);

        //    UpdateOptions updateOp = new UpdateOptions
        //    {
        //        IsUpsert = true
        //    };
        //    var updateData = Builders<T>.Update.Set(x => x, item);

        //    await collection.FindOneAndUpdateAsync(filter.ToBsonDocument(), updateData.ToBsonDocument());
        //}

        public async Task MongoInsertMany(List<T> listData)
        {
            await this.DbCollection.InsertManyAsync(listData);
        }

    }
}
