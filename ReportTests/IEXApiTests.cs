
namespace ReportTests
{
    using IEXApiHandler;
    using IEXApiHandler.IEXData;
    using IEXApiHandler.IEXData.Stock;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    public class IEXApiTests
    {
        readonly string _testSymbol = "IPAR";
        readonly IEXClientHandler _clientHandler;

        public IEXApiTests()
        {
            _clientHandler = new IEXClientHandler();
        }

        /// <summary>
        /// 11/10/2019 - GetReport only works with pay acount.
        /// </summary>
        [Ignore("Need to have pay account for GetReport to work")]
        [Test]
        public void GetFinancialReportTest()
        {
            var data = this._clientHandler.GetFinancialReport("ipar", Period.Annual);
            Assert.NotNull(data);
        }

        [Ignore("Need to have pay account for GetReport to work")]
        [Test]
        public void GetFinancialReportBatchTest()
        {
            var tickers = new string[] { "ipar", "ge", "docu" };
            var data = this._clientHandler.GetBatchFinancials(tickers, Period.Annual);

            Assert.NotNull(data);
        }

        [Test]
        public void GetKeyStatTest()
        {
            IEXClientHandler response = new IEXClientHandler();
            var data = this._clientHandler.GetKeyStats(this._testSymbol);
            Assert.NotNull(data);
            Assert.IsTrue(data.SharesOutstanding > 10_000);
        }

        [Test]
        public void GetHistoricDataTest()
        {
            IEXClientHandler response = new IEXClientHandler();
            List<Chart> data = this._clientHandler.GetHistoricData(this._testSymbol, ChartOption._1m);
            Assert.NotNull(data);
            Assert.True(data.Count >= 1);
        }

        [Test]
        public void GetQuoteTest()
        {
            IEXClientHandler response = new IEXClientHandler();
            var data = this._clientHandler.GetQuote(this._testSymbol);
            Assert.NotNull(data);
            Assert.IsTrue(data.latestPrice > 0);
        }

        [Test]
        public void GetCompanyTest()
        {
            IEXClientHandler response = new IEXClientHandler();
            var data = this._clientHandler.GetCompany(this._testSymbol);
            Assert.NotNull(data);
            Assert.IsTrue(!string.IsNullOrEmpty(data.companyName));
        }

        [Test]
        public void GetBatchReportTest()
        {
            IEXClientHandler response = new IEXClientHandler();
            var data = this._clientHandler.GetBatchReport(this._testSymbol,
                                                new List<IEXDataType> { IEXDataType.quote, IEXDataType.news, IEXDataType.chart },
                                                ChartOption._1m,
                                                1);
            Assert.NotNull(data);
            Assert.NotNull(data.Quote);
            Assert.NotNull(data.NewsList);
            Assert.NotNull(data.Charts);
        }

        [Test]
        public void GetBatchReportMultiSymbolsTest()
        {
            IEXClientHandler response = new IEXClientHandler();
            var tickers = new string[] { "ipar", "ge", "docu" };
            var data = this._clientHandler.GetBatchReport(tickers, 
                                                new List<IEXDataType> { IEXDataType.quote, IEXDataType.news, IEXDataType.chart }, 
                                                ChartOption._1m, 
                                                1);
            Assert.NotNull(data);
            Assert.AreEqual(data.Count, tickers.Length);
        }

        [Test]
        public void CovertBatchReportTest()
        {
            Dictionary<string, BatchReport> dictReports = new Dictionary<string, BatchReport>();

            BatchReport docuReport = new BatchReport();
            docuReport.Quote = new Quote
            {
                companyName = "Docusign",
                close = 10,
                latestTime = DateTime.Now.ToLongTimeString(),
            };
            docuReport.Charts = new List<Chart>
            {
                new Chart
                {
                    average = 11,
                    date = DateTime.Now.ToLongTimeString()
                },
                new Chart
                {
                    average = 8,
                    date = DateTime.Now.AddDays(-2).ToLongTimeString()
                }
            };
            docuReport.NewsList = new List<News>
            {
                new News
                {
                    CreatedDate = DateTime.Now,
                    Url = "http://www.google.com"
                },
                new News
                {
                    CreatedDate = DateTime.Now,
                    Url = "http://www.docusign.com"
                }

            };

            dictReports.Add("DOCU", docuReport);

            BatchReport geReport = new BatchReport();
            geReport.Quote = new Quote
            {
                companyName = "Docusign",
                close = 10,
                latestTime = DateTime.Now.ToLongTimeString(),

            };
            geReport.Charts = new List<Chart>
            {
                new Chart
                {
                    average = 11,
                    date = DateTime.Now.ToLongTimeString()
                },
                new Chart
                {
                    average = 8,
                    date = DateTime.Now.AddDays(-2).ToLongTimeString()
                }
            };
            geReport.NewsList = new List<News>
            {
                new News
                {
                    CreatedDate = DateTime.Now,
                    Url = "http://www.google.com"
                },
                new News
                {
                    CreatedDate = DateTime.Now,
                    Url = "http://www.docusign.com"
                }

            };

            dictReports.Add("GE", geReport);
            string jsonStr = JsonConvert.SerializeObject(dictReports);
        }                
    }
}
