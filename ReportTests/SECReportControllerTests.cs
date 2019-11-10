
//namespace ReportTests
//{
//    using SECReportWeb.Controllers;
//    using System.Linq;
//    using NUnit.Framework;

//    [TestFixture]
//    public class SECReportControllerTests
//    {
//        [Test]
//        public void SearchFilingByTickerOrCompanyNameTest()
//        {
//            SECReportController c = new SECReportController();

//            string ticker = "ge";

//            var result = c.GetFiling(ticker);
//            Assert.NotNull(result);
//            Assert.AreEqual(result.Count(), 1);
//            Assert.AreEqual(result.FirstOrDefault().CompanyInfo.Ticker.ToLower(), ticker.ToLower());

//            ticker = "msft";
//            result = c.GetFiling(ticker);
//            Assert.NotNull(result);
//            Assert.AreEqual(result.Count(), 1);
//            Assert.AreEqual(result.FirstOrDefault().CompanyInfo.Ticker.ToLower(), ticker.ToLower());

//            string query = "microsoft";
//            result = c.GetFiling(query);
//            Assert.NotNull(result);
//            Assert.AreEqual(result.Count(), 1);
//            Assert.True(result.FirstOrDefault().CompanyInfo.Name.ToLower().Contains(query.ToLower()));

//            query = "foobar";
//            result = c.GetFiling(query);
//            Assert.AreEqual(result.Count(), 0);

//        }

//        [Test]
//        public void GetLastestFilingTest()
//        {
//            SECReportController c = new SECReportController();
//            var result = c.GetLatestCompanyFiling();
//            Assert.NotNull(result);
//        }
//    }
//}
