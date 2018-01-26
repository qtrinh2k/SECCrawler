﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SECCrawler;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Company")]
    public class CompanyController : Controller
    {
        const string _databaseName = "StockDB";
        const string _collectionName = "SECReport";
        const string _connectionString = "mongodb://localhost";

        // GET: api/SECReport
        [HttpGet("GetCompanies")]
        public IEnumerable<CIKInfo> GetCompanyInfo()
        {
            var database = GetMongoDatabase();
            var collection = database.GetCollection<SECFilingInfo>(_collectionName);
            var filterResults = collection
                .Find(x => x.CompanyInfo.Exchange.Equals("NYSE"))
                .ToListAsync()
                .Result;

            return filterResults.Select(x => x.CompanyInfo);
        }

        [HttpGet("GetByTicker")]
        public CIKInfo GetByTicker(string ticker)
        {
            var builder = Builders<BsonDocument>.Filter;

            var database = GetMongoDatabase();
            var collection = database.GetCollection<SECFilingInfo>(_collectionName);
            var filterResults = collection
                .Find(x => ticker.Equals(x.CompanyInfo.Ticker))
                .ToListAsync()
                .Result;

            return filterResults.FirstOrDefault().CompanyInfo;
        }
        [HttpGet("GetByCIK")]
        public CIKInfo GetByCIK(string cik)
        {
            var database = GetMongoDatabase();
            var collection = database.GetCollection<SECFilingInfo>(_collectionName);
            var filterResults = collection
                .Find(x => x.CompanyInfo.CIK == long.Parse(cik))
                .ToListAsync()
                .Result;

            return filterResults.FirstOrDefault().CompanyInfo;
        }

        // POST: api/SECReport
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/SECReport/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private IMongoDatabase GetMongoDatabase()
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase(_databaseName);

            return database;
        }
    }
}