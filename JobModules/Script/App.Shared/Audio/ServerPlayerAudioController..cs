using App.Shared.GameModules.Player.CharacterBone;
using App.Shared.GameModules.Weapon;
using App.Shared.Player.Events;
using Core;
using Core.Event;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.Audio
{
    public class ServerPlayerAudioController : PlayerAudioControllerBase
    {
        public override void Update(IUserCmd cmd)
        {
            if (entity.playerClientUpdate.HasFootstepFrame)
            {
                PlayFootstepAudioS((AudioGrp_Footstep) entity.playerClientUpdate.FootstepFrameGroup);
            }
        }

        public override void PlayJumpStepAudio()
        {
            AudioJumpstepEvent audioEvent =
                (AudioJumpstepEvent) EventInfos.Instance.Allocate(EEventType.AJumpstep, false);
            audioEvent.Initialize(GetFootMatType(), new Vector3(PlayerObject.transform.position.x,
                0, PlayerObject.transform.position.z), PlayerObject.transform.eulerAngles);
            entity.localEvents.Events.AddEvent(audioEvent);
        }

        public override void PlayFootstepAudioS(AudioGrp_Footstep stepState)
        {
            AudioFootstepEvent audioEvent =
                (AudioFootstepEvent) EventInfos.Instance.Allocate(EEventType.AFootstep, false);
            audioEvent.Initialize(stepState, GetFootMatType(),
                new Vector3(PlayerObject.transform.position.x, 0, PlayerObject.transform.position.z),
                PlayerObject.transform.eulerAngles);
            entity.localEvents.Events.AddEvent(audioEvent);
        }

        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(ServerPlayerAudioController));

        private GameObject PlayerObject
        {
            get { return entity.appearanceInterface.Appearance.CharacterP1; }
        }

        private GameObject DefaultWeaponObject
        {
            get { return entity.appearanceInterface.Appearance.GetWeaponP1InHand() ?? PlayerObject; }
        }

        private Transform FireMuzzleObject
        {
            get
            {
                var muzzleTrans = CharacterBoneSynchronizer.GetMuzzleP3Pos(entity);
                if (muzzleTrans)
                    return muzzleTrans;
                
                return DefaultWeaponObject.transform;
            }
        }


        public override void PlaySimpleAudio(EAudioUniqueId uniqueId, bool sync = false)
        {
            if (sync)
            {
                AudioDefaultEvent defaultEvent =
                    EventInfos.Instance.Allocate(EEventType.ADefault, false) as AudioDefaultEvent;
                defaultEvent.Initialize(uniqueId, FireMuzzleObject.position, FireMuzzleObject.eulerAngles);
                entity.localEvents.Events.AddEvent(defaultEvent);
            }
        }

        public override void PlayFireAudio(int weaponId, AudioGrp_ShotMode shotMode)
        {
            AudioWeaponFireEvent fireEvent =
                EventInfos.Instance.Allocate(EEventType.AWeaponFire, false) as AudioWeaponFireEvent;

            fireEvent.Initialize(shotMode, weaponId, FireMuzzleObject.position,
                FireMuzzleObject.eulerAngles);
            entity.localEvents.Events.AddEvent(fireEvent);
        }

        public override void PlayMeleeAttackAudio(int weaponId, int attackType)
        {
            int audioEventId = SingletonManager.Get<AudioWeaponManager>().FindById(weaponId).Left1;
            if (audioEventId > 0)
            {
                AudioMeleeAtkEvent audioEvent =
                    EventInfos.Instance.Allocate(EEventType.AMeleeAttack, false) as AudioMeleeAtkEvent;
                audioEvent.Initialize(audioEventId, attackType, FireMuzzleObject.position,
                    FireMuzzleObject.eulerAngles);
                entity.localEvents.Events.AddEvent(audioEvent);
            }
        }


        public override void PlayReloadAuido(int weaponId, AudioGrp_Magazine magizine, float magizineSpeed)
        {
            AudioPullboltEvent pullboltEvent =
                EventInfos.Instance.Allocate(EEventType.APullbolt, false) as AudioPullboltEvent;
            pullboltEvent.Initialize(magizine, weaponId, FireMuzzleObject.position,
                FireMuzzleObject.eulerAngles);
            entity.localEvents.Events.AddEvent(pullboltEvent);
        }
    }
}