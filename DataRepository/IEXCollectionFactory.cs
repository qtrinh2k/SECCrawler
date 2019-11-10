using System;
using System.Collections.Generic;
using System.Text;

namespace DataRepository
{
    public class IEXCollectionFactory
    {
        public static MongoDBRepository<T> CreateMongoDBCollection<T>(T collection, string collecionName)
            where T: class, new()
        {
            return new MongoDBRepository<T>(collecionName);
        }
    }
}
