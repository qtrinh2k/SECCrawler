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
        [HttpGet("GetCompanies")]
        public IEnumerable<CIKInfo> GetCompanyInfo()
        {
            var database = GetMongoDatabase();
            var collection = database.GetCollection<SECFilingInfo>(_collectionName);
            var filterResults = collection
                .Find(x => x.CompanyInfo.Exchange.Equals("NYSE"))
                .ToListAsync()
                .Result;

            return filterResults.Take(10).Select(x => x.CompanyInfo);
        }

        [HttpGet("GetFiling")]
        public IEnumerable<SECFilingInfo> GetFiling(string q)
        {
            List<SECFilingInfo> filterResults = new List<SECFilingInfo>();

            if (long.TryParse(q, out _) &&
                GetByCIK(q) is SECFilingInfo result)
            {
                filterResults.Add(result);
                return filterResults;
            }

            var database = GetMongoDatabase();
            var collection = database.GetCollection<SECFilingInfo>(_collectionName);
            filterResults = collection
                .Find(x => (q.Equals(x.CompanyInfo.Ticker) ||
                             x.CompanyInfo.Name.Contains(q)))
                .ToListAsync()
                .Result;
            
            return filterResults;
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

            return filterResults?.FirstOrDefault()?.CompanyInfo;
        }

        [HttpGet("SearchStock")]
        public IEnumerable<string> SearchStock(string term)
        {           
            List<SECFilingInfo> filterResults = new List<SECFilingInfo>();

            var database = GetMongoDatabase();
            var collection = database.GetCollection<SECFilingInfo>(_collectionName);
            filterResults = collection
                .Find(x => (term.Equals(x.CompanyInfo.Ticker) ||
                             x.CompanyInfo.Name.Contains(term)))
                .ToListAsync()
                .Result;

            var results = filterResults.Select(x => string.Format($"{x.CompanyInfo.Ticker} -{x.CompanyInfo.Name}"));
            return results;
        }

        [HttpGet("GetByCIK")]
        public SECFilingInfo GetByCIK(string cik)
        {
            var database = GetMongoDatabase();
            var collection = database.GetCollection<SECFilingInfo>(_collectionName);
            List<SECFilingInfo> filterResults = collection
                .Find(x => x.CompanyInfo.CIK == long.Parse(cik))
                .ToListAsync()
                .Result;
            //return filterResults.Any() ? filterResults.FirstOrDefault().CompanyInfo : null;
            return filterResults?.FirstOrDefault();
        }

        public SECFilingInfo GetFilingByTicker(string ticker)
        {
            var database = GetMongoDatabase();
            var collection = database.GetCollection<SECFilingInfo>(_collectionName);
            var filterResults = collection
                .Find(x => ticker.Equals(x.CompanyInfo.Ticker))
                .ToListAsync()
                .Result;

            return filterResults?.FirstOrDefault();
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
