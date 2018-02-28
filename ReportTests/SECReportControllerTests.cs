using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SECReportWeb.Controllers;

namespace ReportTests
{
    public class SECReportControllerTests
    {
        [Fact]
        public void SearchFilingByTickerOrCompanyNameTest()
        {
            SECReportController c = new SECReportController();

            string ticker = "ge";

            var result = c.GetFiling(ticker);
            Assert.NotNull(result);
            Assert.Equal(result.Count(), 1);
            Assert.Equal(result.FirstOrDefault().CompanyInfo.Ticker.ToLower(), ticker.ToLower());

            ticker = "msft";
            result = c.GetFiling(ticker);
            Assert.NotNull(result);
            Assert.Equal(result.Count(), 1);
            Assert.Equal(result.FirstOrDefault().CompanyInfo.Ticker.ToLower(), ticker.ToLower());

            string query = "microsoft";
            result = c.GetFiling(query);
            Assert.NotNull(result);
            Assert.Equal(result.Count(), 1);
            Assert.True(result.FirstOrDefault().CompanyInfo.Name.ToLower().Contains(query.ToLower()));

            query = "foobar";
            result = c.GetFiling(query);
            Assert.Null(result);
            Assert.Equal(result.Count(), 0);

        }
    }
}
