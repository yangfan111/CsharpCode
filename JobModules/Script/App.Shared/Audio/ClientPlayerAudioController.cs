using App.Shared.Configuration;
using App.Shared.Player.Events;
using Core;
using Core.Event;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.AssetManager;
using XmlConfig;

namespace App.Shared.Audio
{
    public class ClientPlayerAudioController : PlayerAudioControllerBase
    {
        public override void LoadMapAmbient(IUnityAssetManager assetManager)
        {
            MapAmbInfo ambInfo;
            Wwise_IDs.GetMapAmb(mapId,out ambInfo);
            if(ambInfo.Id>0 && !string.IsNullOrEmpty(ambInfo.EnvironmentAmb))
                assetManager.LoadAssetAsync(entity,new AssetInfo(MapAmbInfo.AssetName, ambInfo.EnvironmentAmb), 
                OnLoadMapAmbientSucess);
        }

        private void OnLoadMapAmbientSucess(PlayerEntity playerEntity, UnityObject unityObject)
        {
            DebugUtil.MyLog("Load Map Amb Sucesss");
        }
        
        public override void StopSwimAudio()
        {
            var audioMgr = AkSoundEngineController.AudioMgrGetter;
            if (entity.playerAudio.InWaterState && audioMgr != null && !SharedConfig.IsMute)
            {
                GameAudioMedia.PlayEventAudio((int) EAudioUniqueId.StopSwim, audioMgr.battleListener.FstViewEmitter);
                GameAudioMedia.PlayEventAudio((int) EAudioUniqueId.StopSwim, audioMgr.battleListener.FstViewEmitter);
                GameAudioMedia.PlayEventAudio((int) EAudioUniqueId.StopSwim2, audioMgr.battleListener.ThdViewEmitter);
                GameAudioMedia.PlayEventAudio((int) EAudioUniqueId.StopSwim2, audioMgr.battleListener.ThdViewEmitter);
                entity.playerAudio.InWaterState = false;
            }
        }
        public override void Update(IUserCmd cmd)
        {
            if (entity.playerClientUpdate.HasFootstepFrame)
                entity.playerClientUpdate.FootstepFrameGroup = (short) AudioGrp_Footstep.None;
            var audioMgr = AkSoundEngineController.AudioMgrGetter;
            if (audioMgr == null)
                return;
            if (!audioMgr.battleListener.HasParent && PlayerObject)
            {
                audioMgr.battleListener.SetPartent(PlayerObject.transform);
                MapAmbInfo ambInfo;
                Wwise_IDs.GetMapAmb(mapId,out ambInfo);
                ambInfo.PlayAmb();
            }
            audioMgr.PlayAmbient();    
           
           base.Update(cmd);

#if UNITY_EDITOR
            audioMgr.battleListener.ThdViewEmitter.transform.localPosition = GlobalConst.ThrdEmitterDistanceDelta;
            audioMgr.battleListener.ThdViewEmitter.transform.LookAt(audioMgr.battleListener.DefaultListenerObj.transform);
            audioMgr.battleListener.FstViewEmitter.transform.localPosition = GlobalConst.FstEmitterDistanceDelta;
            audioMgr.battleListener.FstViewEmitter.transform.LookAt(audioMgr.battleListener.DefaultListenerObj.transform);
#endif
        }


        public override void PlayDeadAudio()
        {
            PlaySimpleAudio(EAudioUniqueId.FlashDizzyStop);
            PlaySimpleAudio(EAudioUniqueId.MagazineStop);
        }

        private AkGameObj EmitterObject
        {
            get { return AkSoundEngineController.AudioMgrGetter.battleListener.GetEmitterObject(entity.cameraStateNew.IsThird()); }
        }


        public override void PlayJumpStepAudio()
        {
            if (AkSoundEngineController.AudioMgrGetter == null)
                return;
            var footMatType = FootMatType;
            GameAudioMedia.PlayJumpstepAudio(footMatType, EmitterObject);
        }

        public override void PlayFootstepAudioC(AudioGrp_Footstep stepState)
        {

            if (AkSoundEngineController.AudioMgrGetter == null)
                return;
            var footMatType = FootMatType;
            GameAudioMedia.PlayFootstepAudio(stepState, footMatType, EmitterObject);
            entity.playerClientUpdate.FootstepFrameGroup = (byte)stepState;
//                AudioFootstepEvent audioEvent =(AudioFootstepEvent) EventInfos.Instance.Allocate(EEventType.AFootstep, false);ErrorCode_SelectedChildNotAvailable
//                audioEvent.Initialize(stepState, FootMatType,
//                    new Vector3(PlayerObject.transform.position.x, 0, PlayerObject.transform.position.z),
//                    PlayerObject.transform.eulerAngles);
//                entity.localEvents.Events.AddEvent(audioEvent);
        }

        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(ClientPlayerAudioController));

        private GameObject PlayerObject
        {
            get { return entity.appearanceInterface.Appearance.CharacterP1; }
        }


        private bool IsOnTrigger;

        public override void PlayEmptyFireAudio()
        {
            if (AkSoundEngineController.AudioMgrGetter == null||SharedConfig.IsMute)
                return;
            if (!IsOnTrigger)
            {
                IsOnTrigger = true;
                GameAudioMedia.PlayEventAudio((int) EAudioUniqueId.EmptyFire, EmitterObject);
            }
        }

        public override void StopFireTrigger()
        {
            if (AkSoundEngineController.AudioMgrGetter == null||SharedConfig.IsMute)
                return;
            if (IsOnTrigger)
                IsOnTrigger = false;
        }

        public override void PlaySimpleAudio(EAudioUniqueId uniqueId, bool sync = false)
        {
            if (AkSoundEngineController.AudioMgrGetter == null||SharedConfig.IsMute)
                return;
            GameAudioMedia.PlayEventAudio((int) uniqueId, EmitterObject);
        }
    

        public override void PlayFireAudio(int weaponId, AudioGrp_ShotMode shotMode)
        {
            if (AkSoundEngineController.AudioMgrGetter == null ||SharedConfig.IsMute)
                return;
            GameAudioMedia.PlayWeaponFireAudio(weaponId, EmitterObject, shotMode);
        }

        public override void PlayMeleeAttackAudio(int weaponId, int attackType)
        {
            if (AkSoundEngineController.AudioMgrGetter == null||SharedConfig.IsMute)
                return;
            int audioEventId = 0;
            GameAudioMedia.PlayMeleeAttack(weaponId, (AudioGrp_MeleeAttack) attackType, EmitterObject,
                ref audioEventId);
        }


        public override void PlayReloadAuido(int weaponId, AudioGrp_Magazine magizine, float magizineSpeed)
        {
            if (AkSoundEngineController.AudioMgrGetter == null||SharedConfig.IsMute)
                return;
            AkSoundEngine.SetRTPCValue(Wwise_IDs.Gun_magazine_speed, magizineSpeed, EmitterObject.gameObject);
            DebugUtil.MyLog("SetRTPC:"+magizineSpeed);
            GameAudioMedia.PlayWeaponReloadAudio(weaponId, magizine, EmitterObject);
        }

        public override void StopPullBoltAudio(int configId)
        {
            if (AkSoundEngineController.AudioMgrGetter == null ||SharedConfig.IsMute)
                return;
            GameAudioMedia.PlayEventAudio((int)EAudioUniqueId.MagazineStop, EmitterObject);
        }
    }
}