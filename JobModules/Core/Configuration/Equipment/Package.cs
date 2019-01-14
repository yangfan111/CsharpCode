using System;
using System.Collections.Generic;
using System.Xml;
using Core.Configuration.Utils;

namespace Core.Configuration.Equipment
{
    public class Package : XmlNodeBase
    {
        [XmlNode("Id")]
        private int _id = 0;
        [XmlNode("Name")]
        private string _name = string.Empty;

        private List<WeightedResourceItem> _weightedResourceItems = new List<WeightedResourceItem>();

        private static Dictionary<string, Action<object, string>> _handler = new Dictionary<string, Action<object, string>>();

        static Package()
        {
            Init(typeof(Package), _handler);
        }

        public int Id { get { return _id; } }
        public string Name { get { return _name; } }
        public List<WeightedResourceItem> WeightedResourceItems { get { return _weightedResourceItems; } }

        protected override Dictionary<string, Action<object, string>> GetHandler()
        {
            return _handler;
        }

        protected override void ParseSpecial(XmlNode node)
        {
            if (node.Name == "ResourceItems")
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    var weightedResourceItem = new WeightedResourceItem();
                    weightedResourceItem.Parse(child);
                    _weightedResourceItems.Add(weightedResourceItem);
                }
            }
            else
            {
                base.ParseSpecial(node);
            }
        }
    }

    public class WeightedResourceItem : XmlNodeBase
    {
        [XmlNode("Id")]
        private int _id = 0;
        [XmlNode("Number")]
        private int _number = 0;
        [XmlNode("Weight")]
        private int _weight = 0;

        private static Dictionary<string, Action<object, string>> _handler = new Dictionary<string, Action<object, string>>();

        static WeightedResourceItem()
        {
            Init(typeof(WeightedResourceItem), _handler);
        }

        public int Id { get { return _id; } }
        public int Number { get { return _number; } }
        public int Weight { get { return _weight; } }

        protected override Dictionary<string, Action<object, string>> GetHandler()
        {
            return _handler;
        }
    }
}
