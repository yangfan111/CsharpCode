using WeaponConfigNs;

namespace XmlConfig
{
    public class ClipDropConfig
    {
        public ClipDropConfigItem[] Items;
    }

    public class ClipDropConfigItem
    {
        public int Id;
        public AssetInfo ModelAsset;
        public AssetInfo DropAsset;
        public int DropSound;
    }
}
