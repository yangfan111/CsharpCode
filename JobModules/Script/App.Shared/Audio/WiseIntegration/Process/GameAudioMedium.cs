using Assets.Utils.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core.Utils;
using UnityEngine;
using WeaponConfigNs;
using Utils.Singleton;
using Utils.Configuration;
using XmlConfig;
using App.Shared.Util;

namespace App.Shared.Audio
{
    public class GameAudioMedium
    {

        private static readonly LoggerAdapter audioLogger = new LoggerAdapter(typeof(AKAudioDispatcher));


        /// <summary>
        /// 枪械开火
        /// </summary>
        /// <param name="weaponState"></param>
        public static void ProcessWeaponAudio(int weaponId, GameObject target, Func<AudioWeaponItem, int> propertyFilter)
        {
#if UNITY_EDITOR
            if (AudioInfluence.IsForbidden) return;
#endif
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId, propertyFilter);
            CommonUtil.WeakAssert(evtConfig != null, "AudioEventItem not find");
            AKRESULT ret = AKAudioEntry.Dispatcher.PostEvent(evtConfig, target);
            if (ret != AKRESULT.AK_Success)
            {
                ReportException(ret, () => throw new AudioFrameworkException(string.Format("result:{0},source:{1}", ret, evtConfig)));
            }
        }
        private static void ReportException(AKRESULT ret, Action onExcepProcess)
        {
            if (onExcepProcess == null)
                audioLogger.Error("AudioFramework:"+ret);
            else
                onExcepProcess();
        }
        /// <summary>
        /// 枪械切换
        /// </summary>
        /// <param name="weaponCfg"></param>
        public static void ProcessWeaponSwitch(int newWeapon,GameObject target)
        {
#if UNITY_EDITOR
            if (AudioInfluence.IsForbidden) return;

#endif
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(newWeapon,(item)=>item.SwitchIn);


            //假装有event
            //   AKAudioEntry.Dispatcher.PrepareEvent(testWeaponEvent);
        }
        /// <summary>
        /// 武器切换音效
        /// </summary>
        /// <param name="weaponId"></param>
        public static void PerformOnWeaponSwitch(int weaponId)
        {
#if UNITY_EDITOR
            if (AudioInfluence.IsForbidden) return;

#endif
            if (!AKAudioEntry.PrepareReady) return;
            NewWeaponConfigItem weaponCfg = SingletonManager.Get<WeaponConfigManager>().GetConfigById(weaponId);
            ProcessWeaponSwitch(weaponCfg);
        }
        /// <summary>
        /// 枪械模式更换
        /// </summary>
        /// <param name="weaponState"></param>
        public static void PerformOnFireModelSwitch(NewWeaponConfigItem config, GameObject weaponGo)
        {
#if UNITY_EDITOR
            if (AudioInfluence.IsForbidden) return;

#endif
            if (!AKAudioEntry.PrepareReady) return;
            // NewWeaponConfigItem weaponCfg = WeaponConfigManager.Instance.GetConfigById(weaponState.CurrentWeapon);
            //   var fireModelCfg = WeaponConfigManager.Instance.GetFireModeCountById(weaponState.CurrentWeapon);
            AKEventCfg evtCfg = AudioConfigSimulator.SimAKEventCfg1();
            testWeaponModel = testWeaponModel == "Gun_shot_mode_type_single" ? "Gun_shot_mode_type_triple" : "Gun_shot_mode_type_single";
            AKAudioEntry.Dispatcher.VarySwitchState(evtCfg.switchGroup, testWeaponModel, weaponGo);

        }
        public static void PostAutoRegisterGameObjAudio(Vector3 position, bool createObject)
        {

        }
    }
}