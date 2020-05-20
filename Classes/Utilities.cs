using System.Collections.Generic;

namespace wheredoyouwanttoeat2.Classes
{
    public static class Utilities
    {
        /// <summary>
        /// Takes a comma separated list of tags and trims the excess spaces and converts them all to lower-case
        /// </summary>
        /// <param name="tagList">Comma Separated List of Tags</param>
        /// <returns>A list of strings with all extra spaces removed and converted to lower case</returns>
        public static List<string> CorrectUserEnteredTags(string tagList)
        {
            List<string> correctedTags = new List<string>();

            foreach (string tag in tagList.Split(','))
            {
                correctedTags.Add(tag.Trim().ToLower());
            }

            return correctedTags;
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