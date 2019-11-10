namespace ReportTests
{
    using DataRepository;
    using IEXApiHandler;
    using IEXApiHandler.IEXData;
    using IEXApiHandler.IEXData.Stock;
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class ProcessDataTests
    {
        [Test]
        public void BatchProcessSymbolsTest()
        {
            //getting list of company
            string[] symbols = new string[] { "ge", "appl", "msft", "ipar", "gntx" };
            //get data
            IEXClientHandler response = new IEXClientHandler();
            var results = response.GetBatchReport(symbols, new List<IEXDataType> { IEXDataType.company }, IEXApiHandler.IEXData.Stock.ChartOption._1d, 1);
            
            Assert.NotNull(results);
            //insert to db
            CompanyCollection repo = new CompanyCollection();

            repo.MongoInsertMany(results.Values.Select(x => x.Company).Distinct().ToList()).Wait();
        }

        [Test]
        public void BatchProcessSymbolsManyAsyncTest()
        {
            //getting list of company
            string[] symbols = new string[] { "ge", "appl", "msft", "ipar", "gntx" };
            //get data
            IEXClientHandler response = new IEXClientHandler();
            var results = response.GetBatchReport(symbols, new List<IEXDataType> { IEXDataType.company }, IEXApiHandler.IEXData.Stock.ChartOption._1d, 1);

            Assert.NotNull(results);
            //insert to db
            CompanyCollection repo = new DataRepository.CompanyCollection();

            //repo.MongoInsert(results.Values.Select(x => x.Company).Distinct().ToList()).Wait();
            repo.MongoInsertMany(results.Values.Select(x => x.Company).Distinct().ToList()).Wait();
        }


        [Test]
        public void UpdateDataTest()
        {
            //insert to db
            CompanyCollection repo = new CompanyCollection();
            var collection = repo.DbCollection;
            var results = collection.Find<Company>(x => x.symbol.ToLower() == "ge", null)
            .ToListAsync()
            .Result;

            var company = results.First();
            company.description = "TEST UPDATE";
            repo.MongoUpsert(company).Wait();
        }

        [Test]
        public void GetCompanyInfoTest()
        {
            CompanyCollection repo = new CompanyCollection();
            var collection = repo.DbCollection;
            Assert.NotNull(collection);

            var results = collection.Find<Company>(x => x.symbol != null, null)
                .ToListAsync()
                .Result;

            Assert.NotNull(results);
        }
    }
}
