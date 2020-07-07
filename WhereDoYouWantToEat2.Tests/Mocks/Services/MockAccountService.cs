using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhereDoYouWantToEat2.Models;
using WhereDoYouWantToEat2.Services.Interfaces;

namespace WhereDoYouWantToEat2.Tests.Mocks.Services
{
    public class MockAccountService : Mock<IAccountService>
    {
        public MockAccountService MockRegisterUserAsync(bool isSuccessful)
        {
            Setup(x => x.RegisterUserAsync(It.IsAny<User>(), It.IsAny<string>()))
                .Returns(Task.FromResult(isSuccessful));

            return this;
        }

        public MockAccountService MockLoginUserAsync(bool isSuccessful)
        {
            Setup(x => x.LoginUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(isSuccessful));

            return this;
        }

        public MockAccountService MockUpdateUserProfileAsync(bool isSuccessful)
        {
            Setup(x => x.UpdateUserProfileAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(isSuccessful));

            return this;
        }

        public MockAccountService MockChangeUserPasswordAsync(bool isSuccessful)
        {
            Setup(x => x.ChangeUserPasswordAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(isSuccessful));

            return this;
        }

        public MockAccountService MockDeleteUserAccountAsync(string resultMessage)
        {
            Setup(x => x.DeleteUserAccountAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(resultMessage));

            return this;
        }

        public MockAccountService MockGetUserRestaurants(IEnumerable<Restaurant> restaurants)
        {
            Setup(x => x.GetUserRestaurants())
                .Returns(restaurants);

            return this;
        }
    }
}