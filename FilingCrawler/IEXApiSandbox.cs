using DataRepository;
using IEXApiHandler;
using IEXApiHandler.IEXData;
using IEXApiHandler.IEXData.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilingCrawler
{
    /// <summary>
    /// playground for IEX api
    /// </summary>
    public class IEXApiSandbox
    {
        internal static void InsertCompanyData(IEnumerable<CIKInfo> cikInfos)
        {
            int total = cikInfos.Count();
            int count = 0;
            int batchCount = 50;
            do
            {
                var symbols = cikInfos.Select(x => x.Ticker).Skip(count).Take(batchCount).ToArray();
                count += symbols.Count();

                //get data
                IEXClientHandler response = new IEXClientHandler();
                var results = response.GetBatchReport(symbols, new List<IEXDataType> { IEXDataType.company }, IEXApiHandler.IEXData.Stock.ChartOption._1d, 1);

                //insert to db
                var comp = new MongoDBRepository<Company>("Company");
                //repo.MongoInsert(results.Values.Select(x => x.Company).Distinct().ToList()).Wait();
                comp.MongoInsertMany(results.Values.Select(x => x.Company).Distinct().ToList()).Wait();

            } while (count < total);

        }
        internal static void InsertChartData(IEnumerable<CIKInfo> cikInfos)
        {
            
            IEXClientHandler response = new IEXClientHandler();
            CompanyCollection compColl = new CompanyCollection();

            //symbols from list
            var symbols = cikInfos.Select(x => x.Ticker).ToList();

            //checking for company exists in db
            var dictComp = compColl.GetCompanyId(symbols);

            //insert to db
            var chartCollection = new MongoDBRepository<Chart>("Charts");

            foreach (string symbol in dictComp.Keys)
            {
                try
                {
                    var results = response.GetBatchReport(symbol, new List<IEXDataType> { IEXDataType.chart }, ChartOption._5y, 100);

                    if (!dictComp.ContainsKey(symbol))
                    {
                        Console.WriteLine($"Not data for symbol {symbol}.");
                        continue;
                    }

                    //update charts with companyId
                    Parallel.For (0, results.Charts.Count, (i) =>
                    {
                        results.Charts[i].companyId = dictComp[symbol];
                    });

                    chartCollection.MongoInsertMany(results.Charts).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        internal static void InsertIEXData(IEnumerable<CIKInfo> cikInfos, List<IEXDataType> listIexDataTypes)
        {
            int total = cikInfos.Count();
            int count = 0;
            int batchCount = 50;
            CompanyCollection companyRepo = new CompanyCollection();

            do
            {
                var symbols = cikInfos.Select(x => x.Ticker).Skip(count).Take(batchCount).ToArray();
                count += symbols.Count();

                var dictSymbolComp = companyRepo.GetCompanyId(symbols.ToList());
                //get data
                IEXClientHandler response = new IEXClientHandler();
                var results = response.GetBatchReport(symbols, listIexDataTypes, ChartOption._1d, 1);

                //insert to db
                var comp = new MongoDBRepository<Company>("Company");
                var stat = new MongoDBRepository<Stat>("IEXStat");

                foreach (string symbol in results.Keys)
                {
                    Guid compId = Guid.NewGuid(); //ObjectId.GenerateNewId().ToString();
                    results[symbol].Company._id = compId;
                    results[symbol].Stat._id = Guid.NewGuid();
                    results[symbol].Stat.companyId = compId;

                    comp.MongoInsert(results[symbol].Company).Wait();
                    stat.MongoInsert(results[symbol].Stat).Wait();
                }

            } while (count < total);

        }


    }
}
