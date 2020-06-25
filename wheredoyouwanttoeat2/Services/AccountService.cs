using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Respositories.Interfaces;
using wheredoyouwanttoeat2.Services.Interfaces;

namespace wheredoyouwanttoeat2.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IRepository<Restaurant> _restaurantRepository;
        private readonly IRepository<RestaurantTag> _restaurantTagRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IUserProvider _userProvider;

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager, IRepository<Restaurant> restaurantRepository, IRepository<RestaurantTag> restaurantTagRepository, IRepository<Tag> tagRepository, IUserProvider provider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _restaurantRepository = restaurantRepository;
            _restaurantTagRepository = restaurantTagRepository;
            _tagRepository = tagRepository;
            _userProvider = provider;
        }

        public async Task<bool> RegisterUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return result.Succeeded;
        }

        public async Task<bool> LoginUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await _signInManager.SignOutAsync();

                var result = await _signInManager.PasswordSignInAsync(user, password, true, false);

                if (result.Succeeded)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> UpdateUserProfileAsync(string email, string name)
        {
            var user = await _userProvider.GetLoggedInUserAsync();

            user.Email = email;
            user.UserName = email;
            user.Name = name;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> ChangeUserPasswordAsync(string currentPassword, string newPassword)
        {
            var user = await _userProvider.GetLoggedInUserAsync();

            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                return result.Succeeded;
            }

            return false;
        }

        public async Task<string> DeleteUserAccountAsync(string password)
        {
            var user = await _userProvider.GetLoggedInUserAsync();

            var passwordResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (!passwordResult.Succeeded)
            {
                return "Incorrect password";
            }

            await _signInManager.SignOutAsync();

            await DeleteUserRestaurants(user);

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return "Error deleting account";
            }

            return "";
        }

        public IEnumerable<Restaurant> GetUserRestaurants()
        {
            string userId = _userProvider.GetUserId();
            return _restaurantRepository.Get(r => r.UserId == userId);
        }

        protected async Task DeleteUserRestaurants(User user)
        {
            var restaurants = _restaurantRepository.Get(r => r.UserId == user.Id).OrderBy(r => r.Name).ToList();

            foreach (var restaurant in restaurants)
            {
                List<RestaurantTag> restaurantTags = _restaurantTagRepository.Get(rt => rt.RestaurantId == restaurant.RestaurantId).ToList();
                List<int> tagIds = new List<int>();
                foreach (RestaurantTag restaurantTag in restaurantTags)
                {
                    // save to list for possible cleanup
                    tagIds.Add(restaurantTag.TagId);

                    await _restaurantTagRepository.Delete(restaurantTag);
                }

                List<int> tagsToDelete = new List<int>();
                foreach (int tagId in tagIds)
                {
                    if (_restaurantTagRepository.Get(rt => rt.TagId == tagId).Count() == 0)
                    {
                        // it's not used anywhere else, let's delete it to keep the database cleaner
                        tagsToDelete.Add(tagId);
                    }
                }

                // delete unused tags
                foreach (int tagId in tagsToDelete)
                {
                    var tagToDelete = _tagRepository.Get(t => t.TagId == tagId).FirstOrDefault();
                    if (tagToDelete != null)
                    {
                        await _tagRepository.Delete(tagToDelete);
                    }
                }
            }

            await _restaurantRepository.DeleteRange(restaurants);
        }
    }
}