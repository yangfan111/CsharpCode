using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.Configuration;
using App.Shared.Terrains;
using Core.Utils;
using Utils.Singleton;
using Utils.Utils;
using XmlConfig;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class OnStep : IAnimationEventCallback
    {
        private const int StepInterval = 300;
        private float _lastTime = 0f;
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(OnStep));
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            if(player.time.ClientTime - _lastTime < StepInterval)
            {
                return;
            }
            _lastTime = player.time.ClientTime;
            if(player.gamePlay.LifeState == (int)EPlayerLifeState.Dead)
            {
                //死亡时不播放行走音效
                return;
            }
            if(player.IsOnVehicle())
            {
                //在车上不播放行走音效
                return;
            }
            switch(player.gamePlay.GameState)
            {
                case GameState.AirPlane:
                case GameState.Gliding:
                case GameState.Invisible:
                case GameState.JumpPlane:
                    //以上状态不播放行走音效
                    return;
                case GameState.Normal:
                case GameState.Poison:
                case GameState.Visible:
                    break;
            }
            var myTerrain = SingletonManager.Get<TerrainManager>().GetTerrain(SingletonManager.Get<MapConfigManager>().SceneParameters.Id);
            var curPosture = player.stateInterface.State.GetCurrentPostureState();
            var id = UniversalConsts.InvalidIntId;
            var inWater = SingletonManager.Get<MapConfigManager>().InWater(player.position.Value);
            switch (curPosture)
            {
                case PostureInConfig.Crouch:
                    if(inWater)
                    {
                        player.soundManager.Value.PlayOnce(EPlayerSoundType.SquatSwamp);
                    }
                    else
                    {
                        player.soundManager.Value.PlayOnce(EPlayerSoundType.Squat);
                    }
                    break;
                case PostureInConfig.Prone:
                    if(inWater)
                    {
                        player.soundManager.Value.PlayOnce(EPlayerSoundType.CrawlSwamp);
                    }
                    else
                    {
                        player.soundManager.Value.PlayOnce(EPlayerSoundType.Crawl);
                    }
                    break;
                case PostureInConfig.Stand:
                    if(inWater)
                    {
                        player.soundManager.Value.PlayOnce(EPlayerSoundType.WalkSwamp);
                    }
                    else
                    {
                        player.soundManager.Value.PlayOnce(EPlayerSoundType.Walk);
                    }
                    break;
                case PostureInConfig.Swim:
                    player.soundManager.Value.PlayOnce(EPlayerSoundType.Swim);
                    break;
                case PostureInConfig.Dive:
                    player.soundManager.Value.PlayOnce(EPlayerSoundType.Dive);
                    break;
            }
        }
    }
}
