using System.Collections.Generic;
using System.Xml;

namespace Core.Configuration.Equipment
{
    class LocationParser
    {
        private Dictionary<int, Location> _locations = new Dictionary<int, Location>();

        public Dictionary<int, Location> LocationCollection { get { return _locations; } }

        public void Parse(string xmlText)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlText);
            if (document.DocumentElement != null)
            {
                foreach (XmlNode child in document.DocumentElement.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "Location":
                            var location = new Location();
                            location.Parse(child);
                            _locations.Add(location.Id, location);
                            break;
                    }
                }
            }
        }
    }
}
