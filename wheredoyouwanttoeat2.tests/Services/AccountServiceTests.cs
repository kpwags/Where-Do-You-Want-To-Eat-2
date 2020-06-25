using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Respositories.Interfaces;
using wheredoyouwanttoeat2.Services;
using wheredoyouwanttoeat2.Services.Interfaces;
using wheredoyouwanttoeat2.tests.Mocks.Services;
using Xunit;

namespace wheredoyouwanttoeat2.tests.Services
{
    public class AccountServiceTests
    {
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<SignInManager<User>> _mockSignInManager;

        public AccountServiceTests()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<User>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<User>>().Object);
        }

        [Fact]
        public void AccountService_RegisterUserAsync_ReturnsResult()
        {
            var newUser = new User
            {
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var mockRestaurantRepo = Mock.Of<IRepository<Restaurant>>();
            var mockRestaurantTagRepo = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepo = Mock.Of<IRepository<Tag>>();
            var mockUserProvider = Mock.Of<IUserProvider>();

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockSignInManager.Setup(x => x.SignInAsync(It.IsAny<User>(), It.IsAny<AuthenticationProperties>(), It.IsAny<string>()));

            var accountService = new AccountService(_mockUserManager.Object, _mockSignInManager.Object, mockRestaurantRepo, mockRestaurantTagRepo, mockTagRepo, mockUserProvider);

            bool successfulRegistration = accountService.RegisterUserAsync(newUser, "Password123").Result;

            Assert.True(successfulRegistration);
        }

        [Fact]
        public void AccountService_RegisterUserAsync_ReturnsError()
        {
            var newUser = new User
            {
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var mockRestaurantRepo = Mock.Of<IRepository<Restaurant>>();
            var mockRestaurantTagRepo = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepo = Mock.Of<IRepository<Tag>>();
            var mockUserProvider = Mock.Of<IUserProvider>();

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            _mockSignInManager.Setup(x => x.SignInAsync(It.IsAny<User>(), It.IsAny<AuthenticationProperties>(), It.IsAny<string>()));

            var accountService = new AccountService(_mockUserManager.Object, _mockSignInManager.Object, mockRestaurantRepo, mockRestaurantTagRepo, mockTagRepo, mockUserProvider);

            bool failedRegistration = accountService.RegisterUserAsync(newUser, "Password123").Result;

            Assert.False(failedRegistration);
        }

        [Fact]
        public void AccountService_LoginUserAsync_SuccessfulLogin()
        {
            var loggingInUser = new User
            {
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var mockRestaurantRepo = Mock.Of<IRepository<Restaurant>>();
            var mockRestaurantTagRepo = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepo = Mock.Of<IRepository<Tag>>();
            var mockUserProvider = Mock.Of<IUserProvider>();

            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(loggingInUser);

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            var accountService = new AccountService(_mockUserManager.Object, _mockSignInManager.Object, mockRestaurantRepo, mockRestaurantTagRepo, mockTagRepo, mockUserProvider);

            bool successfulRegistration = accountService.LoginUserAsync("test@wheredoyouwanttoeat.xyz", "Password123").Result;

            Assert.True(successfulRegistration);
        }

        [Fact]
        public void AccountService_LoginUserAsync_FailedLoginUserNotFound()
        {
            var loggingInUser = new User
            {
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var mockRestaurantRepo = Mock.Of<IRepository<Restaurant>>();
            var mockRestaurantTagRepo = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepo = Mock.Of<IRepository<Tag>>();
            var mockUserProvider = Mock.Of<IUserProvider>();

            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            var accountService = new AccountService(_mockUserManager.Object, _mockSignInManager.Object, mockRestaurantRepo, mockRestaurantTagRepo, mockTagRepo, mockUserProvider);

            bool failedLogin = accountService.LoginUserAsync("test@wheredoyouwanttoeat.xyz", "Password123").Result;

            Assert.False(failedLogin);
        }

        [Fact]
        public void AccountService_LoginUserAsync_FailedLoginUserBadPassword()
        {
            var loggingInUser = new User
            {
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var mockRestaurantRepo = Mock.Of<IRepository<Restaurant>>();
            var mockRestaurantTagRepo = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepo = Mock.Of<IRepository<Tag>>();
            var mockUserProvider = Mock.Of<IUserProvider>();

            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(loggingInUser);

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            var accountService = new AccountService(_mockUserManager.Object, _mockSignInManager.Object, mockRestaurantRepo, mockRestaurantTagRepo, mockTagRepo, mockUserProvider);

            bool failedLogin = accountService.LoginUserAsync("test@wheredoyouwanttoeat.xyz", "Password123").Result;

            Assert.False(failedLogin);
        }

        [Fact]
        public void AccountService_ChangeUserPasswordAsync_SuccessfulChange()
        {
            var loggedInUser = new User
            {
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var mockRestaurantRepo = Mock.Of<IRepository<Restaurant>>();
            var mockRestaurantTagRepo = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepo = Mock.Of<IRepository<Tag>>();
            var mockUserProvider = new MockUserProvider().MockGetLoggedInUserAsync(loggedInUser);

            _mockUserManager.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var accountService = new AccountService(_mockUserManager.Object, _mockSignInManager.Object, mockRestaurantRepo, mockRestaurantTagRepo, mockTagRepo, mockUserProvider.Object);

            bool successfulChange = accountService.ChangeUserPasswordAsync("Password123_1", "Password123_2").Result;

            Assert.True(successfulChange);
        }

        [Fact]
        public void AccountService_ChangeUserPasswordAsync_UnsuccessfulChange()
        {
            var loggedInUser = new User
            {
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var mockRestaurantRepo = Mock.Of<IRepository<Restaurant>>();
            var mockRestaurantTagRepo = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepo = Mock.Of<IRepository<Tag>>();
            var mockUserProvider = new MockUserProvider().MockGetLoggedInUserAsync(null);

            _mockUserManager.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var accountService = new AccountService(_mockUserManager.Object, _mockSignInManager.Object, mockRestaurantRepo, mockRestaurantTagRepo, mockTagRepo, mockUserProvider.Object);

            bool successfulChange = accountService.ChangeUserPasswordAsync("Password123_1", "Password123_2").Result;

            Assert.False(successfulChange);
        }

        [Fact]
        public void AccountService_DeleteUserAccountAsync_SuccessfulDeletion()
        {
            var loggedInUser = new User
            {
                Id = "1243435",
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var mockRestaurantRepo = Mock.Of<IRepository<Restaurant>>();
            var mockRestaurantTagRepo = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepo = Mock.Of<IRepository<Tag>>();
            var mockUserProvider = new MockUserProvider().MockGetLoggedInUserAsync(loggedInUser);

            _mockUserManager.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            var accountService = new AccountService(_mockUserManager.Object, _mockSignInManager.Object, mockRestaurantRepo, mockRestaurantTagRepo, mockTagRepo, mockUserProvider.Object);

            string successfulDeletion = accountService.DeleteUserAccountAsync("Password123_1").Result;

            Assert.Equal("", successfulDeletion);
        }

        [Fact]
        public void AccountService_DeleteUserAccountAsync_InvalidPassword()
        {
            var loggedInUser = new User
            {
                Id = "1243435",
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var mockRestaurantRepo = Mock.Of<IRepository<Restaurant>>();
            var mockRestaurantTagRepo = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepo = Mock.Of<IRepository<Tag>>();
            var mockUserProvider = new MockUserProvider().MockGetLoggedInUserAsync(loggedInUser);

            _mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            var accountService = new AccountService(_mockUserManager.Object, _mockSignInManager.Object, mockRestaurantRepo, mockRestaurantTagRepo, mockTagRepo, mockUserProvider.Object);

            string deleteResult = accountService.DeleteUserAccountAsync("Password123_1").Result;

            Assert.Equal("Incorrect password", deleteResult);
        }

        [Fact]
        public void AccountService_DeleteUserAccountAsync_UnsuccessfulDeletion()
        {
            var loggedInUser = new User
            {
                Id = "1243435",
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                UserName = "test@wheredoyouwanttoeat.xyz"
            };

            var mockRestaurantRepo = Mock.Of<IRepository<Restaurant>>();
            var mockRestaurantTagRepo = Mock.Of<IRepository<RestaurantTag>>();
            var mockTagRepo = Mock.Of<IRepository<Tag>>();
            var mockUserProvider = new MockUserProvider().MockGetLoggedInUserAsync(loggedInUser);

            _mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(IdentityResult.Failed());

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            var accountService = new AccountService(_mockUserManager.Object, _mockSignInManager.Object, mockRestaurantRepo, mockRestaurantTagRepo, mockTagRepo, mockUserProvider.Object);

            string deleteResult = accountService.DeleteUserAccountAsync("Password123_1").Result;

            Assert.Equal("Error deleting account", deleteResult);
        }
    }
}