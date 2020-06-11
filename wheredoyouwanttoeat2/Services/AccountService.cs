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

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager, IRepository<Restaurant> restaurantRepository, IRepository<RestaurantTag> restaurantTagRepository, IRepository<Tag> tagRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _restaurantRepository = restaurantRepository;
            _restaurantTagRepository = restaurantTagRepository;
            _tagRepository = tagRepository;
        }

        public async Task<IdentityResult> RegisterUser(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<bool> LoginUser(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await _signInManager.SignOutAsync();

                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, password, true, false);

                if (result.Succeeded)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<IdentityResult> UpdateUserProfile(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangeUserPassword(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<string> DeleteUserAccount(User user, string password)
        {
            var passwordResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (!passwordResult.Succeeded)
            {
                return "Invalid password";
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

        public IEnumerable<Restaurant> GetUserRestaurants(string userId)
        {
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