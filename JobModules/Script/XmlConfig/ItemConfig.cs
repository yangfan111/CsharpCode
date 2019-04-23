using WeaponConfigNs;

namespace XmlConfig
{
    //NOTICE: 注意，在BagUtility中有代码和这个枚举相关，修改枚举的时候，请看下是否需要修改BagUtility中的对应方法
    public enum EItemType
    {
        None,
        Helmet = 1,// 头盔
        Backpack,// 背包
        Vest, // 护甲
        Shoe,// 鞋
        Glasses, // 眼镜
        Mask, // 面具
        Jacket, // 夹克 
        Shirt, // T恤
        Gloves, // 手套
        Pant,// 裤子
        Hat, // 帽子
        Rifle,
        Shotgun,
        Sniper,
        Pistol,
        Melee,
        Grenade,
        Bullet,
        Muzzle,//枪口配件
        LowerRail,//枪把
        UpperRail,//瞄具
        Magazine,//弹夹
        Stock,//枪托
    }

    public class ItemAssetInfo
    {
        public string BundleName;
        public string AssetName;
    }

    public class ItemConfig
    {
        public ItemConfigItem[] Items;
    }

    public class ItemConfigItem
    {
        public int Id;
        public string Name;
        public int AssetId;
        public ItemAssetInfo Asset;
        public EBulletCaliber Caliber;
        public EItemType ItemType;
        public int Volume;
        public int BatchNumber;
    }

}
