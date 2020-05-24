using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Text.Json;
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
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };

                using (XmlWriter writer = XmlWriter.Create(sww, settings))
                {
                    xsSubmit.Serialize(writer, this);
                    xml = sww.ToString(); // Your XML
                }
            }

            return xml;
        }

        public string DownloadAsJSON()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = "{}";

            json  = JsonSerializer.Serialize(this, options);

            return json;
        }
    }
}