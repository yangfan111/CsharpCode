using App.Shared.Audio.WiseIntegration;
using App.Shared.Player;
using Core;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System;
using App.Shared.GameModules.Player;
using UnityEngine;
using Utils.AssetManager;
using XmlConfig;

namespace App.Shared.Audio
{
    public class ClientPlayerAudioController : PlayerAudioControllerBase
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(ClientPlayerAudioController));

        private Func<AudioGrp_FootMatType> footstepCmdExec;
        private bool IsOnTrigger;

        private AkGameObj EmitterObject
        {
            get
            {
                try
                {
                    return AkSoundEngineController.AudioMgrGetter.battleListener.GetEmitterObject(true);
                }
                catch (System.Exception e)
                {
                    AudioUtil.Logger.ErrorFormat("Audio emitter exception {0}",e.Message);
                }

                return null;
            }
        }

        private GameObject PlayerObject
        {

            get
            {
                return entity.RootGo();
            }
        }

        public override void Initialize(PlayerEntity entity, int mapId)
        {
            base.Initialize(entity, mapId);
            footstepCmdExec = GetFootMatType;
        }

        public override void SetGMFootstep(bool use)
        {
            if (use)
                footstepCmdExec = GetGMFootMatType;
            else
                footstepCmdExec = GetFootMatType;
        }

        public AudioGrp_FootMatType GetGMFootMatType()
        {
            return (AudioGrp_FootMatType) GMVariable.AudioFoostepMatType;
        }
        /// <summary>
        /// 加载环境特效
        /// </summary>
        /// <param name="assetManager"></param>
        public override void LoadMapAmbient(IUnityAssetManager assetManager)
        {
            _logger.Info("Req Load Map Amb ");
            MapAmbInfo ambInfo;
            Wwise_IDs.GetMapAmb(mapId, out ambInfo);
            if (ambInfo.Id > 0 && !string.IsNullOrEmpty(ambInfo.EnvironmentAmb))
            {
                GameAudioMedia.LoadSimpleBank(string.Format("Map_{0}", mapId),AudioBank_LoadMode.Aync, (akresult) =>
                {
                    if (akresult.Sucess())
                    {
                        _logger.Info("try Load Map Amb "+ambInfo.EnvironmentAmb);
                        assetManager.LoadAssetAsync(entity, new AssetInfo(MapAmbInfo.AssetName, ambInfo.EnvironmentAmb),
                            OnLoadMapAmbientSucess);
                    }
                });
              
            }
              
        }

        private void OnLoadMapAmbientSucess(PlayerEntity playerEntity, UnityObject unityObject)
        {
            _logger.InfoFormat("Load Map Amb Sucesss {0}",unityObject);
        }

        public override void StopSwimAudio()
        {
            var audioMgr = AkSoundEngineController.AudioMgrGetter;
            if (entity.playerAudio.InWaterState && audioMgr != null)
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
                Wwise_IDs.GetMapAmb(mapId, out ambInfo);
                ambInfo.PlayAmb();
            }

            audioMgr.PlayAmbient();
            /*if (!entity.WeaponController().IsWeaponSlotEmpty(EWeaponSlotType.TacticWeapon))
            {
                if (Time.time - entity.playerAudio.C4UnityStamp >= 1)
                {
                    entity.playerAudio.C4UnityStamp = Time.time;

                    PlaySimpleAudio(EAudioUniqueId.C4_Alarm);
                }
            }*/
            //
            // if (entity.StateInteractController().UserInput.IsInput(EPlayerInput.IsReloadInterrupt))
            // {
            //     StopPullBoltAudio();
            // }

            base.Update(cmd);

#if UNITY_EDITOR
            audioMgr.battleListener.ThdViewEmitter.transform.localPosition = GlobalConst.ThrdEmitterDistanceDelta;
            audioMgr.battleListener.ThdViewEmitter.transform.LookAt(
                audioMgr.battleListener.DefaultListenerObj.transform);
            audioMgr.battleListener.FstViewEmitter.transform.localPosition = GlobalConst.FstEmitterDistanceDelta;
            audioMgr.battleListener.FstViewEmitter.transform.LookAt(
                audioMgr.battleListener.DefaultListenerObj.transform);
#endif
        }

        public override void PlayDeadAudio()
        {
            PlaySimpleAudio(EAudioUniqueId.FlashDizzyStop);
            PlaySimpleAudio(EAudioUniqueId.MagazineStop);
        }


        public override void PlayJumpStepAudio()
        {
            if (AkSoundEngineController.AudioMgrGetter == null)
                return;
            var footMatType = footstepCmdExec();
            GameAudioMedia.PlayJumpstepAudio(footMatType, EmitterObject);
        }

        public override void PlayFootstepAudioC(AudioGrp_Footstep stepState)
        {
            if (AkSoundEngineController.AudioMgrGetter == null)
                return;
            var footMatType = footstepCmdExec();
            var softVal     = entity.StateInteractController().GetCurrStates().Contains(EPlayerState.Walk) ? 1 : 0;
            AkSoundEngine.SetRTPCValue(Wwise_IDs.FootstepSoft, softVal, EmitterObject.gameObject);
            GameAudioMedia.PlayFootstepAudio(stepState, footMatType, EmitterObject);
            entity.playerClientUpdate.FootstepFrameGroup = (byte) stepState;
            entity.playerClientUpdate.LastMatType = (byte) footMatType;
            //                AudioFootstepEvent audioEvent =(AudioFootstepEvent) EventInfos.Instance.Allocate(EEventType.AFootstep, false);ErrorCode_SelectedChildNotAvailable
            //                audioEvent.Initialize(stepState, FootMatType,
            //                    new Vector3(PlayerObject.transform.position.x, 0, PlayerObject.transform.position.z),
            //                    PlayerObject.transform.eulerAngles);
            //                entity.localEvents.Events.AddEvent(audioEvent);
        }

        public override void PlayEmptyFireAudio()
        {
            if (AkSoundEngineController.AudioMgrGetter == null )
                return;
            if (!IsOnTrigger)
            {
                IsOnTrigger = true;
                GameAudioMedia.PlayEventAudio((int) EAudioUniqueId.EmptyFire, EmitterObject);
            }
        }

        public override void StopFireTrigger()
        {
            if (AkSoundEngineController.AudioMgrGetter == null)
                return;
            if (IsOnTrigger)
                IsOnTrigger = false;
        }

        public override void PlaySimpleAudio(EAudioUniqueId uniqueId, bool sync = false)
        {
            if (AkSoundEngineController.AudioMgrGetter == null || SharedConfig.IsMute)
                return;
            switch (uniqueId)
            {
                case EAudioUniqueId.Prone:
                case EAudioUniqueId.ProneToStand:
                case EAudioUniqueId.WeaponIn:
                case EAudioUniqueId.WeaponOff:
                    StopPullBoltAudio();
                    break;
                case EAudioUniqueId.FlashDizzyStop:
                    AkSoundEngine.SetRTPCValue(Wwise_IDs.FlashBomb, 0);
                    break;
            }

            GameAudioMedia.PlayEventAudio((int) uniqueId, EmitterObject);
        }


        public override void PlayFireAudio(int weaponId, AudioGrp_ShotMode shotMode)
        {
            if (AkSoundEngineController.AudioMgrGetter == null )
                return;
            GameAudioMedia.PlayWeaponFireAudio(weaponId, EmitterObject, shotMode);
        }

        public override void PlayMeleeAttackAudio(int weaponId, int attackType)
        {
            if (AkSoundEngineController.AudioMgrGetter == null )
                return;
            int audioEventId = 0;
            GameAudioMedia.PlayMeleeAttack(weaponId, (AudioGrp_MeleeAttack) attackType, EmitterObject,
                ref audioEventId);
        }
        public override void PlayDizzyAudio(float dis, float disMax)
        {
            var emitterObject = EmitterObject;
            var audioPlayComponent = emitterObject.GetComponent<AkRTPCPlayComponent>();
            if (!audioPlayComponent)
                audioPlayComponent = emitterObject.gameObject.AddComponent<AkRTPCPlayComponent>();
            float dizzyDuration = dis>0? 5* Math.Max(0, ((disMax - dis)) / disMax):5;
          //  DebugUtil.MyLog("dizzyDuration:"+dizzyDuration + "Dis:"+Math.Max(40-dis,0));
            audioPlayComponent.Initialize(Wwise_IDs.FlashBomb,dizzyDuration,true, () =>
                {
                    PlaySimpleAudio(EAudioUniqueId.FlashDizzyStop);
                });
            PlaySimpleAudio(EAudioUniqueId.FlashDizzy);
        }

        public override void PlayReloadAuido(int weaponId, AudioGrp_Magazine magizine, float magizineSpeed)
        {
            if (AkSoundEngineController.AudioMgrGetter == null )
                return;
            AkSoundEngine.SetRTPCValue(Wwise_IDs.GunMagazineSpeed, magizineSpeed, EmitterObject.gameObject);
            DebugUtil.MyLog("SetRTPC:" + magizineSpeed);
            GameAudioMedia.PlayWeaponReloadAudio(weaponId, magizine, EmitterObject);
        }

        public override void StopPullBoltAudio()
        {
            if (AkSoundEngineController.AudioMgrGetter == null)
                return;
            GameAudioMedia.PlayEventAudio((int) EAudioUniqueId.MagazineStop, EmitterObject);
        }
        
        public override void StopDefaultListener()
        {
            AkSoundEngineController.AudioMgrGetter.battleListener.SpatialListener.enabled = false;
            AkSoundEngineController.AudioMgrGetter.battleListener.DefaultListener.enabled = false;
        }
        
        public override void OpenDefaultListener()
        {
            AkSoundEngineController.AudioMgrGetter.battleListener.SpatialListener.enabled = true;
            AkSoundEngineController.AudioMgrGetter.battleListener.DefaultListener.enabled = true;
        }
    }
}