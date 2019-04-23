using WeaponConfigNs;

namespace XmlConfig
{
    public class SoundConfig
    {
        public SoundConfigItem[] Items;
    }

    public class SoundConfigItem
    {
        public int Id;
        public AssetInfo Asset;
        public bool Sync;
        public int Delay;
        public float Distance;
    }

    public enum EWeaponSoundType
    {
        SwitchIn,
        LeftFire1,
        LeftFire2,
        RightFire1,
        RightFire2,
        ReloadStart,
        ReloadEnd,
        PullBolt,
        OnShoulder,
        SwitchFireMode,
        Transform,
        ClipDrop,
        WeaponSoundLength = 100,
    }

    public enum EPlayerSoundType
    {
        UseBandage,
        UseAidKt,
        UseDrink,
        UseTablet,
        UseEpinephrine,
        UseGasoline,
        OpenParachute,
        OnGliding,
        OnParachute,
        GetDown,
        GetUp,
        ChangeWeapon,
        ChangeMode,
        OpenDoor,
        CloseDoor,
        Walk,
        Squat,
        Crawl,
        WalkSwamp,
        SquatSwamp,
        DropWater,
        Swim,
        Dive,
        CrawlSwamp,
        Land,
        Length,
    }
}