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
    }
}