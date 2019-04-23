using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class PreLoadingConfig
    {
        public PreLoading[] Items;
    }

    [XmlType("child")]
    public class PreLoading
    {
        public int Id;
        public string AbName;
        public int IsLoadAsset;
        public string Tips;
        public int TipsId;
    }
}
