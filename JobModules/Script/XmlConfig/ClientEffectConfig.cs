using WeaponConfigNs;

namespace XmlConfig
{
    /// <summary>
    /// 和clienteffect表的id对应，不必和名字一一对应
    /// </summary>
    public enum EClientEffectDefaultIds
    {
        None,
        StoneHit = 1,
        SteelHit = 2,
        WoodHit = 3,
        SoilHit = 4,
        WaterHit = 5,
        GlassHit = 6,
        DefaultHit = 7,
        HumanHitEffect = 8,
        ShieldHit = 9,
        BulletDrop = 18,
        ClipDrop = 19,
        DefaultBulletFly=33,
 
    }
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
        BulletFly,
        RifleBallistic,
        ShotGunBallistic,
        SniperBallistic,
        
        End,
    }

    public enum EEffectObjectClassify
    {
        EnvHit=0,
        BulletDrop=1,
        BulletFly=2,
        Muzzle = 3,
        PlayerHit=4,
        Explosion=5,
        Foggy=6,
        Count,

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
