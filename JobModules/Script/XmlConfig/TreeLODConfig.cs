using System.Collections.Generic;

namespace Assets.XmlConfig
{
    public class TreeLODConfig
    {
        public List<TreeLODItem> LODSettings = new List<TreeLODItem>();
    }

    public class TreeLODItem
    {
        public string AssetBundle;
        public string assetName;
        public List<float> LODPercent = new List<float>();
    }
}