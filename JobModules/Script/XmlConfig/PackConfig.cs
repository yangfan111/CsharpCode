using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlConfig
{
    [XmlRoot("root")]
    public class PackConfig
    {
        public Pack[] Items;
    }

    [XmlType("pack")]
    public class Pack
    {
        public int Id;
        public string Name;
        public int NameId;
        public string Res;
        public string Description;
        public int DescriptionId;
        public int Xlv;
        public bool Auto;

        public List<ItemLink> Items;
    }

    [XmlType("Item")]
    public class ItemLink
    {
        public int Type;
        public int Tid;
        public int Num;
        public bool Merge;
    }
}
