using App.Shared.Audio;
using App.Shared.Configuration;
using App.Shared.Terrains;
using Core;
using Core.Event;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private static readonly LoggerAdapter audioLogger = new LoggerAdapter(typeof(AKAudioDispatcher));
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
        //    audioLogger.Info("Auido play once");
        }
        public static void PlayWeaponFireAudio(int weaponId, Vector3 firePos,AudioGrp_ShotMode shotMode)
        {
            if (SharedConfig.IsServer)
                return;
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId,(item)=>item.Fire);
            if (evtConfig != null && AKAudioEntry.Dispatcher != null)
            {
                var target = audioObjectGenerator.GetAudioEmitter();
                target.transform.position = firePos;
                target.SetActive(true);
                AKAudioEntry.Dispatcher.SetSwitch(target,shotMode);
                AKAudioEntry.Dispatcher.PostEvent(evtConfig, target);
                audioLogger.Info("Wise Fire Once");
                
              
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
        private static IMyTerrain terrainConfig;

        private static IMyTerrain GetTerrainConfig(int sceneId)
        {
            if(terrainConfig == null ||terrainConfig._mapId != sceneId)       
                terrainConfig = SingletonManager.Get<TerrainManager>().GetTerrain(sceneId);
            return terrainConfig;
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
        public static void PlayBulletHitEnvironmentAudio(AudioGrp_HitMatType hitMatType,Vector3 Position)
        {
            if (BulletFireEventConfig == null)
                return;
            var target = audioObjectGenerator.GetAudioEmitter();
            target.transform.position = Position;
            target.SetActive(true);
            AKAudioEntry.Dispatcher.SetSwitch(target,hitMatType);
            AKAudioEntry.Dispatcher.PostEvent(BulletFireEventConfig, target);
        }

        public static bool PlayStepEnvironmentAudio(AudioGrp_Footstep sourceType, Vector3 Position)
        {
            if (SharedConfig.IsServer || AKAudioEntry.Dispatcher == null|| sourceType == AudioGrp_Footstep.None)
                return false;
          
            var target = audioObjectGenerator.GetAudioEmitter();
            target.transform.position = Position;
            target.SetActive(true);
            int sceneId = SingletonManager.Get<MapConfigManager>().SceneParameters.Id;
            TerrainMatOriginType matType = (TerrainMatOriginType)GetTerrainConfig(sceneId).GetTerrainPositionMatType(Position);
            AudioGrp_MatIndex matGrpIndex = matType.ToAudioMatGrp();
            AKAudioEntry.Dispatcher.SetSwitch(target, matGrpIndex);
            AKAudioEntry.Dispatcher.SetSwitch(target, sourceType);
        //    audioLogger.InfoFormat("AKAudioEntry.Dispatcher.SetSwitch:{0},{1}",matGrpIndex,sourceType);
            AKAudioEntry.Dispatcher.PostEvent(FootStepEvent, target);
            audioLogger.Info("wise step once");
            return true;
        }
       
    
        /// <summary>
        /// 枪械模式切换
        /// </summary>
        /// <param name="weaponCfg"></param>
        public static void SwitchFireModelAudio(EFireMode model, GameObject target)
        {
            if (SharedConfig.IsServer)
                return;
#if UNITY_EDITOR
            if (AudioInfluence.IsForbidden) return;
#endif
           
            AudioGrp_ShotMode shotMode = model.ToAudioGrpShotMode();
            if (AKAudioEntry.Dispatcher != null)
                AKAudioEntry.Dispatcher.SetSwitch(target, shotMode);
        }

        public static void PostAutoRegisterGameObjAudio(Vector3 position, bool createObject)
        {
        }
    }
}
