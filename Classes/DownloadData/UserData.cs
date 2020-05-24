using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace wheredoyouwanttoeat2.Classes.DownloadData
{
    public class UserData
    {
        public User User { get; set; }

        public List<Restaurant> Restaurants { get; set; }

        public string DownloadAsXML()
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(UserData));
            var xml = "";

            using (var sww = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, this);
                    xml = sww.ToString(); // Your XML
                }
            }

            return xml;
        }

        public string DownloadAsJSON()
        {
            return "";
        }
    }
}