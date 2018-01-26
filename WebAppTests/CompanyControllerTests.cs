using System;
using Xunit;
using WebApp.Controllers;

namespace WebAppTests
{
    public class CompanyControllerTests
    {
        [Fact]
        public void Test1()
        {
            CompanyController controller = new CompanyController();
            //var results = controller.GetCompanyInfo();
            //Assert.NotNull(results);
            //var tickerInfo = controller.GetByCIK("1090872");
            //Assert.NotNull(tickerInfo);

            var result = controller.GetCompanyInfoByTicker("AA");
            Assert.NotNull(result);
        }
    }
}
