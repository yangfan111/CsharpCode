using App.Shared.Audio;
using App.Shared.Configuration;
using App.Shared.Terrains;
using Core;
using Core.Event;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using App.Shared.Player.Events;
using Core.ObjectPool;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="GameAudioMedia" />
    /// </summary>
    public class GameAudioMedia
    {
        
        private readonly static AudioObjectGenerator audioObjectGenerator = new AudioObjectGenerator();
        
        public static void Dispose()
        {
            audioObjectGenerator.Dispose();
        }
        public static void PlayWeaponSimpleAudio(int weaponId, GameObject target, Func<AudioWeaponItem, int> propertyFilter)
        {
            if (SharedConfig.IsServer)
                return;
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId, propertyFilter);
            if (evtConfig != null && AKAudioEntry.Dispatcher != null)
            {
                AKAudioEntry.Dispatcher.PostEvent(evtConfig, target);
              
            }
            else
            {
                AKAudioEntry.AudioLogger.ErrorFormat("Wise Audio Process Error,target:{0},evtCfg:{1}",target,evtConfig);    

            }
        //    audioLogger.Info("Auido play once");
        }

        public static void PlayWeaponFireAudio(int weaponId, Vector3 firePos,AudioGrp_ShotMode shotMode)
        {
            if (SharedConfig.IsServer)
                return;
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId,(item)=>item.Fire);
            var target = GetEmitter(firePos);
            if (evtConfig != null && target != null)
            {
                AKAudioEntry.Dispatcher.SetSwitch(target,shotMode);
                AKAudioEntry.Dispatcher.PostEvent(evtConfig, target);
            }
            else
            {
                AKAudioEntry.AudioLogger.ErrorFormat("Wise Audio Process Error,target:{0},evtCfg:{1}",target,evtConfig);    
            }
        }
        public static void PlayWeaponReloadAudio(int weaponId,AudioGrp_Magazine magazineIndex,  GameObject target)
        {
             if (SharedConfig.IsServer)
                return;
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId,(item)=>item.ReloadStart);
            if (evtConfig != null && AKAudioEntry.Dispatcher != null)
            {
                AKAudioEntry.Dispatcher.SetSwitch(target,magazineIndex);
                AKAudioEntry.Dispatcher.PostEvent(evtConfig, target);
              
            }
           // audioLogger.Info("Auido play once");
        }

        private static AudioEventItem footStepEventConfig;
        private static AudioEventItem FootStepEvent
        {
            get
            {
                if(footStepEventConfig == null)
                    footStepEventConfig = SingletonManager.Get<AudioEventManager>().FindById(GlobalConst.AudioEvt_Footstep);
                return footStepEventConfig;
            }
        }
        private static AudioEventItem bulletFireEventConfig;
        private static AudioEventItem BulletFireEventConfig
        {
            get
            {
                if(bulletFireEventConfig == null)
                    bulletFireEventConfig = SingletonManager.Get<AudioEventManager>().FindById(GlobalConst.AudioEvt_BulletHit);
                return bulletFireEventConfig;
            }
        }
        private static AudioEventItem bulletFlyEventConfig;
        private static AudioEventItem BulletFlyEventConfig
        {
            get
            {
                if(bulletFlyEventConfig == null)
                    bulletFlyEventConfig = SingletonManager.Get<AudioEventManager>().FindById(GlobalConst.AudioEvt_BulletFly);
                return bulletFlyEventConfig;
            }
        }
        private static GameObject GetEmitter(Vector3 Position)
        {
            var target = audioObjectGenerator.GetAudioEmitter();
            target.transform.position = Position;
            target.SetActive(true);
            return target;
        }

        public static void PlayBulletFlyAudio(GameObject target)
        {
            AKAudioEntry.Dispatcher.PostEvent(BulletFlyEventConfig, target);
        }
        public static void PlayBulletDropAudio(int eventId,AudioGrp_FootMatType dropMatType, Vector3 Position)
        {
            if (SharedConfig.IsServer) return;

            var target = GetEmitter(Position);
            var evtCfg = SingletonManager.Get<AudioEventManager>().FindById(eventId);
            if (target != null && evtCfg != null)
            {
           //     DebugUtil.MyLog(dropMatType);
                AKAudioEntry.Dispatcher.SetSwitch(target,dropMatType);
                AKAudioEntry.Dispatcher.PostEvent(evtCfg, target);
            }
            else
            {
                AKAudioEntry.AudioLogger.ErrorFormat("Wise Audio Process Error,target:{0},evtCfg:{1}",target,evtCfg);    
            }
        }
        public static void PlayHitEnvironmentAudio(AudioGrp_HitMatType hitMatType,Vector3 Position)
        {
            if (SharedConfig.IsServer) return;
            var target = GetEmitter(Position);
            AKAudioEntry.Dispatcher.SetSwitch(target,hitMatType);
            AKAudioEntry.Dispatcher.PostEvent(BulletFireEventConfig, target);
        }
        //播放自己的脚步音频
        public static bool PlayStepEnvironmentAudio(AudioGrp_Footstep sourceType,AudioGrp_FootMatType matType,GameObject target)
        {
            if (SharedConfig.IsServer || AKAudioEntry.Dispatcher == null|| sourceType == AudioGrp_Footstep.None)
                return false;
            AKAudioEntry.Dispatcher.SetSwitch(target, matType);
            AKAudioEntry.Dispatcher.SetSwitch(target, sourceType);
            AKAudioEntry.Dispatcher.PostEvent(FootStepEvent, target);
            return true;
        }
        public static bool PlayStepEnvironmentAudio(AudioEvent audioEvent)
        {
            if (SharedConfig.IsServer || AKAudioEntry.Dispatcher == null)
                return false;
          
            var target = GetEmitter(audioEvent.relatedPos);
            int sceneId = SingletonManager.Get<MapConfigManager>().SceneParameters.Id;
            if (target == null) return false;
            AKAudioEntry.Dispatcher.SetSwitch(target, audioEvent.footMatType);
            AKAudioEntry.Dispatcher.SetSwitch(target, audioEvent.footstepState);
        //    audioLogger.InfoFormat("AKAudioEntry.Dispatcher.SetSwitch:{0},{1}",matGrpIndex,sourceType);
            AKAudioEntry.Dispatcher.PostEvent(FootStepEvent, target);
            //audioLogger.Info("wise step once");
            return true;
        }
       
        public static void PostAutoRegisterGameObjAudio(Vector3 position, bool createObject)
        {
        }
    }
}
