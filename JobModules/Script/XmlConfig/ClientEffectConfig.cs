using WeaponConfigNs;

namespace XmlConfig
{
    public enum EClientEffectType
    {
        HumanHitEffect,
        DefaultHit,
        WoodHit,
        SteelHit,
        SoilHit,
        StoneHit,
        GlassHit,
        WaterHit,
        BulletDrop,
        ClipDrop,
        ShieldHit,
        MuzzleSpark,
        GrenadeExplosion,
        FlashBomb,
        FogBomb,
        BurnBomb,
        PullBolt,
        SprayPrint, /*喷漆*/
        End,
    }

    /*喷漆类型*/
    public enum ESprayPrintType {
        TypeBounds_1 = 1, /*包围盒*/ 
        TypeProjection_2, /*投影*/
    }
    public class ClientEffectConfigItem
    {
        public int Id;
        public EClientEffectType Type;
        public AssetInfo Asset;
        public int SoundId;
    }

    public class ClientEffectConfig
    {
        public ClientEffectConfigItem[] Items;
    }

}
