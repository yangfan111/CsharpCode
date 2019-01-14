using Assets.Utils.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WeaponConfigNs;
using UnityEngine;
using Core.Configuration;
using Utils.Singleton;
namespace Core.Audio
{
    public class GameAudioMedium
    {
        static int testWeaponEvent = 1;
        static string testWeaponModel= "Gun_shot_mode_type_single";

        /// <summary>
        /// 枪械开火
        /// </summary>
        /// <param name="weaponState"></param>
        public static void PerformOnGunFire(WeaponLogic.IPlayerWeaponState weaponState)
        {
            if (AudioInfluence.IsForbidden || AKAudioEntry.Dispatcher == null) return;
            NewWeaponConfigItem weaponCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponState.CurrentWeapon);
            AKAudioEntry.AudioAssert(weaponCfg != null, string.Format("weapon config id [{0}] not find", weaponState.CurrentWeapon));
            //假装有event
            AKAudioEntry.Dispatcher.PostEvent(testWeaponEvent, weaponState.CurrentWeaponGo);
        }
    
        /// <summary>
        /// 枪械切换
        /// </summary>
        /// <param name="weaponState"></param>
        public static void PerformOnGunSwitch(WeaponLogic.IPlayerWeaponState
            weaponState)
        {
            PerformOnGunSwitch(weaponState.CurrentWeapon);
        }
        public static void PerformOnGunSwitch(NewWeaponConfigItem weaponCfg)
        {
            if (AudioInfluence.IsForbidden || AKAudioEntry.Dispatcher == null) return;
            AKAudioEntry.AudioAssert(weaponCfg != null, string.Format("weapon config id [{0}] not find", weaponCfg.Id));
            //假装有event
            int eventId = 2;
            testWeaponEvent = testWeaponEvent == 1 ? 2 : 1;
            AKAudioEntry.Dispatcher.PrepareEvent(testWeaponEvent);
        }
        public static void PerformOnGunSwitch(int weaponId)
        {
            if (AudioInfluence.IsForbidden || AKAudioEntry.Dispatcher == null) return;
            NewWeaponConfigItem weaponCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponId);
            PerformOnGunSwitch(weaponCfg);
        }
        /// <summary>
        /// 枪械模式更换
        /// </summary>
        /// <param name="weaponState"></param>
        public static void PerformOnGunModelSwitch(CommonFireConfig comCfg, WeaponLogic.IPlayerWeaponState weaponState)
        {
            if (AudioInfluence.IsForbidden || AKAudioEntry.Dispatcher == null) return;
            // NewWeaponConfigItem weaponCfg = WeaponConfigManager.Instance.GetConfigById(weaponState.CurrentWeapon);
            //   var fireModelCfg = WeaponConfigManager.Instance.GetFireModeCountById(weaponState.CurrentWeapon);
            AKEventCfg evtCfg = AudioConfigSimulator.SimAKEventCfg1();
            testWeaponModel = testWeaponModel == "Gun_shot_mode_type_single" ? "Gun_shot_mode_type_triple" : "Gun_shot_mode_type_single";
            AKAudioEntry.Dispatcher.VarySwitchState(evtCfg.switchGroup, testWeaponModel, weaponState.CurrentWeaponGo);

        }
        public static void PostAutoRegisterGameObjAudio(Vector3 position,bool createObject)
        {

        }
    }
}