using Assets.Utils.Configuration;
using System;
using Core.Utils;
using UnityEngine;
using WeaponConfigNs;
using Utils.Singleton;
using Utils.Configuration;
using XmlConfig;
using App.Shared.Util;
using App.Shared.GameModules.Weapon;
using Entitas;

namespace App.Shared.Audio
{
    public class GameAudioMedium
    {

        private static readonly LoggerAdapter audioLogger = new LoggerAdapter(typeof(AKAudioDispatcher));

        public static void PlayWeaponAudio(int weaponId,GameObject target, Func<AudioWeaponItem, int> propertyFilter)
        {
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId, propertyFilter);
            if(evtConfig != null && AKAudioEntry.Dispatcher != null)
                AKAudioEntry.Dispatcher.PostEvent(evtConfig, target);

        }
        /// <summary>
        /// 枪械切换
        /// </summary>
        /// <param name="weaponCfg"></param>
        public static void SwitchFireModelAudio(EFireMode model,GameObject target)
        {
#if UNITY_EDITOR
            if (AudioInfluence.IsForbidden) return;
#endif
            AudioGrp_ShotModelIndex shotModelIndex = model.ToAudioGrpIndex();
            if (AKAudioEntry.Dispatcher != null)
                AKAudioEntry.Dispatcher.SetSwitch(target, shotModelIndex);

        }

        public static void PostAutoRegisterGameObjAudio(Vector3 position, bool createObject)
        {

        }
    }
}