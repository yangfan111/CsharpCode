using App.Shared.Player.Events;
using App.Shared.Util;
using Core;
using Core.Utils;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.Audio
{
    /// <summary>
    ///     Defines the <see cref="GameAudioMedia" />
    /// </summary>
    public class GameAudioMedia
    {
        public static readonly LoggerAdapter AudioLogger = new LoggerAdapter("GameAudioMedia");

        internal static AudioDispatcher Dispatcher { private get; set; }


        private static AudioEventItem GetEventConfig(int id)
        {
            return SingletonManager.Get<AudioEventManager>().FindById(id);
        }

        private static AudioEventItem GetEventConfig(EAudioUniqueId uniqueId)
        {
            return GetEventConfig(((int) uniqueId));
        }

        public static void Dispose()
        {
            LocalObjectGenerator.Dispose();
        }

        public static void Prepare()
        {
            if (Dispatcher == null)
            {
                AudioBankLoader bankResLoader = new AudioBankLoader();
                Dispatcher = new AudioDispatcher(bankResLoader);
                AKRESULT result = bankResLoader.Initialize();

                if (result != AKRESULT.AK_Success)
                {
                    AudioLogger.Error("Sound bank not initialized");
                    return;
                }

                DebugUtil.MyLog("Audio media initialize sucess", DebugUtil.DebugColor.Green);
                AudioLogger.Info("Audio media initialize sucess");
            }
            else
            {
                Dispatcher.Free();
            }
        }

        internal static void PlayWeaponFireModeAudio()
        {
        }

        private static int FilterFire(AudioWeaponItem item)
        {
            return item.Fire;
        }

        private static int FilterReload(AudioWeaponItem item)
        {
            return item.ReloadStart;
        }

        private static int FilterLeft1(AudioWeaponItem item)
        {
            return item.Left1;
        }

        //        private static int FilterLeft2(AudioWeaponItem item)
        //        {
        //            return item.Left2;
        //        }
        //
        //        private static int FilterRight(AudioWeaponItem item)
        //        {
        //            return item.Right;
        //        }

        internal static void PlayWeaponFireAudio(AudioWeaponFireEvent fireEvent)
        {
            if (SharedConfig.IsServer || AkSoundEngineController.AudioMgrGetter == null)
                return;
            var target = GetEmitter(fireEvent);

            //        DebugUtil.MyLog("Play Fire:{0},is Mute:{1}",target.transform.position,target.IsMute);
            PlayWeaponFireAudio(fireEvent.WeaponId, target, fireEvent.ShotMode);
        }

        internal static void PlayWeaponReloadAudio(AudioPullboltEvent pullboltEvent)
        {
            if (SharedConfig.IsServer || AkSoundEngineController.AudioMgrGetter == null)
                return;
            var target = GetEmitter(pullboltEvent);
            PlayWeaponReloadAudio(pullboltEvent.WeaponId, pullboltEvent.Magazine, target);
        }

        internal static void PlayWeaponFireAudio(int weaponId, Vector3 pos, AudioGrp_ShotMode shotMode)
        {
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId, FilterFire);
            var            target    = GetEmitter(pos);
            Dispatcher.SetSwitch(target, shotMode);
            Dispatcher.PostEvent(evtConfig, target);
        }

        internal static void PlayWeaponFireAudio(int weaponId, AkGameObj target, AudioGrp_ShotMode shotMode)
        {
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId, FilterFire);
            if (evtConfig != null && target)
            {
                AuidoGrp_RefShotMode refShotMode = (shotMode == AudioGrp_ShotMode.Silencer)
                                ? AuidoGrp_RefShotMode.Silencer
                                : AuidoGrp_RefShotMode.Default;
                Dispatcher.SetSwitch(target, shotMode);
                Dispatcher.SetSwitch(target, refShotMode);


                Dispatcher.PostEvent(evtConfig, target);
            }
            else
            {
                AudioUtil.Logger.InfoFormat("Audio Post event failed,target:{0},evtCfg:{1}", target, weaponId);
            }
        }

        //TODO:同步
        internal static void PlayWeaponReloadAudio(int weaponId, AudioGrp_Magazine magazineIndex, AkGameObj target)
        {
            AudioEventItem evtConfig =
                            SingletonManager.Get<AudioWeaponManager>().FindById(weaponId, item => item.ReloadStart);
            if (evtConfig != null && target)
            {
                Dispatcher.SetSwitch(target, magazineIndex);
                Dispatcher.PostEvent(evtConfig, target);
            }
            else
            {
                AudioUtil.Logger.ErrorFormat("Audio Post event failed,target:{0},evtCfg:{1}", target, weaponId);
            }


            // audioLogger.Info("Auido play once");
        }


        internal static void PlayMeleeAttack(int weaponId, AudioGrp_MeleeAttack op, AkGameObj target,
                                             ref int audioEventId)
        {
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId, FilterLeft1);
            if (evtConfig != null && target)
            {
                Dispatcher.SetSwitch(target, op);
                Dispatcher.PostEvent(evtConfig, target, true);
                audioEventId = evtConfig.Id;
            }
            else
                AudioUtil.Logger.ErrorFormat("Audio Post event failed,target:{0},evtCfg:{1}", target, weaponId);
        }

        internal static void StopReloadAudio(int weaponId, AkGameObj target)
        {
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId, FilterReload);

            if (evtConfig != null && target)
                Dispatcher.StopEvent(evtConfig, target);
            else
                AudioUtil.Logger.ErrorFormat("Audio Post event failed,target:{0},evtCfg:{1}", target, weaponId);
        }

        internal static void PlayFootstepAudio(AudioGrp_Footstep sourceType, AudioGrp_FootMatType matType,
                                               AkGameObj target)
        {
            if (target)
            {
                Dispatcher.SetSwitch(target, matType);
                Dispatcher.SetSwitch(target, sourceType);
                Dispatcher.PostEvent(GetEventConfig(EAudioUniqueId.Footstep), target, true);
            }
        }

        internal static void PlayJumpstepAudio(AudioGrp_FootMatType matType, AkGameObj target)
        {
            if (target)
            {
                Dispatcher.SetSwitch(target, matType);
                Dispatcher.PostEvent(GetEventConfig(EAudioUniqueId.JumpStep), target, true);
            }
        }

        private static AkGameObj GetEmitter(AudioEvent audioEvent)
        {
            var target = LocalObjectGenerator.AudioLocal.GetAudioEmitter();
            target.transform.position    = audioEvent.relatedPos;
            target.transform.eulerAngles = audioEvent.relatedRocation;
            AkSoundEngine.SetRTPCValue(Wwise_IDs.Different_player_effect, 1f, target.gameObject);
            return target;
        }

        private static AkGameObj GetEmitter(Vector3 postion)
        {
            var target = LocalObjectGenerator.AudioLocal.GetAudioEmitter();
            target.transform.position = postion;
            AkSoundEngine.SetRTPCValue(Wwise_IDs.Different_player_effect, 1f, target.gameObject);
            return target;
        }

        public static void PlayTestEvent(string name, Vector3 position)
        {
            var target = LocalObjectGenerator.AudioLocal.GetAudioEmitter();
            target.transform.position = position;
            AkSoundEngine.PostEvent(name, target.gameObject);
        }

        public static void PlayEventAudio(int eventId, Vector3 pos)
        {
            if (SharedConfig.IsServer || AkSoundEngineController.AudioMgrGetter == null)
                return;
            PlayEventAudio(eventId, GetEmitter(pos));
        }

        public static void PlayEventAudio(int eventId, AkGameObj target, bool needCheck = false)
        {
            if (!target && needCheck && SharedConfig.IsServer || AkSoundEngineController.AudioMgrGetter == null)
                return;
            var evtCfg = SingletonManager.Get<AudioEventManager>().FindById(eventId);
            if (evtCfg != null)
            {
                Dispatcher.PostEvent(evtCfg, target, true);
            }
            else
            {
                AudioUtil.Logger.ErrorFormat("Audio Post event failed,target:{0},evtCfg:{1}", target, eventId);
            }
        }

        public static void PlayFlashDizzyAudio(Vector3 Position, float rtpcVal)
        {
            if (SharedConfig.IsServer || AkSoundEngineController.AudioMgrGetter == null)
                return;
            var target = GetEmitter(Position);
            AkSoundEngine.SetRTPCValue(Wwise_IDs.FlashBomb, rtpcVal, target.gameObject);
            PlayEventAudio((int) EAudioUniqueId.FlashDizzy, target);
        }

        public static void OnEventCallback(object cookie, AkCallbackType type, AkCallbackInfo callbackInfo)
        {
            if (type == AkCallbackType.AK_EndOfEvent)
            {
                AkGameObj akGameObj = cookie as AkGameObj;
                if (!akGameObj.IsMainObject)
                    LocalObjectGenerator.AudioLocal.FinishAudio(akGameObj);

                //                var info = callbackInfo as AkEventCallbackInfo;
                //                if (info != null)
                //                {
                ////                    audioSamplesDelegates.Remove(info.playingID);
                ////                    audioFormatDelegates.Remove(info.playingID);
                //                }
            }
        }

        #region sync from Server

        public static void PlayFootstepAudio(AudioFootstepEvent audioEvent)
        {
            if (SharedConfig.IsServer || AkSoundEngineController.AudioMgrGetter == null)
                return;
            var emitter = GetEmitter(audioEvent);
            PlayFootstepAudio(audioEvent.FootstepGrp, audioEvent.FootMatType, emitter);
        }

        public static void PlayJumpstepAudio(AudioJumpstepEvent audioEvent)
        {
            if (SharedConfig.IsServer || AkSoundEngineController.AudioMgrGetter == null)
                return;
            var emitter = GetEmitter(audioEvent);
            PlayJumpstepAudio(audioEvent.FootMatType, emitter);
        }

        public static void PlayHitEnvironmentAudio(AudioGrp_HitMatType hitMatType, int audioId, AkGameObj akGameObj)
        {
            if (SharedConfig.IsServer || AkSoundEngineController.AudioMgrGetter == null)
                return;
            if (akGameObj)
            {
                akGameObj.ThrdPlay(Wwise_IDs.Different_player_effect);
                Dispatcher.SetSwitch(akGameObj, hitMatType);
                PlayEventAudio(audioId, akGameObj);
            }
        }

        public static void PlayHitEnvironmentAudio(AudioGrp_HitMatType hitMatType, int audioId, Vector3 Position)
        {
            if (SharedConfig.IsServer || AkSoundEngineController.AudioMgrGetter == null)
                return;
            var akGameObj = GetEmitter(Position);
            Dispatcher.SetSwitch(akGameObj, hitMatType);
            PlayEventAudio(audioId, akGameObj);
        }

        public static void PlayHitPlayerAudio(EBodyPart hitPartType, int audioId, Vector3 Position)
        {
            if (SharedConfig.IsServer || AkSoundEngineController.AudioMgrGetter == null)
                return;
            AudioGrp_HitMatType hitMatType = hitPartType.ToAudioGrpHitMatType();
            var                 target     = GetEmitter(Position);
            Dispatcher.SetSwitch(target, hitMatType);
            PlayEventAudio(audioId, target);
        }


        public static void PlayBulletDropAudio(int eventId, AudioGrp_FootMatType dropMatType, Vector3 Position)
        {
            if (SharedConfig.IsServer || AkSoundEngineController.AudioMgrGetter == null)
                return;
            var target = GetEmitter(Position);
            Dispatcher.SetSwitch(target, dropMatType);
            PlayEventAudio(eventId, target);
        }

        public static void PlayUniqueEventAudio(AudioDefaultEvent defaultEvent)
        {
            if (!SharedConfig.IsServer && Dispatcher != null)
            {
                var target = GetEmitter(defaultEvent);
                var evtCfg = GetEventConfig(defaultEvent.EventId);

                Dispatcher.PostEvent(evtCfg, target);
            }
        }

        #endregion
    }
}