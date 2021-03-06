using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace WhereDoYouWantToEat2.Classes
{
    public static class Utilities
    {
        public static AppSettings AppSettings;


        /// <summary>
        /// Takes a comma separated list of tags and trims the excess spaces and converts them all to lower-case
        /// </summary>
        /// <param name="tagList">Comma Separated List of Tags</param>
        /// <returns>A list of strings with all extra spaces removed and converted to lower case</returns>
        public static List<string> CorrectUserEnteredTags(string tagList)
        {
            if (tagList == null)
            {
                return new List<string>();
            }

            List<string> correctedTags = new List<string>();

            foreach (string tag in tagList.Split(','))
            {
                correctedTags.Add(tag.Trim().ToLower());
            }

            return correctedTags;
        }

        /// <summary>
        /// Uses the MapQuest Geocoding API to get the latitude and longitude of an address
        /// </summary>
        /// <param name="apiKey">The MapQuest API Key</param>
        /// <param name="address">The address to find the coordinates for</param>
        /// <returns>The Latitude and Longitude coordinates of the address</returns>
        public static async Task<LatLong> GetLatitudeAndLongitudeForAddress(string address)
        {
            decimal latitude = 0;
            decimal longitude = 0;

            string mapQuestUrl = $"http://www.mapquestapi.com/geocoding/v1/address?key={AppSettings.MapQuestAPIKey}&location={address}";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(mapQuestUrl))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var mapQuestResponse = JsonSerializer.Deserialize<MapQuestResponse>(apiResponse);

                        if (mapQuestResponse.results.Count > 0)
                        {
                            if (mapQuestResponse.results[0].locations.Count > 0)
                            {
                                latitude = mapQuestResponse.results[0].locations[0].displayLatLng.lat;
                                longitude = mapQuestResponse.results[0].locations[0].displayLatLng.lng;
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP request error calling MapQuest API: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calling MapQuest API: {ex.Message}");
            }


            return new LatLong(latitude, longitude);
        }


        public static List<string> TooManyChoices = new List<string>
        {
            "You really need another place to go? Okay, fine! You're going to...",
            "That wasn't good enough? Okay, sure...you're going to...",
            "That place looked pretty good...but I guess you can go to...",
            "Really? Okay...you're going to...",
            "You should choose soon...you're going to..."
        };

        public static List<string> WayTooManyChoices = new List<string>
        {
            "Oh for Pete's sake...just go to",
            "For crying out loud...just go to",
            "You're going to go hungry if you keep this up...go to",
            "Hunger's looking better all the time, huh? Go to",
            "I got nothing...just go to"
        };

        public static List<string> WayWayTooManyChoices = new List<string>
        {
            "I give up...just go make yourself a bowl of cereal.",
            "You're impossible to work with...make a peanut butter & jelly sandwich.",
            "I guess you're just going to have to go hungry."
        };
    }
}