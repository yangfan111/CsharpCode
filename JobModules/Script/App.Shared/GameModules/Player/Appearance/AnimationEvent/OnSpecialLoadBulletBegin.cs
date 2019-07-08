
using Core.Utils;


namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class OnSpecialLoadBulletBegin : IAnimationEventCallback
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(OnSpecialLoadBulletBegin));
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
                player.AudioController().PlayReloadBulletAudio(player.WeaponController().HeldConfigId);
      //      DebugUtil.MyLog("Step In");
        //    Logger.Info("Step in");
//            if (!player.IsStepAudioValied())
//                return;
//            var stepState = player.GetFootStepState();
//            if (stepState == AudioGrp_Footstep.None)
//                return;
//            player.AudioController().PlayEnvironmentAudio(stepState);
//            if (!AudioUtil.IsStepAudioValied(player))
//            {
//                return;
//            }
//            switch(player.gamePlay.GameState)
//            {
//                case GameState.AirPlane:
//                case GameState.Gliding:
//                case GameState.Invisible:
//                case GameState.JumpPlane:
//                    //以上状态不播放行走音效
//                    return;
//                case GameState.Normal:
//                case GameState.Poison:
//                case GameState.Visible:
//                    break;
//            }
           
//   //         var myTerrain = SingletonManager.Get<TerrainManager>().GetTerrain(SingletonManager.Get<MapConfigManager>().SceneParameters.Id);
//            PostureInConfig curPosture = player.stateInterface.State.GetCurrentPostureState();
//            var id = UniversalConsts.InvalidIntId;
//            var inWater = false;// SingletonManager.Get<MapConfigManager>().InWater(player.position.Value);
//            AudioGrp_Footstep step = AudioGrp_Footstep.None;
//            switch (curPosture)
//            {
//                case PostureInConfig.Crouch:
//                    if (!inWater)
//                        step = AudioGrp_Footstep.Squat;
//                    break;
//                case PostureInConfig.Prone:
//                    if (!inWater)
//                        step = AudioGrp_Footstep.Crawl;
//                    break;
//                case PostureInConfig.Stand:
//                    if (!inWater)
//                        step = AudioGrp_Footstep.Walk;
//                    
//                    break;
//                case PostureInConfig.Swim:
//                    player.soundManager.Value.PlayOnce(EPlayerSoundType.Swim);
//                    break;
//                case PostureInConfig.Dive:
//                    player.soundManager.Value.PlayOnce(EPlayerSoundType.Dive);
//                    break;
//            }
         
          
//                GameAudioMedia.PlayEnvironmentAudio(stepState, player.position.Value, player.appearanceInterface.Appearance.CharacterP1);
//                player.playerAudio.LastFootPrintPlayStamp = player.time.ClientTime;
            
            //switch (curPosture)
            //{
            //    case PostureInConfig.Crouch:
            //        if(inWater)
            //        {
            //            player.soundManager.Value.PlayOnce(EPlayerSoundType.SquatSwamp);
            //        }
            //        else
            //        {
            //            player.soundManager.Value.PlayOnce(EPlayerSoundType.Squat);
            //        }
            //        break;
            //    case PostureInConfig.Prone:
            //        if(inWater)
            //        {
            //            player.soundManager.Value.PlayOnce(EPlayerSoundType.CrawlSwamp);
            //        }
            //        else
            //        {
            //            player.soundManager.Value.PlayOnce(EPlayerSoundType.Crawl);
            //        }
            //        break;
            //    case PostureInConfig.Stand:
            //        if(inWater)
            //        {
            //            player.soundManager.Value.PlayOnce(EPlayerSoundType.WalkSwamp);
            //        }
            //        else
            //        {
            //            player.soundManager.Value.PlayOnce(EPlayerSoundType.Walk);
            //        }
            //        break;
            //    case PostureInConfig.Swim:
            //        player.soundManager.Value.PlayOnce(EPlayerSoundType.Swim);
            //        break;
            //    case PostureInConfig.Dive:
            //        player.soundManager.Value.PlayOnce(EPlayerSoundType.Dive);
            //        break;
            //}
        }
    }
}
