using App.Shared.Audio;
using App.Shared.Sound;
using Core.Utils;
using System.IO;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.Configuration;
using App.Shared.GameModules.Player;
using UnityEngine;
using Utils.Singleton;
using Utils.Utils;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared
{
    public static class PlayerEntityAudioExt
    {
      
        public static bool IsStepAudioValied(this PlayerEntity playerEntity)
        {
            return IsStepGameStateAudioValied(playerEntity) && IsStepGlobalStateAudioValied(playerEntity);
        }
        private static bool IsStepGlobalStateAudioValied(this PlayerEntity player)
        {
            return(player.gamePlay.LifeState != (int)EPlayerLifeState.Dead && !player.IsOnVehicle());
        }

        private static bool IsStepGameStateAudioValied(this PlayerEntity player)
        {
            switch(player.gamePlay.GameState)
            {
                case GameState.AirPlane:
                case GameState.Gliding:
                case GameState.Invisible:
                case GameState.JumpPlane:
                    //以上状态不播放行走音效
                    return false;
                case GameState.Normal:
                case GameState.Poison:
                case GameState.Visible:
                    break;
            }

            return true;
        }

        public static AudioGrp_Footstep GetFootStepState(this PlayerEntity player)
        {
            PostureInConfig curPosture = player.stateInterface.State.GetCurrentPostureState();
            var inWater = SingletonManager.Get<MapConfigManager>().InWater(player.position.Value);
            if (inWater)
                return AudioGrp_Footstep.None;
            return AudioUtil.ToAudioFootGrp(curPosture);
         
        }

        public static PlayerAudioController AudioController(this PlayerEntity playerEntity)
        {
            return GameModuleManagement.Get<PlayerAudioController>(playerEntity.entityKey.Value.EntityId).Value; }
        }


}
