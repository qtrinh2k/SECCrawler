using DataRepository;
using IEXApiHandler.IEXData.Stock;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Linq;
using MongoDB.Bson;

namespace ReportTests
{
    [TestFixture]
    public class StockScreenerTests
    {
        [Test]
        public void GetCompanyByMarketCapTest()
        {
            var compRepo = new MongoDBRepository<Company>(nameof(Company));
            CompanyCollection companyCollection = new CompanyCollection();

            var statRepo = new MongoDBRepository<Stat>("IEXStat");
            var finRepo = new MongoDBRepository<Financial>(nameof(Financial));

            long minCap = 1_000_000;
            long maxCap = 2_000_000_000;

            var filter = Builders<Stat>.Filter.Lte(x => x.Marketcap, maxCap) &
                         Builders<Stat>.Filter.Gte(x => x.Marketcap, minCap);

            var results = statRepo.DbCollection.FindAsync(filter).Result.ToList();
            
            Assert.NotNull(results);

            results.ForEach((x) =>
            {
                Console.WriteLine(x.Marketcap);
                Assert.True(x.Marketcap <= maxCap, $"Actual={x.Marketcap} <= Expected={maxCap}");
            });

            var companies = companyCollection.GetCompanyId(results.Select(x => x.Symbol).ToList());

            Assert.AreEqual(companies.Count, results.Count);
        }

        [Test]
        public void GetCompanyFinancialAggregateTest()
        {
            long minCap = 1_000_000;
            long maxCap = 2_000_000_000;
            var statRepo = new MongoDBRepository<Stat>("IEXStat").DbCollection.AsQueryable();
            var compRepo = new MongoDBRepository<Company>(nameof(Company)).DbCollection.AsQueryable();
            var result = (from s in statRepo
                         join c in compRepo on s.companyId equals c._id //into joined
                         where s.Marketcap >= minCap && s.Marketcap <= maxCap
                         select new { s.companyId, c.companyName}).ToList();

            Assert.NotNull(result);            
        }

        [Test]
        public void GetCompanyRevenueGrowth10Pct()
        {
            var compRepo = new MongoDBRepository<Company>(nameof(Company)).DbCollection;
            var histColl = new MongoDBRepository<Financial>(nameof(Financial)).DbCollection;
            var results = histColl.Aggregate().Group(key => key.companyId,
                value => new {
                    CompanyId = value.Select(x => x.companyId).First(),
                    Revenue = value.Sum(x => x.totalRevenue),
                    FirstYear = value.First().totalRevenue,
                    LastYear = value.Last().totalRevenue,
                    YearCount = value.Count()
                }).ToList().Distinct();

            foreach(var result in results)
            {
                double growthRate = ((double)(result.FirstYear - result.LastYear) / result.LastYear) * 100 / result.YearCount;
                var filter = Builders<Company>.Filter.Eq("_id", result.CompanyId);
                var comp = compRepo.Find(filter).ToListAsync().Result;

                if (growthRate > 10 && comp.Count > 0)
                {
                    Console.WriteLine($"{comp.First().symbol}, {result.Revenue}, {result.FirstYear}, {result.LastYear}, {growthRate}");
                }                
            }
            Assert.NotNull(results);
        }

        [Test]
        public void FindIncomeGrowthTest()
        {
            var compRepo = new MongoDBRepository<Company>(nameof(Company)).DbCollection;
            var histColl = new MongoDBRepository<Financial>(nameof(Financial)).DbCollection;
            var results = histColl.Aggregate().Group(key => key.companyId,
                value => new {
                    CompanyId = value.Select(x => x.companyId).First(),
                    TotalIncome = value.Sum(x => x.netIncome).Value,
                    FirstYear = value.First().netIncome.Value,
                    LastYear = value.Last().netIncome.Value,
                    YearCount = value.Count()
                }).ToList().Distinct();

            foreach (var result in results)
            {
                double growthRate = ((double)(result.FirstYear - result.LastYear) / result.LastYear) * 100 / result.YearCount;
                var filter = Builders<Company>.Filter.Eq("_id", result.CompanyId);
                var comp = compRepo.Find(filter).ToListAsync().Result;

                if (growthRate > 10 && comp.Count > 0)
                {
                    Console.WriteLine($"{comp.First().symbol}, {result.TotalIncome}, {result.FirstYear}, {result.LastYear}, {growthRate}");
                }
            }
        }

        [Test]
        public void QueryTest()
        {
            var compRepo = new MongoDBRepository<Company>(nameof(Company)).DbCollection;
            var filter = Builders<Company>.Filter.Eq("_id", ObjectId.Parse("5bd6920d9656412594cf7657"));
            //var filter = Builders<Company>.Filter.Eq("symbol", "GE");
            var result = compRepo.FindAsync(filter).Result.First();
            Assert.AreEqual(result.symbol, "GE");
            Console.WriteLine($"Id:{result._id}, Comp:{result.companyName}");
        }
    }
}
