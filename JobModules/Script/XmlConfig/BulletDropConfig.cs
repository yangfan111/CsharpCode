using WeaponConfigNs;

namespace XmlConfig
{
    public class BulletDropConfig
    {
        public BulletDropConfigItem[] Items;
    }

    public class BulletDropConfigItem
    {
        public int Id;
        public EBulletCaliber Type;
        public AssetInfo ModelAsset;
        public AssetInfo DropAsset;
        public float DropVelocity;
        public int DropSound;
    }
}
