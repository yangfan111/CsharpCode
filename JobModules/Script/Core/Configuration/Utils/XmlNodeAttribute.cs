using System;

namespace Core.Configuration.Utils
{
    class XmlNodeAttribute : Attribute
    {
        public string Id { get; set; }

        public XmlNodeAttribute(string id)
        {
            Id = id;
        }
    }
}
