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
using App.Shared.Util;
using Core.ObjectPool;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.Audio
{
    /// <summary>
    /// Defines the <see cref="GameAudioMedia" />
    /// </summary>
    public class GameAudioMedia
    {
        private readonly static AudioObjectGenerator audioObjectGenerator = new AudioObjectGenerator();


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
            audioObjectGenerator.Dispose();
        }

        public static readonly LoggerAdapter AudioLogger = new LoggerAdapter(typeof(AudioEntry));


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
            if (SharedConfig.IsServer || AudioEntry.Dispatcher == null)
                return;
            var target = GetEmitter(fireEvent);
            PlayWeaponFireAudio(fireEvent.WeaponId, target, fireEvent.ShotMode);
        }

        internal static void PlayWeaponReloadAudio(AudioPullboltEvent pullboltEvent)
        {
            if (SharedConfig.IsServer || AudioEntry.Dispatcher == null)
                return;
            var target = GetEmitter(pullboltEvent);
            PlayWeaponReloadAudio(pullboltEvent.WeaponId, pullboltEvent.Magazine, target);
        }


        internal static void PlayWeaponFireAudio(int weaponId, AkGameObj target, AudioGrp_ShotMode shotMode)
        {
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId, FilterFire);
            if (evtConfig != null && target != null)
            {
                AuidoGrp_RefShotMode refShotMode = (shotMode == AudioGrp_ShotMode.Silencer)
                    ? AuidoGrp_RefShotMode.Silencer
                    : AuidoGrp_RefShotMode.Default;
                AudioEntry.Dispatcher.SetSwitch(target, shotMode);
                AudioEntry.Dispatcher.SetSwitch(target, refShotMode);
                AudioEntry.Dispatcher.PostEvent(evtConfig, target);
            }
            else
            {
                AudioEntry.Logger.InfoFormat("Audio Post event failed,target:{0},evtCfg:{1}", target, weaponId);
            }
        }

        //TODO:同步
        internal static void PlayWeaponReloadAudio(int weaponId, AudioGrp_Magazine magazineIndex, AkGameObj target)
        {
            AudioEventItem evtConfig =
                SingletonManager.Get<AudioWeaponManager>().FindById(weaponId, (item) => item.ReloadStart);
            if (evtConfig != null)
            {
                AudioEntry.Dispatcher.SetSwitch(target, magazineIndex);
                AudioEntry.Dispatcher.PostEvent(evtConfig, target);
            }
            else
            {
                AudioEntry.Logger.ErrorFormat("Audio Post event failed,target:{0},evtCfg:{1}", target, weaponId);
            }


            // audioLogger.Info("Auido play once");
        }


        internal static void PlayMeleeAttack(int     weaponId, AudioGrp_MeleeAttack op, AkGameObj target,
                                             ref int audioEventId)
        {
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId, FilterLeft1);
            if (evtConfig != null)
            {
                AudioEntry.Dispatcher.SetSwitch(target, op);
                AudioEntry.Dispatcher.PostEvent(evtConfig, target, true);
                audioEventId = evtConfig.Id;
            }
            else
                AudioEntry.Logger.ErrorFormat("Audio Post event failed,target:{0},evtCfg:{1}", target, weaponId);
        }

        internal static void StopReloadAudio(int weaponId, AkGameObj target)
        {
            AudioEventItem evtConfig = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId, FilterReload);
            if (evtConfig != null)
                AudioEntry.Dispatcher.StopEvent(evtConfig, target);
            else
                AudioEntry.Logger.ErrorFormat("Audio Post event failed,target:{0},evtCfg:{1}", target, weaponId);
        }

        internal static void PlayFootstepAudio(AudioGrp_Footstep sourceType, AudioGrp_FootMatType matType,
                                               AkGameObj        target)
        {
            AudioEntry.Dispatcher.SetSwitch(target, matType);
            AudioEntry.Dispatcher.SetSwitch(target, sourceType);
            AudioEntry.Dispatcher.PostEvent(GetEventConfig(EAudioUniqueId.Footstep), target, true);
        }

        internal static void PlayJumpstepAudio(AudioGrp_FootMatType matType, AkGameObj target)
        {
            AudioEntry.Dispatcher.SetSwitch(target, matType);
            AudioEntry.Dispatcher.PostEvent(GetEventConfig(EAudioUniqueId.JumpStep), target, true);
        }

        #region sync from Server

        public static void PlayFootstepAudio(AudioFootstepEvent audioEvent)
        {
            if (SharedConfig.IsServer || AudioEntry.Dispatcher == null)
                return;
            var emitter = GetEmitter(audioEvent);
            PlayFootstepAudio(audioEvent.FootstepGrp, audioEvent.FootMatType, emitter);
        }

        public static void PlayJumpstepAudio(AudioJumpstepEvent audioEvent)
        {
            if (SharedConfig.IsServer || AudioEntry.Dispatcher == null)
                return;
            var emitter = GetEmitter(audioEvent);
            PlayJumpstepAudio(audioEvent.FootMatType, emitter);
        }

        public static void PlayHitEnvironmentAudio(AudioGrp_HitMatType hitMatType, Vector3 Position)
        {
            if (SharedConfig.IsServer || AudioEntry.Dispatcher == null)
                return;
            var emitter = GetEmitter(Position);
            AudioEntry.Dispatcher.SetSwitch(emitter, hitMatType);
            AudioEntry.Dispatcher.PostEvent(GetEventConfig(EAudioUniqueId.BulletHit), emitter, true);
        }


        public static void PlayBulletDropAudio(int eventId, AudioGrp_FootMatType dropMatType, Vector3 Position)
        {
            if (SharedConfig.IsServer || AudioEntry.Dispatcher == null)
                return;
            var target = GetEmitter(Position);
            AudioEntry.Dispatcher.SetSwitch(target, dropMatType);
            PlayEventAudio(eventId, target);
        }

        public static void PlayUniqueEventAudio(AudioDefaultEvent defaultEvent)
        {
            if (!SharedConfig.IsServer && AudioEntry.Dispatcher != null)
            {
                var target = GetEmitter(defaultEvent);
                var evtCfg = GetEventConfig(defaultEvent.EventId);
                AudioEntry.Dispatcher.PostEvent(evtCfg, target);
            }
        }

        #endregion

        private static AkGameObj GetEmitter(AudioEvent audioEvent)
        {
            var target = audioObjectGenerator.GetAudioEmitter();
            target.transform.position    = audioEvent.relatedPos;
            target.transform.eulerAngles = audioEvent.relatedRocation;
            return target;
        }

        private static AkGameObj GetEmitter(Vector3 postion)
        {
            var target = audioObjectGenerator.GetAudioEmitter();
            target.transform.position = postion;
            return target;
        }

        public static void PlayEventAudio(int eventId, AkGameObj target, bool needCheck = false)
        {
            if (needCheck && SharedConfig.IsServer || AudioEntry.Dispatcher == null)
                return;
            var evtCfg = SingletonManager.Get<AudioEventManager>().FindById(eventId);
            if (evtCfg != null)
            {
                AudioEntry.Dispatcher.PostEvent(evtCfg, target, true);
            }
            else
            {
                AudioEntry.Logger.ErrorFormat("Audio Post event failed,target:{0},evtCfg:{1}", target, eventId);
            }
        }

        public static void PlayEventAudio(int eventId, Vector3 Position)
        {
            if (SharedConfig.IsServer || AudioEntry.Dispatcher == null)
                return;
            var target = GetEmitter(Position);
            PlayEventAudio(eventId, target);
        }

        public static void OnEventCallback(object cookie, AkCallbackType type, AkCallbackInfo callbackInfo)
        {
            if (type == AkCallbackType.AK_EndOfEvent)
            {
                audioObjectGenerator.FinishAudio(cookie as AkGameObj);
//                var info = callbackInfo as AkEventCallbackInfo;
//                if (info != null)
//                {
////                    audioSamplesDelegates.Remove(info.playingID);
////                    audioFormatDelegates.Remove(info.playingID);
//                }
            }
        }
    }
}