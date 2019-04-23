using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class DropItemConfig
    {
        public DropItem[] Items;

        private static DropItemConfig _current;

        public static DropItemConfig current
        {
            get
            {
                if (_current == null)
                {
                    _current = new DropItemConfig();
                }
                return _current;
            }
            set { _current = value; }
        }
    }

    [XmlType("child")]
    public class DropItem
    {
        public string ItemType;
        public string Item;
        public string ItemNotes;
        public string ExtraDrop;
        public string CatNotes;
    }
}
