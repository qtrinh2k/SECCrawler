using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SECCrawler;

namespace SECReportWeb.Controllers
{
    [Route("api/[controller]")]
    public class SECReportController : Controller
    {
        const string _databaseName = "StockDB";
        const string _collectionName = "SECReport";
        const string _connectionString = "mongodb://localhost";

        // GET: api/SECReport
        [HttpGet("GetCompanyInfo")]
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
        public CIKInfo GetCompanyInfoByTicker(string ticker)
        {
            var database = GetMongoDatabase();
            var collection = database.GetCollection<SECFilingInfo>(_collectionName);
            var filterResults = collection
                .Find(x => x.CompanyInfo.Ticker.Equals(ticker.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToListAsync()
                .Result;

            return filterResults.FirstOrDefault().CompanyInfo;
        }

        //// GET: api/SECReport/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}
        
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
