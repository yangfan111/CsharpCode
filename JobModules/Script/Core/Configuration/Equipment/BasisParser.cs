using System;
using System.Collections.Generic;
using System.Xml;

namespace Core.Configuration.Equipment
{
    class BasisParser
    {
        private Dictionary<int, ResourceItem> _resourceItems = new Dictionary<int, ResourceItem>();
        private Dictionary<int, Package> _packages = new Dictionary<int, Package>();
        private Dictionary<int, Combination> _combinations = new Dictionary<int, Combination>();

        public Dictionary<int, ResourceItem> ResourceItems { get { return _resourceItems; } }
        public Dictionary<int, Package> Packages { get { return _packages; } }
        public Dictionary<int, Combination> Combinations { get { return _combinations; } }

        private Dictionary<string, Action<XmlNode>> _handler;

        public BasisParser()
        {
            _handler = new Dictionary<string, Action<XmlNode>>()
            {
                { "ResourceItems", CreateResourceItem },
                { "Packages", CreatePackage },
                { "Combinations", CreateCombination }
            };
        }

        public void Parse(string xmlText)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlText);
            if (document.DocumentElement != null)
            {
                foreach (XmlNode child in document.DocumentElement.ChildNodes)
                {
                    if (_handler.ContainsKey(child.Name))
                    {
                        var invoke = _handler[child.Name];
                        foreach (XmlNode node in child.ChildNodes)
                        {
                            invoke(node);
                        }
                    }
                }
            }
        }

        private void CreateResourceItem(XmlNode node)
        {
            var resourceItem = new ResourceItem();
            resourceItem.Parse(node);
            _resourceItems.Add(resourceItem.Id, resourceItem);
        }

        private void CreatePackage(XmlNode node)
        {
            var package = new Package();
            package.Parse(node);
            _packages.Add(package.Id, package);
        }

        private void CreateCombination(XmlNode node)
        {
            var combination = new Combination();
            combination.Parse(node);
            _combinations.Add(combination.Id, combination);
        }
    }
}
