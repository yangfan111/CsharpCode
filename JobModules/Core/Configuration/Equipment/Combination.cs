using System;
using System.Collections.Generic;
using System.Xml;
using Core.Configuration.Utils;

namespace Core.Configuration.Equipment
{
    public class Combination : XmlNodeBase
    {
        [XmlNode("Id")]
        private int _id = 0;
        [XmlNode("Name")]
        private string _name = string.Empty;
        private List<WeightedPackage> _weightedPackages = new List<WeightedPackage>();

        private static Dictionary<string, Action<object, string>> _handler = new Dictionary<string, Action<object, string>>();

        static Combination()
        {
            Init(typeof(Combination), _handler);
        }

        public int Id { get { return _id; } }
        public string Name { get { return _name; } }
        public List<WeightedPackage> WeightedPackages { get { return _weightedPackages; } }

        protected override Dictionary<string, Action<object, string>> GetHandler()
        {
            return _handler;
        }

        protected override void ParseSpecial(XmlNode node)
        {
            if (node.Name == "Packages")
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    var weightedPackage = new WeightedPackage();
                    weightedPackage.Parse(child);
                    _weightedPackages.Add(weightedPackage);
                }
            }
            else
            {
                base.ParseSpecial(node);
            }
        }
    }

    public class WeightedPackage : XmlNodeBase
    {
        [XmlNode("Id")]
        private int _id = 0;
        [XmlNode("Weight")]
        private int _weight = 0;

        private static Dictionary<string, Action<object, string>> _handler = new Dictionary<string, Action<object, string>>();

        static WeightedPackage()
        {
            Init(typeof(WeightedPackage), _handler);
        }

        public int Id { get { return _id; } }
        public int Weight { get { return _weight; } }

        protected override Dictionary<string, Action<object, string>> GetHandler()
        {
            return _handler;
        }
    }
}
