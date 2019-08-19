using System;
using System.Collections.Generic;
using App.Client.GameModules.ClientEffect.EffectLogic;
using App.Shared.Components.ClientEffect;
using Utils.Utils;
using XmlConfig;

namespace App.Client.GameModules.ClientEffect
{
    public class ClientEffectLogicFactory
    {
        public static Dictionary<EClientEffectType, IEffectLogic> _types = new Dictionary<EClientEffectType, IEffectLogic>(CommonIntEnumEqualityComparer<EClientEffectType>.Instance);

        static ClientEffectLogicFactory()
        {
        //    _types[EClientEffectType.HumanHitEffect] = new HumanHitClientEffect();
       //     _types[EClientEffectType.SoilHit] =  new SoilHitClientEffect();
         //   _types[EClientEffectType.WoodHit] = new WoodHitClientEffect();
            _types[EClientEffectType.SteelHit] = new SteelHitClientEffect();
           // _types[EClientEffectType.StoneHit] = new StoneHitClientEffect();
         //   _types[EClientEffectType.WaterHit] = new WaterHitClientEffect();
       //     _types[EClientEffectType.GlassHit] = new GlassHitClientEffect();
         //   _types[EClientEffectType.BulletDrop] = new BulletDropClientEffect();
        //    _types[EClientEffectType.ClipDrop] = new ClipDropClientEffect();
        //    _types[EClientEffectType.MuzzleSpark] = new MuzzleSparkClientEffect();
         //   _types[EClientEffectType.DefaultHit] = new DefaultHitClientEffect();
            _types[EClientEffectType.GrenadeExplosion] = new GrenadeExplosionEffect();
            _types[EClientEffectType.FlashBomb] = new FlashBombEffect();
            _types[EClientEffectType.FogBomb] = new FogBombEffect();
            _types[EClientEffectType.BurnBomb] = new BurnBombEffect();
            _types[EClientEffectType.PullBolt] = new PullBoltEffect();
            _types[EClientEffectType.SprayPrint] = new SprayClientEffect();
        //    _types[EClientEffectType.ShieldHit] = new ShieldHitClientEffect();
        }

        public static IEffectLogic CreateEffectLogic(int type, Contexts contexts, int effectId = 0)
        {
            // if (type >= _types.Count)
            // {
            //     throw new Exception(String.Format("effect logic not registered {0}", type));
            // }

            var logic = _types[(EClientEffectType)type];
            if(null != logic)
            {
                logic.SetContexts(contexts);
                logic.OnCreate((EClientEffectType)type, effectId);
            }
            return logic;
        }
    }
}