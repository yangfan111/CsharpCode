using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Core.Clothes
{
    [XmlRoot("CharacterConfig")]
    public class CharacterConfig
    {
        [XmlElement("FloderPath")]
        public string resourceFolderPath = string.Empty;
        [XmlElement("SkeletonName")]
        public string skeletonName = string.Empty;
        [XmlArray("Dict")]
        public List<PartElements> dict = new List<PartElements>();

        public static CharacterConfig ParseFromString(string xml)
        {
            var serializer = new XmlSerializer(typeof(CharacterConfig));
            StringReader sr = new StringReader(xml);
            CharacterConfig info = (CharacterConfig)serializer.Deserialize(sr);
            return info;
        }
    }

    [XmlRoot("PartElements")]
    public class PartElements
    {
        [XmlElement("PartType")]
        public PartType type = PartType.Body;
        [XmlArray("Resources")]
        public List<string> resources = new List<string>();
    }

    public enum PartType
    {
        Body,
        Hair,
        Jacket01,
        Jacket02,
        Legs,
        Shoes
    }
}
