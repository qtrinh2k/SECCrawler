using System;
using Xunit;
using WebApp.Controllers;
using SECReportWeb.Controllers;

namespace WebAppTests
{
    public class CompanyControllerTests
    {
        [Fact]
        public void Test1()
        {
            CompanyController controller = new CompanyController();

            var results = controller.GetCompanyInfo();
            Assert.NotNull(results);

            var tickerInfo = controller.GetByCIK("1090872");
            Assert.NotNull(tickerInfo);

            var result = controller.GetByTicker("AA");
            Assert.NotNull(result);
        }

        [Fact]
        public void Test2()
        {
            SECReportController controller = new SECReportController();
            var result = controller.GetCompanyInfo();
            Assert.NotNull(result);
        }
    }
}
