using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using wheredoyouwanttoeat2.Services.Interfaces;
using wheredoyouwanttoeat2.Respositories.Interfaces;
using wheredoyouwanttoeat2.Models;
using wheredoyouwanttoeat2.Classes;

namespace wheredoyouwanttoeat2.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<Restaurant> _restaurantRepository;
        private readonly IRepository<RestaurantTag> _restaurantTagRepository;
        private readonly IRepository<Tag> _tagRepository;
        private readonly IUserProvider _userProvider;

        public AdminService(IRepository<Restaurant> restaurantRepository, IRepository<RestaurantTag> restaurantTagRepository, IRepository<Tag> tagRepository, IUserProvider provider)
        {
            _restaurantRepository = restaurantRepository;
            _restaurantTagRepository = restaurantTagRepository;
            _tagRepository = tagRepository;
            _userProvider = provider;
        }

        public IEnumerable<Restaurant> GetUserRestaurants(string userId = "")
        {
            if (userId == "")
            {
                userId = _userProvider.GetUserId();
            }

            return _restaurantRepository.Get(r => r.UserId == userId);
        }

        public Restaurant GetRestaurantById(int id)
        {
            return _restaurantRepository.GetById(id);
        }

        public async Task<Restaurant> AddRestaurant(Restaurant restaurant)
        {
            if (!restaurant.IsValid())
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException("Name is required");
            }

            await _restaurantRepository.Add(restaurant);

            // if a full address was entered, get the latitude and longitude
            // this is for displaying the map on the details page...I'm making the call on the server so the calls to Mapquest's API is limited to restaurant updates
            if (restaurant.HasFullAddress)
            {
                try
                {
                    LatLong coordinates = await Utilities.GetLatitudeAndLongitudeForAddress(restaurant.FullAddress);

                    restaurant.Latitude = coordinates.Latitude;
                    restaurant.Longitude = coordinates.Longitude;

                    await UpdateRestaurant(restaurant);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving latitude and longitude from MapQuest API {ex.Message}");
                }
            }

            return restaurant;
        }

        public async Task AddTagsToRestaurant(Restaurant restaurant, List<string> tags)
        {
            foreach (string tag in tags)
            {
                var dbTag = GetTagByName(tag);
                if (dbTag == null)
                {
                    // tag does not exist, add it
                    dbTag = new Tag
                    {
                        Name = tag.Trim().ToLower()
                    };

                    await _tagRepository.Add(dbTag);
                }

                // create the restaurant-tag link
                var restaurantTag = new RestaurantTag
                {
                    Restaurant = restaurant,
                    RestaurantId = restaurant.RestaurantId,
                    Tag = dbTag,
                    TagId = dbTag.TagId
                };

                await _restaurantTagRepository.Add(restaurantTag);
            }
        }

        public async Task UpdateRestaurantTags(Restaurant restaurant, List<string> tags)
        {
            foreach (string tag in tags)
            {
                var dbTag = GetTagByName(tag);
                if (dbTag == null)
                {
                    // tag does not exist, add it
                    dbTag = new Tag
                    {
                        Name = tag.Trim().ToLower()
                    };

                    await _tagRepository.Add(dbTag);
                }

                // check to see if the link exists
                var dbRestaurantTag = _restaurantTagRepository.Get(rt => rt.RestaurantId == restaurant.RestaurantId && rt.TagId == dbTag.TagId).FirstOrDefault();
                if (dbRestaurantTag == null)
                {
                    // link does not exist, create it
                    var restaurantTag = new RestaurantTag
                    {
                        Restaurant = restaurant,
                        RestaurantId = restaurant.RestaurantId,
                        Tag = dbTag,
                        TagId = dbTag.TagId
                    };

                    await _restaurantTagRepository.Add(restaurantTag);
                }
            }
        }

        public async Task CleanUpTags(Restaurant restaurant, List<string> tags)
        {
            List<int> restaurantTagsToDelete = new List<int>();
            foreach (var rt in _restaurantTagRepository.Get(rt => rt.RestaurantId == restaurant.RestaurantId))
            {
                var tag = _tagRepository.Get(t => t.TagId == rt.TagId).FirstOrDefault();
                if (tag != null && !tags.Contains(tag.Name))
                {
                    // add it to the list to delete
                    restaurantTagsToDelete.Add(rt.TagId);
                }
            }

            foreach (int tagId in restaurantTagsToDelete)
            {
                var tagToDelete = _restaurantTagRepository.Get(rt => rt.RestaurantId == restaurant.RestaurantId && rt.TagId == tagId).FirstOrDefault();
                if (tagToDelete != null)
                {
                    await _restaurantTagRepository.Delete(tagToDelete);
                }
            }

            // check to see if the tag is used anywhere else
            List<int> tagsToDelete = new List<int>();
            foreach (int tagId in restaurantTagsToDelete)
            {
                if (_restaurantTagRepository.Get(rt => rt.TagId == tagId).Count() == 0)
                {
                    // it's not used anywhere else, let's delete it to keep the database cleaner
                    tagsToDelete.Add(tagId);
                }
            }

            foreach (int tagId in tagsToDelete)
            {
                var tagToDelete = _tagRepository.Get(t => t.TagId == tagId).FirstOrDefault();
                if (tagToDelete != null)
                {
                    await _tagRepository.Delete(tagToDelete);
                }
            }
        }

        public async Task<Restaurant> UpdateRestaurant(Restaurant restaurant, bool updateCoordinates = false)
        {
            if (!restaurant.IsValid())
            {
                throw new System.ComponentModel.DataAnnotations.ValidationException("Name is required");
            }

            if (updateCoordinates || restaurant.Latitude == 0 || restaurant.Longitude == 0)
            {
                try
                {
                    var coordinates = await Utilities.GetLatitudeAndLongitudeForAddress(restaurant.FullAddress);
                    restaurant.Latitude = coordinates.Latitude;
                    restaurant.Longitude = coordinates.Longitude;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving latitude and longitude from MapQuest API {ex.Message}");
                }
            }

            await _restaurantRepository.Update(restaurant);

            return restaurant;
        }

        public async Task DeleteRestaurant(Restaurant restaurant)
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

            await _restaurantRepository.Delete(restaurant);
        }

        public Tag GetTagByName(string tagName)
        {
            return _tagRepository.Get(t => t.Name.ToLower() == tagName.ToLower()).FirstOrDefault();
        }

        public IEnumerable<RestaurantTag> GetRestaurantTags(int restaurantId)
        {
            return _restaurantTagRepository.Get(rt => rt.RestaurantId == restaurantId);
        }
    }
}