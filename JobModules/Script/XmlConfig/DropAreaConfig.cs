using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class DropAreaConfig
    {
        public DropArea[] Items;

        private static DropAreaConfig _current;

        public static DropAreaConfig current
        {
            get
            {
                if (_current == null)
                {
                    _current = new DropAreaConfig();
                }
                return _current;
            }
            set { _current = value; }
        }
    }

    [XmlType("child")]
    public class DropArea
    {
        public int Id;
        public int MapId;
        public int AwardId;
        public string IdNotes;
        public string Range;
        public string Count;
        public string Drop;
    }
}
