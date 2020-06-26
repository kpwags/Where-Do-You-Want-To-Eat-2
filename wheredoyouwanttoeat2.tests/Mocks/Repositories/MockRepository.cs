using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using WhereDoYouWantToEat2.Respositories.Interfaces;

namespace WhereDoYouWantToEat2.Tests.Mocks.Repositories
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

        public MockRepository<T> MockGetById(T result)
        {
            Setup(x => x.GetById(It.IsAny<object>()))
                .Returns(result);

            return this;
        }

        public MockRepository<T> MockAdd(T result)
        {
            Setup(x => x.Add(It.IsAny<T>()));

            return this;
        }

        public MockRepository<T> MockUpdate(T result)
        {
            Setup(x => x.Update(It.IsAny<T>()));

            return this;
        }

        public MockRepository<T> MockDelete(T result)
        {
            Setup(x => x.Delete(It.IsAny<T>()));

            return this;
        }
    }
}