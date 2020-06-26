using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhereDoYouWantToEat2.Models;
using WhereDoYouWantToEat2.Services.Interfaces;


namespace WhereDoYouWantToEat2.Tests.Mocks.Services
{
    public class MockUserProvider : Mock<IUserProvider>
    {
        public MockUserProvider MockGetUserId(string userId)
        {
            Setup(x => x.GetUserId())
                .Returns(userId);

            return this;
        }

        public MockUserProvider MockGetLoggedInUserAsync(User user)
        {
            Setup(x => x.GetLoggedInUserAsync())
                .Returns(Task.FromResult(user));

            return this;
        }

        public MockUserProvider MockGetByEmail(Task<User> user)
        {
            Setup(x => x.GetByEmail(It.IsAny<string>()))
                .Returns(user);

            return this;
        }
    }
}