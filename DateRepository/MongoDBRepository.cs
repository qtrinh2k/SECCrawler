using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

namespace DataRepository
{
    public class MongoDBRepository
    {    
        public MongoDBRepository(string connString, string databaseName, string collectionName)
        {
            ConnectionString = connString;
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }

        public IMongoDatabase GetMongoDatabase()
        {
            var client = new MongoClient(ConnectionString);
            var database = client.GetDatabase(DatabaseName);

            return database;
        }

        public IMongoCollection<T> GetMongoCollection<T>()
        {
            return GetMongoDatabase().GetCollection<T>(CollectionName);
        }
    }
}
