using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Services.Interfaces;


namespace wheredoyouwanttoeat2.tests.Mocks.Services
{
    public class MockUserProvider : Mock<IUserProvider>
    {
        public MockUserProvider MockGetUserId(string userId)
        {
            Setup(x => x.GetUserId())
                .Returns(userId);

            return this;
        }

        public MockUserProvider MockGetLoggedInUserAsync(Task<User> user)
        {
            Setup(x => x.GetLoggedInUserAsync())
                .Returns(user);

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