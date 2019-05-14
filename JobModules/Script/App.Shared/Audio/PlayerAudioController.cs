using App.Shared.Configuration;
using App.Shared.Player.Events;
using App.Shared.Terrains;
using Core;
using Core.Event;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

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

        public void Update(IUserCmd cmd)
        {
            var listenerMgr = AudioEntry.ListenerManager;
            if (!PlayerObject || AudioEntry.ListenerManager == null)
                return;
            if (!listenerMgr.Initialized)
            {
                listenerMgr.SetPartent(PlayerObject.transform);
            }

            if (cmd.FilteredInput.IsInput(EPlayerInput.IsCrouch))
            {
                var states = entity.StateInteractController().GetCurrStates();
                if (states.Contains(EPlayerState.Stand) || states.Contains(EPlayerState.Prone))
                    PlaySimpleAudio(EAudioUniqueId.Crouch, true);
                else
                    PlaySimpleAudio(EAudioUniqueId.CrouchToStand, true);
            }

#if UNITY_EDITOR
            listenerMgr.ThdViewEmitter.transform.localPosition = GlobalConst.ThrdEmitterDistanceDelta;
            listenerMgr.ThdViewEmitter.transform.LookAt(listenerMgr.DefaultListenerObj.transform);
            listenerMgr.FstViewEmitter.transform.localPosition = GlobalConst.FstEmitterDistanceDelta;
            listenerMgr.FstViewEmitter.transform.LookAt(listenerMgr.DefaultListenerObj.transform);
#endif
        }

        private AkGameObj EmitterObject
        {
            get { return AudioEntry.ListenerManager.GetEmitterObject(entity.cameraStateNew.IsThird()); }
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

        public void PlayJumpStepAudio()
        {
            var footMatType = FootMatType;
            GameAudioMedia.PlayJumpstepAudio(footMatType, EmitterObject);
            AudioJumpstepEvent audioEvent =
                (AudioJumpstepEvent) EventInfos.Instance.Allocate(EEventType.AJumpstep, false);
            audioEvent.Initialize(FootMatType,
                new Vector3(PlayerObject.transform.position.x, 0, PlayerObject.transform.position.z),
                PlayerObject.transform.eulerAngles);
            entity.localEvents.Events.AddEvent(audioEvent);
        }

        public void PlayFootstepAudio(AudioGrp_Footstep stepState)

        {
            var footMatType = FootMatType;
            GameAudioMedia.PlayFootstepAudio(stepState, footMatType, EmitterObject);
            AudioFootstepEvent audioEvent =
                (AudioFootstepEvent) EventInfos.Instance.Allocate(EEventType.AFootstep, false);
            audioEvent.Initialize(stepState, FootMatType,
                new Vector3(PlayerObject.transform.position.x, 0, PlayerObject.transform.position.z),
                PlayerObject.transform.eulerAngles);
            entity.localEvents.Events.AddEvent(audioEvent);
        }

        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAudioController));

        private GameObject PlayerObject
        {
            get { return entity.appearanceInterface.Appearance.CharacterP1; }
        }

        private GameObject WeaponObject
        {
            get { return entity.appearanceInterface.Appearance.GetWeaponP1InHand() ?? PlayerObject; }
        }

        private bool IsOnTrigger;

        public void PlayEmptyFireAudio()
        {
            if (!IsOnTrigger)
            {
                IsOnTrigger = true;
                GameAudioMedia.PlayEventAudio((int) EAudioUniqueId.EmptyFire, EmitterObject);
            }
        }

        public void StopFireTrigger()
        {
            if (IsOnTrigger)
                IsOnTrigger = false;
        }

        public void PlaySimpleAudio(EAudioUniqueId uniqueId, bool sync = false)
        {
            GameAudioMedia.PlayEventAudio((int) uniqueId, EmitterObject);
            if (sync)
            {
                AudioDefaultEvent defaultEvent =
                    EventInfos.Instance.Allocate(EEventType.ADefault, false) as AudioDefaultEvent;
                defaultEvent.Initialize(uniqueId, WeaponObject.transform.position, WeaponObject.transform.eulerAngles);
                entity.localEvents.Events.AddEvent(defaultEvent);
            }
        }

        public void PlayFireAudio(int weaponId, AudioGrp_ShotMode shotMode)
        {
            GameAudioMedia.PlayWeaponFireAudio(weaponId, EmitterObject, shotMode);
            AudioWeaponFireEvent fireEvent =
                EventInfos.Instance.Allocate(EEventType.AWeaponFire, false) as AudioWeaponFireEvent;

            fireEvent.Initialize(shotMode, weaponId, WeaponObject.transform.position,
                WeaponObject.transform.eulerAngles);
            entity.localEvents.Events.AddEvent(fireEvent);
        }

        public void PlayMeleeAttackAudio(int weaponId, int attackType)
        {
            int audioEventId = 0;
            GameAudioMedia.PlayMeleeAttack(weaponId, (AudioGrp_MeleeAttack) attackType, EmitterObject,
                ref audioEventId);
            if (audioEventId > 0)
            {
                AudioMeleeAtkEvent audioEvent =
                    EventInfos.Instance.Allocate(EEventType.AMeleeAttack, false) as AudioMeleeAtkEvent;
                audioEvent.Initialize(audioEventId, attackType, WeaponObject.transform.position,
                    WeaponObject.transform.eulerAngles);
                entity.localEvents.Events.AddEvent(audioEvent);
            }
        }

        public void PlayPullBoltAudio(int weaponId)
        {
            PlayReloadAuido(weaponId, AudioGrp_Magazine.PullboltOnly);
        }

        public void PlayReloadBulletAudio(int weaponId)
        {
            PlayReloadAuido(weaponId, AudioGrp_Magazine.FillBulletOnly);
        }

        public void PlayReloadAudio(int weaponId, bool emptyReload)
        {
            if (emptyReload)
                PlayReloadAuido(weaponId, AudioGrp_Magazine.MagizineAndPull);
            else
                PlayReloadAuido(weaponId, AudioGrp_Magazine.MagizineOnly);
        }

        private void PlayReloadAuido(int weaponId, AudioGrp_Magazine magizine)
        {
            GameAudioMedia.PlayWeaponReloadAudio(weaponId, magizine, EmitterObject);
            AudioPullboltEvent pullboltEvent =
                EventInfos.Instance.Allocate(EEventType.APullbolt, false) as AudioPullboltEvent;
            pullboltEvent.Initialize(magizine, weaponId, WeaponObject.transform.position,
                WeaponObject.transform.eulerAngles);
            entity.localEvents.Events.AddEvent(pullboltEvent);
        }

        public void StopPullBoltAudio(int configId)
        {
            GameAudioMedia.StopReloadAudio(configId, EmitterObject);
        }
    }
}