using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;

namespace DataRepository
{
    public class SECFilingCollection : MongoDBRepository<SECFilingInfo>
    {
        const string _collectionName = "SECReport";

        public SECFilingCollection():
            base(_collectionName)
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

            var collection = this.DbCollection;
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
            List<SECFilingInfo> filterResults = this.DbCollection
                .Find(x => x.CompanyInfo.CIK == long.Parse(cik))
                .ToListAsync()
                .Result;

            return filterResults?.FirstOrDefault();
        }

        public List<SECFilingInfo> GetLatestCompanyFiling(DateTime startDate)
        {

            List<SECFilingInfo> latestFiling = new List<SECFilingInfo>();
            foreach(var result in this.DbCollection.AsQueryable())
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
            foreach (SECFilingInfo fi in filingInfos)
            {
                //var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(fi));
                await this.DbCollection.InsertOneAsync(fi);
            }
        }
        #endregion

    }
}
