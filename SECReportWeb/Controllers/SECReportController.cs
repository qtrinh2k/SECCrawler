using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using DataRepository;
using System.Diagnostics.Tracing;

namespace SECReportWeb.Controllers
{
    [Route("api/[controller]")]
    public class SECReportController : Controller
    {
        const string _databaseName = "StockDB";
        const string _collectionName = "SECReport";
        const string _connectionString = "mongodb://localhost";
        StockDBRepository _stockDBRepository = null;

        public SECReportController()
        {
            _stockDBRepository = new StockDBRepository();
        }
        // GET: api/SECReport
        [HttpGet("GetCompanies")]
        public IActionResult GetCompanyInfo()
        {
            var collection = _stockDBRepository.GetMongoCollection<SECFilingInfo>();

            var filterResults = collection
                .Find(x => x.CompanyInfo.Exchange.Equals("NYSE"))
                .ToListAsync()
                .Result;

            return Ok(filterResults.Take(10).Select(x => x.CompanyInfo));
        }

        [HttpGet("GetFiling")]
        public List<SECFilingInfo> GetFiling(string query)
        {
            return _stockDBRepository.SearchByTerm(query);
        }

        [HttpGet("GetByTicker")]
        public CIKInfo GetByTicker(string ticker)
        {
            var builder = Builders<BsonDocument>.Filter;

            var collection = _stockDBRepository.GetMongoCollection<SECFilingInfo>();
            var filterResults = collection
                .Find(x => ticker.Equals(x.CompanyInfo.Ticker))
                .ToListAsync()
                .Result;

            return filterResults?.FirstOrDefault()?.CompanyInfo;
        }

        [HttpGet("GetLatestCompanyFiling")]
        public IActionResult GetLatestCompanyFiling()
        {
            DateTime fromDate = DateTime.Now.AddMonths(-3);
            try
            {
                return base.Ok(_stockDBRepository.GetLatestCompanyFiling(fromDate).Take(20));
            }
            catch
            {
                return BadRequest("NO RESULT FOUND!!!");
            }
        }


        [HttpGet("SearchStock")]
        public IActionResult SearchStock(string term)
        {           
            List<SECFilingInfo> filterResults = new List<SECFilingInfo>();

            var collection = _stockDBRepository.GetMongoCollection<SECFilingInfo>();
            filterResults = collection
                .Find(x => (term.Equals(x.CompanyInfo.Ticker) ||
                             x.CompanyInfo.Name.Contains(term)))
                .ToListAsync()
                .Result;

            return Ok(filterResults.Select(x => string.Format($"{x.CompanyInfo.Ticker} -{x.CompanyInfo.Name}")));
        }

        [HttpGet("GetByCIK")]
        public SECFilingInfo GetByCIK(string cik)
        {
            var collection = _stockDBRepository.GetMongoCollection<SECFilingInfo>();
            List<SECFilingInfo> filterResults = collection
                .Find(x => x.CompanyInfo.CIK == long.Parse(cik))
                .ToListAsync()
                .Result;
            //return filterResults.Any() ? filterResults.FirstOrDefault().CompanyInfo : null;
            return filterResults?.FirstOrDefault();
        }

        public SECFilingInfo GetFilingByTicker(string ticker)
        {
            var collection = _stockDBRepository.GetMongoCollection<SECFilingInfo>();
            var filterResults = collection
                .Find(x => ticker.Equals(x.CompanyInfo.Ticker))
                .ToListAsync()
                .Result;

            return filterResults?.FirstOrDefault();
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
    }
}
