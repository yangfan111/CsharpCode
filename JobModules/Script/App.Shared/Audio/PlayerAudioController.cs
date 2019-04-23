using App.Shared.Configuration;
using App.Shared.Player.Events;
using App.Shared.Terrains;
using Core;
using Core.Event;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace App.Shared.Audio
{
  
    public class PlayerAudioController : ModuleLogicActivator<PlayerAudioController>
    {
        private PlayerEntity entity;


        public PlayerAudioController()
        {

        }

        public PlayerAudioController Value
        {
            get
            {
                if (AudioEntry.Dispatcher == null)
                    return null;
                return this;
            }
        }


        public void Initialize(PlayerEntity entity)
        {
            this.entity = entity;
        }

        public void Update()
        {
            var listenerMgr = AudioEntry.ListenerManager;
            if (!PlayerObject || listenerMgr == null)
                return;
            if (!listenerMgr.HasParent)
            {
                listenerMgr.SetPartent(PlayerObject.transform);
            }
#if UNITY_EDITOR
            listenerMgr.ThdViewEmitter.localPosition = GlobalConst.ThrdEmitterDistanceDelta;
            listenerMgr.ThdViewEmitter.LookAt(listenerMgr.DefaultListenerTrans);// = Quaternion.LookRotation(listenerMgr.DefaultListenerTrans.position - listenerMgr.ThdViewEmitter.transform.position);
      //      listenerMgr.ThdViewEmitter.localRotation = GlobalConst.ThrdEmitterRotationDelta;
            listenerMgr.FstViewEmitter.localPosition = GlobalConst.FstEmitterDistanceDelta;
            listenerMgr.FstViewEmitter.LookAt(listenerMgr.DefaultListenerTrans);
         //   listenerMgr.FstViewEmitter.localRotation = Quaternion.LookRotation(listenerMgr.DefaultListenerTrans.position - listenerMgr.FstViewEmitter.transform.position);
       //     listenerMgr.FstViewEmitter.localRotation = GlobalConst.FstEmitterRotationDelta;
#endif
        }

        private GameObject EmitterObject
        {
            get
            {
                 return entity.cameraStateNew.IsThird()?AudioEntry.ListenerManager.ThdViewEmitter.gameObject
                    : AudioEntry.ListenerManager.FstViewEmitter.gameObject;
            }
        }

        private IMyTerrain terrainConfig;

        private IMyTerrain GetTerrainConfig(int sceneId)
        {
            if (terrainConfig == null || terrainConfig._mapId != sceneId)
                terrainConfig = SingletonManager.Get<TerrainManager>().GetTerrain(sceneId);
            return terrainConfig;
        }

        private AudioGrp_FootMatType FootMatType
        {
            get
            {
                int sceneId = SingletonManager.Get<MapConfigManager>().SceneParameters.Id;
                TerrainMatOriginType matType = (TerrainMatOriginType) GetTerrainConfig(sceneId)
                    .GetTerrainPositionMatType(entity.position.Value);
                return matType.ToAudioMatGrp();
            }
        }

        public void PlayStepEnvironmentAudio(AudioGrp_Footstep stepState)

        {
            var footMatType = FootMatType;
            GameAudioMedia.PlayStepEnvironmentAudio(stepState, footMatType, EmitterObject);
            //     audioAgent.RefreshStepPlayTimestamp();
            AudioEvent audioEvent = (AudioEvent) EventInfos.Instance.Allocate(EEventType.BroadcastAudio, false);
            audioEvent.footstepState = stepState;
            audioEvent.footMatType   = footMatType;
            audioEvent.relatedPos    = entity.position.Value;
            entity.localEvents.Events.AddEvent(audioEvent);
        }

        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAudioController));

        public void PlayStepEnvironmentAudio(AudioEvent e)
        {
            //     GameAudioMedia.PlayStepEnvironmentAudio(e.footstepState, e.relatedPos, PlayerObject);
        }

//        private GameObject WeaponObject
//        {
//            get { return entity.appearanceInterface.Appearance.WeaponHandObject(); }
//        }
//
        private GameObject PlayerObject
        {
            get { return entity.appearanceInterface.Appearance.CharacterP1; }
        }

        public void PlaySwitchAuido(int weaponId, EInOrOff op)
        {
           
            if (op == EInOrOff.In)
            {
                GameAudioMedia.PlayWeaponSimpleAudio(weaponId, EmitterObject,
                    (config) => config.SwitchIn);
            }
            else
            {
                GameAudioMedia.PlayWeaponSimpleAudio(weaponId, EmitterObject,
                    (config) => config.SwitchOff);
            }
        }

        public void PlayFireAudio(int weaponId, AudioGrp_ShotMode shotMode, Vector3 position)
        {
 
            GameAudioMedia.PlayWeaponFireAudio(weaponId, EmitterObject, shotMode);
            var fireEvent = EventInfos.Instance.Allocate(EEventType.Fire, false) as FireEvent;
            fireEvent.fireWeaponId  = weaponId;
            fireEvent.audioFireMode = (int) shotMode;
            fireEvent.audioFirePos  = position;
            entity.localEvents.Events.AddEvent(fireEvent);
            //  GameAudioMedia.PlayWeaponFireAudio(weaponId, WeaponObject, shotMode);
        }


//        public void SwitchFireMode(EFireMode nextMode)
//        {
//            GameAudioMedia.SwitchFireModelAudio(nextMode, WeaponObject);
//            
//        }
        public void PlayPullBoltAudio(int configId)
        {
            GameAudioMedia.PlayWeaponReloadAudio(configId, AudioGrp_Magazine.PullboltOnly,
                EmitterObject);
        }
        public void StopPullBoltAudio(int configId)
        {
            GameAudioMedia.StopReloadAudio(configId,EmitterObject);
        }
        public void PlayReloadBulletAudio(int configId)
        {
//            if (audioAgent.RelatedAudio.ReloadedBulletLeft < 1)
//                return;
            //audioAgent.RelatedAudio.ReloadedBulletLeft -= 1;
            GameAudioMedia.PlayWeaponReloadAudio(configId, AudioGrp_Magazine.FillBulletOnly,
                EmitterObject);
        }

        public void SetReloadBulletAudioCount(int reloadedBulletCount)
        {
            //   audioAgent.RelatedAudio.ReloadedBulletLeft = reloadedBulletCount ;
            //  PlayReloadBulletAudio(configId);
        }

        public void PlayReloadAudio(int configId, bool emptyReload)
        {
         
            if (emptyReload)
                GameAudioMedia.PlayWeaponReloadAudio(configId, AudioGrp_Magazine.MagizineAndPull,
                    EmitterObject);
            else
                GameAudioMedia.PlayWeaponReloadAudio(configId, AudioGrp_Magazine.MagizineOnly,
                    EmitterObject);
            //if(reloadCount>0)
//                GameAudioMedia.PlayWeaponReloadAudio(configId, AudioGrp_Magazine.MagizineAndPull,
//                WeaponObject);
//            else
//                GameAudioMedia.PlayWeaponReloadAudio(configId, AudioGrp_Magazine.MagizineOnly,
//                    WeaponObject);
        }
    }
}