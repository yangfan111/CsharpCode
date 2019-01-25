using System;
using System.Collections.Generic;
using Core.Configuration.Utils;

namespace Core.Configuration.Equipment
{
    public class ResourceItem : XmlNodeBase
    {
        [XmlNode("Id")]
        private int _id = 0;
        [XmlNode("Name")]
        private string _name = string.Empty;
        [XmlNode("AssetBundleName")]
        private string _assetBundleName = string.Empty;
        [XmlNode("AssetName")]
        private string _assetName = string.Empty;
        [XmlNode("Volume")]
        private int _volume = 0;
        [XmlNode("BatchNumber")]
        private int _batchNumber = 0;
        [XmlNode("ItemType")]
        private int _itemType = 0;
        [XmlNode("Icon")]
        private string _icon = string.Empty;

        private static Dictionary<string, Action<object, string>> _handler = new Dictionary<string, Action<object, string>>();

        static ResourceItem()
        {
            Init(typeof(ResourceItem), _handler);
        }

        public int Id { get { return _id; } }
        public string Name { get { return _name; } }
        public string AssetBundleName { get { return _assetBundleName; } }
        public string AssetName { get { return _assetName; } }
        public int Volume { get { return _volume; } }
        public int BatchNumber { get { return _batchNumber; } }
        public int ItemType { get { return _itemType;} }
        public string Icon { get { return _icon;} }

        protected override Dictionary<string, Action<object, string>> GetHandler()
        {
            return _handler;
        }
    }
}
