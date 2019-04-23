using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class DropPoolConfig
    {
        public DropPool[] Items;

        private static DropPoolConfig _current;

        public static DropPoolConfig current
        {
            get
            {
                if (_current == null)
                {
                    _current = new DropPoolConfig();
                }
                return _current;
            }
            set { _current = value; }
        }
    }

    [XmlType("child")]
    public class DropPool
    {
        public string Cat;
        public string CatNotes;
        public string ItemType;
        public string ItemId;
        public string DropCount;
        public string ItemNotes;
        public string Proiority;
    }
}
