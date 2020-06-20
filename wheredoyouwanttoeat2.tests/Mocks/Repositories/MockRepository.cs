using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using wheredoyouwanttoeat2.Respositories.Interfaces;

namespace wheredoyouwanttoeat2.tests.Mocks.Repositories
{
    public class MockRepository<T> : Mock<IRepository<T>> where T : class
    {
        public MockRepository<T> MockGetAll(IQueryable<T> result)
        {
            Setup(x => x.GetAll())
                .Returns(result);

            return this;
        }

        public MockRepository<T> MockGet(IQueryable<T> result)
        {
            Setup(x => x.Get(It.IsAny<Expression<Func<T, bool>>>()))
                .Returns(result);

            return this;
        }

        public MockRepository<T> MockAdd(T result)
        {
            Setup(x => x.Add(It.IsAny<T>()));

            return this;
        }
    }
}