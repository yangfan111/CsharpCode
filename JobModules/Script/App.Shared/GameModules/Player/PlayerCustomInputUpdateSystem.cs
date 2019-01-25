using App.Shared.Components.Player;
using Core.Configuration;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class PlayerCustomInputUpdateSystem:IUserCmdExecuteSystem
    {
        private PlayerCustomInputUpdateController _controller = new PlayerCustomInputUpdateController();
        
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerCustomInputUpdateSystem));
        
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = (PlayerEntity)owner.OwnerEntity;
            
            if (player.gamePlay.IsLifeState(EPlayerLifeState.Dead))
            {
                return;
            }
            
            if (!player.hasPlayerMove || !player.hasStateInterface || !player.hasOrientation)
            {
                return;
            }

            var state = player.stateInterface.State;
            var orientationPitch = player.orientation.Pitch;
            if (state.GetNextPostureState() == PostureInConfig.Dive)
            {
                float upDownValue = player.playerMove.UpDownValue;

                if (cmd.IsSpaceDown || cmd.IsCDown)
                {
                    upDownValue = _controller.UpdateValue(cmd.FrameInterval * 0.001f, cmd.IsSpaceDown, cmd.IsCDown,
                        player.playerMove.UpDownValue);
                }
                else if ((state.IsForth || state.IsBack)  && !cmd.IsSpaceDown && !cmd.IsCDown)
                {
                    upDownValue = _controller.UpdateToTarget(cmd.FrameInterval * 0.001f,
                        Mathf.Clamp((state.IsForth ? -1.0f : -1.0f)
                                    * orientationPitch /
                                    SingletonManager.Get<CameraConfigManager>().Config
                                        .PoseConfigs[(int) ECameraPoseMode.Stand].PitchLimit.Max *
                                    state.VerticalValue
                            , -1, 1), upDownValue);
                }
                else
                {
                    upDownValue = _controller.UpdateValue(cmd.FrameInterval * 0.001f, cmd.IsSpaceDown, cmd.IsCDown,
                        player.playerMove.UpDownValue);
                }
                state.SetDiveUpDownValue(upDownValue);
                player.playerMove.UpDownValue = upDownValue;
            }
            else
            {
                player.playerMove.UpDownValue = 0;
            }
            
        }
        
        
    }
}