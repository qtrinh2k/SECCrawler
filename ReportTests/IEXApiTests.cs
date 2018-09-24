
namespace ReportTests
{
    using IEXApiHandler;
    using IEXApiHandler.IEXData;
    using IEXApiHandler.IEXData.Stock;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class IEXApiTests
    {
        [Fact]
        public void GetReportTest()
        {
            IEXClientHandler response = new IEXClientHandler();

            var data = response.GetReport("ipar", Period.Annual);
            Assert.NotNull(data);
        }

        [Fact]
        public void GetKeyStatTest()
        {
            IEXClientHandler response = new IEXClientHandler();
            var data = response.GetKeyStats("ipar");
            Assert.NotNull(data);
        }

        [Fact]
        public void GetHistoricDataTest()
        {
            IEXClientHandler response = new IEXClientHandler();
            List<Chart> data = response.GetHistoricData("ipar", ChartOption._1m);
            Assert.NotNull(data);
            Assert.True(data.Count >= 1);
        }

        [Fact]
        public void GetQuoteTest()
        {
            IEXClientHandler response = new IEXClientHandler();
            var data = response.GetQuote("ipar");
            Assert.NotNull(data);
        }

        [Fact]
        public void GetCompanyTest()
        {
            IEXClientHandler response = new IEXClientHandler();
            var data = response.GetCompany("ipar");
            Assert.NotNull(data);
        }

        [Fact]
        public void GetBatchReportTest()
        {
            IEXClientHandler response = new IEXClientHandler();
            var data = response.GetBatchReport("ipar",
                                                new List<IEXDataType> { IEXDataType.quote, IEXDataType.news, IEXDataType.chart },
                                                ChartOption._1m,
                                                1);
            Assert.NotNull(data);
        }

        [Fact]
        public void GetBatchReportMultiSymbolsTest()
        {
            IEXClientHandler response = new IEXClientHandler();
            var data = response.GetBatchReport(new string[] { "ipar", "ge", "docu" }, 
                                                new List<IEXDataType> { IEXDataType.quote, IEXDataType.news, IEXDataType.chart }, 
                                                ChartOption._1m, 
                                                1);
            Assert.NotNull(data);
        }

        [Fact]
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
