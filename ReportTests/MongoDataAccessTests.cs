using DataRepository;
using IEXApiHandler.IEXData.Stock;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ReportTests
{

    [TestFixture]
    public class MongoDataAccessTests
    {
        [Test]
        public void GetIEXData()
        {
            var comp = new MongoDBRepository<Company>("Company");
            string symbol = "GE";
            var filter = Builders<Company>.Filter.Eq(x => x.symbol, symbol);
            var result = comp.MongoGet(filter);

            Assert.NotNull(result);
            
        }
    }
}
