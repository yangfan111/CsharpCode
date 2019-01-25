using System.Xml;

namespace Core.Configuration.Utils
{
    public interface IXmlNodeParser
    {
        void Parse(XmlNode node);
    }
}
