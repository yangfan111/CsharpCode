using Assets.Utils.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WeaponConfigNs;
using UnityEngine;
using Core.Configuration;

namespace Core.Audio
{
    public class GameAudioMedium
    {

        /// <summary>
        /// 枪械开火
        /// </summary>
        /// <param name="weaponState"></param>
        public static void PerformAudioOnGunFire(WeaponLogic.IPlayerWeaponState weaponState)
        {
            NewWeaponConfigItem weaponCfg = NewWeaponConfigManager.Instance.GetConfigById(weaponState.CurrentWeapon);
            AKAudioEntry.AudioAssert(weaponCfg != null, string.Format("weapon config id [{0}] not find", weaponState.CurrentWeapon));
            //假装有event
            int eventId = 1;
            AKAudioEntry.Dispatcher.PostEvent(eventId, weaponState.CurrentWeaponGo);
        }
        /// <summary>
        /// 枪械切换
        /// </summary>
        /// <param name="weaponState"></param>
        public static void PeformAudioOnGunSwitch(WeaponLogic.IPlayerWeaponState weaponState)
        {
            NewWeaponConfigItem weaponCfg = NewWeaponConfigManager.Instance.GetConfigById(weaponState.CurrentWeapon);
            AKAudioEntry.AudioAssert(weaponCfg != null, string.Format("weapon config id [{0}] not find", weaponState.CurrentWeapon));
            //假装有event
            int eventId = 1;
            //AKAudioEntry.Dispatcher.PostEvent(eventId, weaponState.CurrentWeaponGo);
        }
        /// <summary>
        /// 枪械模式更换
        /// </summary>
        /// <param name="weaponState"></param>
        public static void PerformAudioOnGunModelSwitch(WeaponLogic.IPlayerWeaponState weaponState)
        {
            NewWeaponConfigItem weaponCfg = NewWeaponConfigManager.Instance.GetConfigById(weaponState.CurrentWeapon);
            //   var fireModelCfg = WeaponConfigManager.Instance.GetFireModeCountById(weaponState.CurrentWeapon);
            AKEventCfg evtCfg = AudioConfigSimulator.SimAKEventCfg1();
            string configedState = "Gun_shot_mode_type_continue";
            AKAudioEntry.Dispatcher.VarySwitchState(evtCfg.switchGroup, configedState, weaponState.CurrentWeaponGo);

        }
    }
}