using App.Shared.Components.Player;
using App.Shared.GameModules.Weapon;
using App.Shared.Player;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Player
{
    class ServerMoveSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ServerMoveSystem));

        private Contexts _contexts;

        public ServerMoveSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = (PlayerEntity) owner.OwnerEntity;
            var moveUpdateData = player.moveUpdate;

            if (!moveUpdateData.NeedUpdate) 
				return;

            var localMoveComponent = player.playerMove;
            var skyMoveUpdateData = player.skyMoveUpdate;

            if (moveUpdateData.MoveType == (int) MoveType.Fly)
            {
                player.playerSkyMove.MoveStage = skyMoveUpdateData.SkyMoveStage;
                player.playerSkyMove.Position = skyMoveUpdateData.SkyPosition;
                player.playerSkyMove.Rotation = skyMoveUpdateData.SkyRotation;
                player.playerSkyMove.LocalPlayerPosition = skyMoveUpdateData.SkyLocalPlayerPosition;
                player.playerSkyMove.LocalPlayerRotation = skyMoveUpdateData.SkyLocalPlayerRotation;
                player.orientation.Pitch = skyMoveUpdateData.Pitch;
                player.orientation.Yaw = skyMoveUpdateData.Yaw;
                player.orientation.Roll = skyMoveUpdateData.Roll;
                player.gamePlay.GameState = skyMoveUpdateData.GameState;
                player.playerSkyMove.IsMoveEnabled = skyMoveUpdateData.IsMoveEnabled;
                //_logger.ErrorFormat("get state:{0}, getMoveEnabled:{1} ", skyMoveUpdateData.GameState,
                //    skyMoveUpdateData.IsMoveEnabled);
            }
            else
            {
                var controller = player.characterContoller.Value;
                var state = player.stateInterface.State;
                var postureInConfig = state.GetNextPostureState();

                controller.SetCurrentControllerType(postureInConfig);
                controller.SetCharacterPosition(player.position.Value);
                controller.SetCharacterRotation(player.orientation.ModelView);
                controller.Rotate(player.orientation.RotationYaw, cmd.FrameInterval * 0.001f);

                if (moveUpdateData.BeginDive) 
					player.stateInterface.State.Dive();
                player.stateInterface.State.SetMoveInWater(moveUpdateData.MoveInWater);
                player.stateInterface.State.SetSteepSlope(moveUpdateData.ExceedSteepLimit);
                player.orientation.ModelPitch = moveUpdateData.ModelPitch;
                player.orientation.ModelYaw = moveUpdateData.ModelYaw;
                localMoveComponent.SpeedRatio = player.moveUpdate.SpeedRatio;
                localMoveComponent.MoveSpeedRatio = player.moveUpdate.MoveSpeedRatio;
            }

            localMoveComponent.Velocity = moveUpdateData.Velocity;
            localMoveComponent.IsGround = moveUpdateData.IsGround;
            localMoveComponent.IsCollided = moveUpdateData.IsCollided;

            UpdatePosition(player, moveUpdateData);

            // _logger.ErrorFormat(" IsC ollided:{1},IsGround:{2}", moveUpdateData.IsCollided,moveUpdateData.IsGround);
        }

        private void UpdatePosition(PlayerEntity player, MoveUpdateComponent moveUpdateData)
        {
            player.position.Value = moveUpdateData.LastPosition;
            player.position.Value += moveUpdateData.Dist;
            var obj = player.RootGo().transform;
            obj.position = player.position.Value;
            obj.eulerAngles = moveUpdateData.Rotation;
        }
    }
}