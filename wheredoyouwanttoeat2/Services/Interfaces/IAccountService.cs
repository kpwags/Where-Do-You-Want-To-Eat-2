using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using wheredoyouwanttoeat2.Models;

namespace wheredoyouwanttoeat2.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> RegisterUserAsync(User user, string password);

        Task<bool> LoginUserAsync(string email, string password);

        Task LogoutAsync();

        Task<bool> UpdateUserProfileAsync(string email, string name);

        Task<bool> ChangeUserPasswordAsync(string currentPassword, string newPassword);

        Task<string> DeleteUserAccountAsync(string password);

        IEnumerable<Restaurant> GetUserRestaurants();
    }
}