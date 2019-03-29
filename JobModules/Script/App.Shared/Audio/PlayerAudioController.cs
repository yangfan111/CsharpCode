using App.Shared.GameModules;
using App.Shared.Player.Events;
using Core.EntityComponent;
using Core.Event;
using Core.Utils;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.Audio
{
    public class PlayerAudioController : ModuleLogicActivator<PlayerAudioController>
    {
        public EntityKey Owner { get; private set; }
        private AudioPlayerComponentsAgent audioAgent;

        public void Initialize(EntityKey owner, AudioPlayerComponentsAgent agent)
        {
            Owner = owner;
            audioAgent = agent;
        }


        public void PlayStepEnvironmentAudio(AudioGrp_Footstep stepState)

        {
            if (stepState == AudioGrp_Footstep.None) return;
            var playInterval = AudioUtil.GetFootstepPlayInterval(stepState);
            if (audioAgent.RelatedTime - audioAgent.RelatedAudio.LastFootPrintPlayStamp > playInterval)
            {
                audioAgent.RefreshStepPlayTimestamp();
                AudioEvent audioEvent = (AudioEvent) EventInfos.Instance.Allocate(EEventType.BroadcastAudio, false);
                audioEvent.footstepState = stepState;
                audioEvent.relatedPos = audioAgent.RelatedPlayerPos;
                audioAgent.RelatedLocalEvents.AddEvent(audioEvent);
                //GameAudioMedia.PlayEnvironmentAudio(stepState, audioAgent.RelatedPlayerPos, StepObject);

            }
        }
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerAudioController));

        public void PlayStepEnvironmentAudio(AudioEvent e)
        {
            _logger.Info("Wise Play step Enviornment audio");
       //     GameAudioMedia.PlayStepEnvironmentAudio(e.footstepState, e.relatedPos, PlayerObject);
        }

        private GameObject WeaponObject
        {
            get { return audioAgent.RelatedAappearence.WeaponHandObject(); }
        }

        public GameObject PlayerObject
        {
            get { return audioAgent.RelatedAappearence.CharacterP1; }
        }

        public void PlaySwitchAuido(int weaponId, InOrOff op)
        {
            if (op == InOrOff.In)
            {
                GameAudioMedia.PlayWeaponSimpleAudio(weaponId, WeaponObject,
                    (config) => config.SwitchIn);
            }
            else
            {
                GameAudioMedia.PlayWeaponSimpleAudio(weaponId,PlayerObject,
                    (config) => config.SwitchOff);
            }
        }

        public void PlayFireAudio(int weaponId, AudioGrp_ShotMode shotMode,Vector3 position)
        {
            var fireEvent = EventInfos.Instance.Allocate(EEventType.Fire, false) as FireEvent;
            fireEvent.fireWeaponId = weaponId;
            fireEvent.audioFireMode = (int)shotMode;
            fireEvent.audioFirePos = position;
            audioAgent.RelatedLocalEvents.AddEvent(fireEvent);
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
                WeaponObject);
        }

        public void PlayReloadBulletAudio(int configId)
        {
//            if (audioAgent.RelatedAudio.ReloadedBulletLeft < 1)
//                return;
                //audioAgent.RelatedAudio.ReloadedBulletLeft -= 1;
                GameAudioMedia.PlayWeaponReloadAudio(configId, AudioGrp_Magazine.FillBulletOnly,
                    WeaponObject);
        
        }
        public void SetReloadBulletAudioCount(int reloadedBulletCount)
        {
         //   audioAgent.RelatedAudio.ReloadedBulletLeft = reloadedBulletCount ;
          //  PlayReloadBulletAudio(configId);
        }
        public void PlayReloadAudio(int configId,bool emptyReload)
        {
          
           if(emptyReload)
               GameAudioMedia.PlayWeaponReloadAudio(configId, AudioGrp_Magazine.MagizineAndPull,
                   WeaponObject);
           else 
               GameAudioMedia.PlayWeaponReloadAudio(configId, AudioGrp_Magazine.MagizineOnly,
                    WeaponObject);
           //if(reloadCount>0)
//                GameAudioMedia.PlayWeaponReloadAudio(configId, AudioGrp_Magazine.MagizineAndPull,
//                WeaponObject);
//            else
//                GameAudioMedia.PlayWeaponReloadAudio(configId, AudioGrp_Magazine.MagizineOnly,
//                    WeaponObject);

        }

    }

}
