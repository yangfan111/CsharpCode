using Assets.XmlConfig;
using Core;
using XmlConfig;

namespace App.Shared
{
    public struct EffectTypeInfo
    {
        public EClientEffectDefaultIds DefaultId;
        public AudioGrp_HitMatType AudioType;

    }
    public struct ExplosionEffectInfo
    {
        public EClientEffectType ClientEfcType;
        public EAudioUniqueId AudioId;
        public EEffectObjectClassify ObjectClassify;

    }
    public static class EffectUtil
    {
        public static EClientEffectType ToClientEfcType(this EEnvironmentType infoType)
        {
            switch (infoType)
            {
                case EEnvironmentType.Wood:
                    return EClientEffectType.WoodHit;
                case EEnvironmentType.Steel:
                    return EClientEffectType.SteelHit;
                case EEnvironmentType.Soil:
                    return EClientEffectType.SoilHit;
                case EEnvironmentType.Stone:
                    return EClientEffectType.StoneHit;
                case EEnvironmentType.Glass:
                    return EClientEffectType.GlassHit;
                case EEnvironmentType.Water:
                    return EClientEffectType.WaterHit;
                default:
                    return EClientEffectType.DefaultHit;
            }
        }

        public static EffectTypeInfo ToClientEfcInfo(this EEnvironmentType infoType)
        {
            EffectTypeInfo etype= new EffectTypeInfo();
            switch (infoType)
            {
                case EEnvironmentType.Wood:
                    etype.DefaultId = EClientEffectDefaultIds.WoodHit;
                    etype.AudioType = AudioGrp_HitMatType.Wood;
                    break;
                case EEnvironmentType.Steel:
                    etype.DefaultId = EClientEffectDefaultIds.SteelHit;
                    etype.AudioType = AudioGrp_HitMatType.Metal;
                    break;
                case EEnvironmentType.Soil:
                    etype.DefaultId = EClientEffectDefaultIds.SoilHit;
                    break;
                case EEnvironmentType.Stone:
                    etype.DefaultId = EClientEffectDefaultIds.StoneHit;
                    break;
                case EEnvironmentType.Glass:
                    etype.DefaultId = EClientEffectDefaultIds.GlassHit;
                    break;
                case EEnvironmentType.Water:
                    etype.DefaultId = EClientEffectDefaultIds.WaterHit;
                    etype.AudioType = AudioGrp_HitMatType.Water;

                    break;
                default:
                    etype.DefaultId = EClientEffectDefaultIds.DefaultHit;
                    etype.AudioType = AudioGrp_HitMatType.Concrete;
                    break;
            }

            return etype;
        }
        public static ExplosionEffectInfo ToExplosionEffectInfo(this EWeaponSubType subType)
        {
            ExplosionEffectInfo explosionEffectInfo = new ExplosionEffectInfo();
            switch (subType)
            {
                case EWeaponSubType.Grenade:
                    explosionEffectInfo.ClientEfcType = EClientEffectType.GrenadeExplosion;
                    explosionEffectInfo.AudioId = EAudioUniqueId.GrenadeExplosion;
                    explosionEffectInfo.ObjectClassify = EEffectObjectClassify.Explosion;
                    break;
                case EWeaponSubType.FlashBomb:
                    explosionEffectInfo.ClientEfcType = EClientEffectType.FlashBomb;
                    explosionEffectInfo.AudioId = EAudioUniqueId.FlashBombExplosion;
                    explosionEffectInfo.ObjectClassify = EEffectObjectClassify.Explosion;

                    break;
                case EWeaponSubType.FogBomb:
                    explosionEffectInfo.ClientEfcType = EClientEffectType.FogBomb;
                    explosionEffectInfo.AudioId = EAudioUniqueId.FoggyBombExplosion;
                    explosionEffectInfo.ObjectClassify = EEffectObjectClassify.Foggy;


                    break;

                default: 
                    explosionEffectInfo.ClientEfcType = EClientEffectType.BurnBomb;
                    explosionEffectInfo.AudioId = EAudioUniqueId.None;
                    explosionEffectInfo.ObjectClassify = EEffectObjectClassify.Explosion;
                    break;
            }

            return explosionEffectInfo;
        }
       
        
    }
}