using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using wheredoyouwanttoeat2.Models;

namespace wheredoyouwanttoeat2.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterUser(User user, string password);

        Task<bool> LoginUser(string email, string password);

        Task<IdentityResult> UpdateUserProfile(User user);

        Task<IdentityResult> ChangeUserPassword(User user, string currentPassword, string newPassword);

        Task<string> DeleteUserAccount(User user, string password);

        IEnumerable<Restaurant> GetUserRestaurants(string userId);
    }
}