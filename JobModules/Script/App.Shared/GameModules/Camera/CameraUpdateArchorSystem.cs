using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.GameModules.Camera.Utils;
using App.Shared.GameModules.Player;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.CharacterState;

namespace App.Shared.GameModules.Camera
{
    public class CameraUpdateArchorSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CameraUtility));

        private FreeMoveContext _freeMoveContext;
        private int _cmdSeq = 0;
        private float _transitionTime = 300;
        private PlayerContext _playerContext;

        public CameraUpdateArchorSystem(Contexts _contexts)
        {
            _playerContext = _contexts.player;
            _freeMoveContext = _contexts.freeMove;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            _cmdSeq = cmd.Seq;
            PlayerEntity player = owner.OwnerEntity as PlayerEntity;

            if (!player.hasCameraStateNew) return;
            if (!player.hasCameraStateOutputNew) return;

            InternalExecute(player);
            UpdareArchTransition(player);
        }

        private void InternalExecute(PlayerEntity player)
        {
            player.cameraArchor.ArchorType = GetAnchorType(player);
            player.cameraArchor.ArchorPosition =
                GetAnchorPosition(player, player.cameraArchor.ArchorType);
            player.cameraArchor.ArchorEulerAngle =
                GetAnchorEulerAngle(player, player.cameraArchor.ArchorType);

            if (player.appearanceInterface.Appearance.IsFirstPerson && player.hasObserveCamera)
            {
                player.thirdPersonDataForObserving.ThirdPersonArchorPosition =  player.position.Value;
            }
        }

        private void UpdareArchTransition(PlayerEntity player)
        {
            if (player.cameraArchor.ArchorType != player.cameraArchor.LastArchorType && (
                    player.cameraArchor.ArchorType.Equals(ECameraArchorType.Parachuting)
                    || player.cameraArchor.LastArchorType.Equals(ECameraArchorType.Parachuting)
                ))
            {
                player.cameraArchor.EnterTime = player.time.ClientTime;
                player.cameraArchor.ArchorTransitionPosition =
                    GetAnchorPosition(player, player.cameraArchor.LastArchorType) -
                    GetAnchorPosition(player, player.cameraArchor.ArchorType);
            }

            if (player.cameraArchor.EnterTime + _transitionTime > player.time.ClientTime)
            {
                player.cameraArchor.ArchorTransitionOffsetPosition =
                    (1f - (player.time.ClientTime - player.cameraArchor.EnterTime) / _transitionTime) *
                    player.cameraArchor.ArchorTransitionPosition;
            }
            else
            {
                player.cameraArchor.ArchorTransitionOffsetPosition = Vector3.zero;
            }

            player.cameraArchor.LastArchorType = player.cameraArchor.ArchorType;
        }

        public ECameraArchorType GetAnchorType(PlayerEntity player)
        {
            if (player.IsOnVehicle())
            {
                return ECameraArchorType.Car;
            }
            else if (player.hasPlayerSkyMove && player.playerSkyMove.IsParachuteAttached)
            {
                return ECameraArchorType.Parachuting;
            }
            else if (player.gamePlay.GameState == GameState.AirPlane &&
                     null != player.GetAirPlane(_freeMoveContext) && player.GetAirPlane(_freeMoveContext).hasPosition)
            {
                return ECameraArchorType.AirPlane;
            }
            else if(player.gamePlay.IsObserving())
            {
                var follow = _playerContext.GetEntityWithEntityKey(
                    new Core.EntityComponent.EntityKey(player.gamePlay.CameraEntityId, (short) EEntityType.Player));
                if(follow!=null && follow.hasPosition)
                return ECameraArchorType.FollowEntity;
            }
            return ECameraArchorType.Third;
        }

        public Vector3 GetAnchorEulerAngle(PlayerEntity player, ECameraArchorType type)
        {
            switch (type)
            {
                case ECameraArchorType.Car:
                    var r = player.controlledVehicle.CameraCurrentRotation.eulerAngles;
                    return new Vector3(0, r.y, 0);

                case ECameraArchorType.AirPlane:
                    return Vector3.zero;

                case ECameraArchorType.FollowEntity:
                    PlayerEntity follow = _playerContext.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(player.gamePlay.CameraEntityId, (short)EEntityType.Player));
                    if(follow != null)
                    {
                        return follow.orientation.EulerAngle;
                    }
                    else
                    {
                        return player.orientation.EulerAngle;
                    }

                case ECameraArchorType.Parachuting:
                    return new Vector3(0, player.playerSkyMove.Rotation.eulerAngles.y, 0);

                default:
                    return player.orientation.EulerAngle;
            }
        }

        public Vector3 GetAnchorPosition(PlayerEntity player, ECameraArchorType type)
        {
            player.cameraArchor.Active = true;
            switch (type)
            {
                case ECameraArchorType.Car:
                    var t = player.controlledVehicle.CameraCurrentPosition;
                    return new Vector3(t.x, t.y, t.z);

                case ECameraArchorType.AirPlane:
                    return player.GetAirPlane(_freeMoveContext).position.Value;

                case ECameraArchorType.FollowEntity:
                    PlayerEntity follow = _playerContext.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(player.gamePlay.CameraEntityId, (short)EEntityType.Player));
                    if (follow != null)
                    {
                        return follow.position.Value;
                    }
                    else
                    {
                        return player.position.Value;
                    }

                case ECameraArchorType.Parachuting:

                    return player.playerSkyMove.Position;
                default:
                    if (player.hasAppearanceInterface && player.appearanceInterface.Appearance.IsFirstPerson)
                    {
                        var camRoot = player.characterBoneInterface.CharacterBone.GetLocation(SpecialLocation
                            .FirstPersonCamera, CharacterView.FirstPerson);
                        if (null != camRoot)
                        {
                            return camRoot.position;
                        }
                        else
                        {
                            Logger.Error("First camera root can't be found");
                        }
                    }

                    return player.position.Value;
            }
        }

       
    }
}