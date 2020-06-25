using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using wheredoyouwanttoeat2.Controllers;
using wheredoyouwanttoeat2.Services.Interfaces;
using wheredoyouwanttoeat2.tests.Mocks.Services;
using Xunit;

namespace wheredoyouwanttoeat2.tests.Controllers
{
    public class AccountControllerTests
    {
        [Fact]
        public async Task AccountController_Register_Successful()
        {
            var mockLogger = Mock.Of<ILogger<AccountController>>();
            var mockUserProvider = Mock.Of<IUserProvider>();
            var mockAccountService = new MockAccountService().MockRegisterUserAsync(true);

            var accountController = new AccountController(mockUserProvider, mockAccountService.Object, mockLogger);

            var registerViewModel = new ViewModel.Register
            {
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            var result = (RedirectToActionResult)await accountController.Register(registerViewModel);

            Assert.Equal("Home", result.ControllerName);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task AccountController_Register_Unsuccessful()
        {
            var mockLogger = Mock.Of<ILogger<AccountController>>();
            var mockUserProvider = Mock.Of<IUserProvider>();
            var mockAccountService = new MockAccountService().MockRegisterUserAsync(false);

            var accountController = new AccountController(mockUserProvider, mockAccountService.Object, mockLogger);

            var registerViewModel = new ViewModel.Register
            {
                Name = "Test User",
                Email = "test@wheredoyouwanttoeat.xyz",
                Password = "Password123",
                ConfirmPassword = "Password123"
            };

            var result = (ViewResult)await accountController.Register(registerViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.Register>(viewResult.ViewData.Model);

            Assert.IsType<ViewModel.Register>(model);
        }

        [Fact]
        public async Task AccountController_Login_Successful()
        {
            var mockLogger = Mock.Of<ILogger<AccountController>>();
            var mockUserProvider = Mock.Of<IUserProvider>();
            var mockAccountService = new MockAccountService().MockLoginUserAsync(true);

            var accountController = new AccountController(mockUserProvider, mockAccountService.Object, mockLogger);

            var loginViewModel = new ViewModel.Login
            {
                Email = "test@wheredoyouwanttoeat.xyz",
                Password = "Password123",
                ReturnUrl = null
            };

            var result = (RedirectToActionResult)await accountController.Login(loginViewModel);

            Assert.Equal("Home", result.ControllerName);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task AccountController_Login_Unsuccessful()
        {
            var mockLogger = Mock.Of<ILogger<AccountController>>();
            var mockUserProvider = Mock.Of<IUserProvider>();
            var mockAccountService = new MockAccountService().MockLoginUserAsync(false);

            var accountController = new AccountController(mockUserProvider, mockAccountService.Object, mockLogger);

            var loginViewModel = new ViewModel.Login
            {
                Email = "test@wheredoyouwanttoeat.xyz",
                Password = "Password123",
                ReturnUrl = null
            };

            var result = await accountController.Login(loginViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.Login>(viewResult.ViewData.Model);

            Assert.IsType<ViewModel.Login>(model);
            Assert.Equal("Invalid email address or password", model.ErrorMessage);
        }

        [Fact]
        public async Task AccountController_EditProfile_Successful()
        {
            var mockLogger = Mock.Of<ILogger<AccountController>>();
            var mockUserProvider = Mock.Of<IUserProvider>();
            var mockAccountService = new MockAccountService().MockUpdateUserProfileAsync(true);

            var accountController = new AccountController(mockUserProvider, mockAccountService.Object, mockLogger);

            var editProfileViewModel = new ViewModel.EditProfile
            {
                Name = "James Doe",
                Email = "test@wheredoyouwanttoeat.xyz"
            };

            var result = await accountController.EditProfile(editProfileViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.EditProfile>(viewResult.ViewData.Model);

            Assert.IsType<ViewModel.EditProfile>(model);
            Assert.Equal("Profile updated successfully", model.SuccessMessage);
            Assert.Equal("", model.ErrorMessage);
        }

        [Fact]
        public async Task AccountController_EditProfile_Unsuccessful()
        {
            var mockLogger = Mock.Of<ILogger<AccountController>>();
            var mockUserProvider = Mock.Of<IUserProvider>();
            var mockAccountService = new MockAccountService().MockUpdateUserProfileAsync(false);

            var accountController = new AccountController(mockUserProvider, mockAccountService.Object, mockLogger);

            var editProfileViewModel = new ViewModel.EditProfile
            {
                Name = "James Doe",
                Email = "test@wheredoyouwanttoeat.xyz"
            };

            var result = await accountController.EditProfile(editProfileViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.EditProfile>(viewResult.ViewData.Model);

            Assert.IsType<ViewModel.EditProfile>(model);
            Assert.Equal("Error updating user profile", model.ErrorMessage);
            Assert.Equal("", model.SuccessMessage);
        }

        [Fact]
        public async Task AccountController_ChangePassword_Successful()
        {
            var mockLogger = Mock.Of<ILogger<AccountController>>();
            var mockUserProvider = Mock.Of<IUserProvider>();
            var mockAccountService = new MockAccountService().MockChangeUserPasswordAsync(true);

            var accountController = new AccountController(mockUserProvider, mockAccountService.Object, mockLogger);

            var changePasswordViewModel = new ViewModel.ChangePassword
            {
                CurrentPassword = "Password123",
                Password = "NewPassword123",
                ConfirmPassword = "NewPassword123"
            };

            var result = await accountController.ChangePassword(changePasswordViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.ChangePassword>(viewResult.ViewData.Model);

            Assert.IsType<ViewModel.ChangePassword>(model);
            Assert.Equal("Password changed successfully", model.SuccessMessage);
            Assert.Equal("", model.ErrorMessage);
        }

        [Fact]
        public async Task AccountController_ChangePassword_Unsuccessful()
        {
            var mockLogger = Mock.Of<ILogger<AccountController>>();
            var mockUserProvider = Mock.Of<IUserProvider>();
            var mockAccountService = new MockAccountService().MockChangeUserPasswordAsync(false);

            var accountController = new AccountController(mockUserProvider, mockAccountService.Object, mockLogger);

            var changePasswordViewModel = new ViewModel.ChangePassword
            {
                CurrentPassword = "Password123",
                Password = "NewPassword123",
                ConfirmPassword = "NewPassword123"
            };

            var result = await accountController.ChangePassword(changePasswordViewModel);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.ChangePassword>(viewResult.ViewData.Model);

            string messages = string.Join("; ", accountController.ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

            Assert.IsType<ViewModel.ChangePassword>(model);
            Assert.Equal(1, accountController.ModelState.ErrorCount);
            Assert.Equal("Incorrect password", messages);
            Assert.Equal("", model.SuccessMessage);
        }

        [Fact]
        public async Task AccountController_DeleteAccount_Successful()
        {
            var mockLogger = Mock.Of<ILogger<AccountController>>();
            var mockUserProvider = Mock.Of<IUserProvider>();
            var mockAccountService = new MockAccountService().MockDeleteUserAccountAsync("");

            var accountController = new AccountController(mockUserProvider, mockAccountService.Object, mockLogger);

            var deleteAccountViewModel = new ViewModel.DeleteAccount
            {
                Password = "Password123"
            };

            var result = (RedirectToActionResult)await accountController.DeleteAccount(deleteAccountViewModel);

            Assert.Equal("Home", result.ControllerName);
            Assert.Equal("Index", result.ActionName);
        }

        [Fact]
        public async Task AccountController_DeleteAccount_Unsuccessful_IncorrectPassword()
        {
            var mockLogger = Mock.Of<ILogger<AccountController>>();
            var mockUserProvider = Mock.Of<IUserProvider>();
            var mockAccountService = new MockAccountService().MockDeleteUserAccountAsync("Incorrect password");

            var accountController = new AccountController(mockUserProvider, mockAccountService.Object, mockLogger);

            var deleteAccountViewModel = new ViewModel.DeleteAccount
            {
                Password = "Password123"
            };

            var result = await accountController.DeleteAccount(deleteAccountViewModel);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.DeleteAccount>(viewResult.ViewData.Model);

            string messages = string.Join("; ", accountController.ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

            Assert.IsType<ViewModel.DeleteAccount>(model);
            Assert.Equal(1, accountController.ModelState.ErrorCount);
            Assert.Equal("Incorrect password", messages);
            Assert.Equal("", model.SuccessMessage);
        }

        [Fact]
        public async Task AccountController_DeleteAccount_Unsuccessful_Error()
        {
            var mockLogger = Mock.Of<ILogger<AccountController>>();
            var mockUserProvider = Mock.Of<IUserProvider>();
            var mockAccountService = new MockAccountService().MockDeleteUserAccountAsync("Error deleting account");

            var accountController = new AccountController(mockUserProvider, mockAccountService.Object, mockLogger);

            var deleteAccountViewModel = new ViewModel.DeleteAccount
            {
                Password = "Password123"
            };

            var result = await accountController.DeleteAccount(deleteAccountViewModel);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ViewModel.DeleteAccount>(viewResult.ViewData.Model);

            Assert.IsType<ViewModel.DeleteAccount>(model);
            Assert.Equal("Error deleting account", model.ErrorMessage);
            Assert.Equal("", model.SuccessMessage);
        }
    }
}