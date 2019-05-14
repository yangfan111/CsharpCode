using System.Collections.Generic;
using App.Shared.Components.Player;
using App.Shared.GameModules.Camera.Utils;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityTime;
using Core.CameraControl;
using Core.Compare;
using Core.Components;
using Core.Configuration;
using Core.EntityComponent;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Camera
{
    public class CameraPostUpdateSystem : AbstractCameraUpdateSystem, IUserCmdExecuteSystem, IRenderSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CameraPostUpdateSystem));
        private PlayerContext _playerContext;

        public const float Epsilon = 0.0001f;
        public readonly float CollisionRecoverySpeed = 5f;
        public readonly float MinCollisionDistance = 0f;

        private Vector3 observeOffset = SingletonManager.Get<CameraConfigManager>().Config.ObserveConfig.Offset;
        private float observeDistance =
            SingletonManager.Get<CameraConfigManager>().Config.ObserveConfig.ObserveDistance;
        private float MaxTestHeight = SingletonManager.Get<CameraConfigManager>().Config.SpecialZoneConfig.FloorTestHeight;
        private float OffsetInBuilding = SingletonManager.Get<CameraConfigManager>().Config.SpecialZoneConfig.OffsetLengthInBuilding;
        private float FocusPositionLerpTime =
            SingletonManager.Get<CameraConfigManager>().Config.SpecialZoneConfig.FocusPositionLerpTime;

        /// <summary>
        /// 相机离开碰撞点的距离
        /// </summary>
        private const float DistanceOffset = 0.1f;

        /// <summary>
        /// 碰撞检测的各个分布点的分布距离
        /// </summary>
        private const float RaycastOffset = 0.05f;

        /// <summary>
        /// 碰撞检测的方向是发散的，这个参数控制发散的程度            
        /// </summary>
        private const float RaycastDirFactor = 0.5f;

        private readonly List<Vector3> _samplePoints = new List<Vector3>()
        {
            Vector3.zero,
            new Vector3(0, RaycastOffset, 0),
            new Vector3(0, -RaycastOffset, 0),
            new Vector3(RaycastOffset, 0, 0),
            new Vector3(-RaycastOffset, 0, 0)
        };

        private const float _collisionOffsetStartDistance = 1.8f;
        private VehicleContext _vehicleContext;
        private FreeMoveContext _freeMoveContext;
        private int _baseCollisionLayers;
        private int _collisionLayers;

        private SpringArm _archoroffsetArm;
        private SpringArm _postOffsetArm;
        private SpringArm _offsetArm;

        private int LastTime;
        private const float HeightTestRadius = 0.20f;

        public CameraPostUpdateSystem(Contexts context) : base(context)
        {
            _playerContext = context.player;
            _vehicleContext = context.vehicle;
            _freeMoveContext = context.freeMove;
            _baseCollisionLayers = UnityLayers.CameraCollidableLayerMask;
            
            _archoroffsetArm = new SpringArm();
            _postOffsetArm = new SpringArm();
            _offsetArm = new SpringArm();

            _archoroffsetArm.Set(HeightTestRadius, 0.2f, 5, 0.1f, _baseCollisionLayers);
            _postOffsetArm.Set(0.06f, 0.5f, 5, 0.05f, _baseCollisionLayers);
            _offsetArm.Set(0.04f, 0.02f, 5, 0.02f, _baseCollisionLayers);
        }

        public void OnRender()
        {
            var playerEntity = _playerContext.flagSelfEntity;
            
            if (playerEntity.hasCameraArchor && playerEntity.cameraArchor.Active)
            {
                CommonUpdate(playerEntity, playerEntity.userCmd.Latest);
            }
            else
            {
                UnityEngine.Camera.main.transform.position =
                    playerEntity.cameraFinalOutputNew.Position = playerEntity.position.Value;
            }
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var player = owner.OwnerEntity as PlayerEntity;
            if (player == null) return;
            CommonUpdate(player, cmd);
            LastTime = cmd.ClientTime;
        }
        
        protected override void ExecWhenObserving(PlayerEntity player, IUserCmd cmd)
        {   
            int observedObjId = player.gamePlay.CameraEntityId;
            
            var observedPlayer = _playerContext.GetEntityWithEntityKey(new EntityKey(observedObjId, (short)EEntityType.Player));
            if (observedPlayer != null)
            {
                CalcuWhenObservePlayer(player, observedPlayer);
            }
            else
            {
                var observedFreeMove =
                    _freeMoveContext.GetEntityWithEntityKey(new EntityKey(observedObjId, (short) EEntityType.FreeMove));
                if (observedFreeMove != null)
                {
                    CalcuWhenObserveFreeMove(player, observedFreeMove);
                }
            }
        }

        protected override void ExecWhenBeingObserved(PlayerEntity player, IUserCmd cmd)
        {
            if (player.appearanceInterface.Appearance.IsFirstPerson)
            {
                var punchRotation = new Vector3(2 * player.orientation.PunchPitch,
                    2 * player.orientation.PunchYaw, player.orientation.FireRoll);
                UpdateCollisions(player.thirdPersonDataForObserving.ThirdPersonData,
                    player.thirdPersonDataForObserving.ThirdPersonOutput, punchRotation, player, cmd.ClientTime);
            }
        }

        protected override void ExecWhenNormal(PlayerEntity player, IUserCmd cmd)
        {
            if (!player.hasCameraObj) return;
            if (!player.hasCameraFinalOutputNew) return;
            if (!player.hasCameraStateOutputNew) return;
            
            UpdateCollisionLayerMask(player);
            var punchRotation = new Vector3(2 * player.orientation.PunchPitch,
                2 * player.orientation.PunchYaw, player.orientation.FireRoll);

            UpdateCollisions(player.cameraStateOutputNew, player.cameraFinalOutputNew, punchRotation,
                player, cmd.ClientTime);
            
            SingletonManager.Get<DurationHelp>().Position = player.cameraStateOutputNew.ArchorPosition;
        }

        protected override void FinalExec(PlayerEntity player, IUserCmd cmd)
        {
            UploadComponent(player);
        }
        
        private void UpdateCollisionLayerMask(PlayerEntity playerEntity)
        {
            bool needCollisionWithCar =
                playerEntity.stateInterface.State.GetActionKeepState() != ActionKeepInConfig.Drive &&
                playerEntity.gamePlay.GameState != Components.GameState.AirPlane;
            
            _collisionLayers = needCollisionWithCar
                ? _baseCollisionLayers
                : _baseCollisionLayers & ~UnityLayers.VehicleLayerMask;

            _offsetArm.CollisionLayers = _collisionLayers;
            
            _archoroffsetArm.CollisionLayers =
                _postOffsetArm.CollisionLayers = _collisionLayers & ~UnityLayers.GlassLayerMask;
        }
        
        private void UpdateCollisions(CameraStateOutputNewComponent calsOut, CameraFinalOutputNewComponent camera,
            Vector3 punchRotation, PlayerEntity player, int curTime)
        {
            var doLag = !player.appearanceInterface.Appearance.IsFirstPerson;
            var realRotation = Quaternion.Euler(calsOut.ArchorEulerAngle + calsOut.EulerAngle + punchRotation);

            var postOffset = calsOut.ArchorPostOffset + calsOut.PostOffset;
            var realArchorOffset = calsOut.ArchorOffset;
            var realPostOffset = postOffset;
            var archorRotation = Quaternion.Euler(0, calsOut.ArchorEulerAngle.y, 0);
            
            //计算头顶位置
            _archoroffsetArm.Offset = realArchorOffset / 2;
            var heightTestStart = calsOut.ArchorPosition + archorRotation * realArchorOffset / 2;
            _archoroffsetArm.Update(heightTestStart, archorRotation, curTime - LastTime, doLag, calsOut.NeedDetectDistance);
            realArchorOffset = _archoroffsetArm.LastLoc - calsOut.ArchorPosition;
            
            //计算锚点位置
            _postOffsetArm.Offset = realPostOffset;
            _postOffsetArm.Update(_archoroffsetArm.LastLoc, archorRotation, curTime - LastTime, false, calsOut.NeedDetectDistance);
            realPostOffset = _postOffsetArm.LastLoc - _archoroffsetArm.LastLoc;
            var postOffsetFactor = Mathf.Max( realPostOffset.magnitude / postOffset.magnitude, 1);
            var startingPosition = calsOut.FinalArchorPosition =
                calsOut.ArchorPosition + postOffsetFactor * realArchorOffset + realPostOffset;

            //封闭建筑内拉近摄像机距离
//            if(BuildingRestrictTest(calsOut, player))
//                calsOut.Offset = calsOut.Offset.normalized * OffsetInBuilding;
            
            //相机位置计算
            _offsetArm.Offset = calsOut.Offset;
            _offsetArm.Update(startingPosition, realRotation, curTime - LastTime, doLag, true);  

            camera.PlayerFocusPosition = startingPosition;
            camera.Position = _offsetArm.LastLoc;   
            camera.EulerAngle = calsOut.ArchorEulerAngle + calsOut.EulerAngle + punchRotation;
            camera.EulerAngle.x = YawPitchUtility.Normalize(camera.EulerAngle.x);
            camera.EulerAngle.y = YawPitchUtility.Normalize(camera.EulerAngle.y);
            camera.EulerAngle.z = YawPitchUtility.Normalize(camera.EulerAngle.z);
            camera.Fov = calsOut.Fov;
            camera.Far = calsOut.Far;
            camera.Near = calsOut.Near;

#if UNITY_EDITOR
            var p1 = calsOut.ArchorPosition;
            var p2 = _archoroffsetArm.LastLoc;
            var p3 = startingPosition;
            var p4 = _offsetArm.LastLoc;

            Debug.DrawLine(p1, p2, Color.red);
            Debug.DrawLine(p2, p3, Color.green);
            Debug.DrawLine(p3, p4, Color.blue);
#endif
        }

        private bool BuildingRestrictTest(CameraStateOutputNewComponent calsOut, PlayerEntity player)
        {
            if (player.stateInterface.State.GetActionState() != ActionInConfig.Gliding &&
                player.stateInterface.State.GetActionState() != ActionInConfig.Parachuting &&
                player.gamePlay.GameState != Components.GameState.AirPlane)
            {
                var realHeight = HeightTest(player, MaxTestHeight);
                if (realHeight >= 0)
                {
                    if (realHeight < MaxTestHeight)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        private const float TinyDistanceOffset = 0.01f;
        //胶囊探测，防止摄像机与天花板穿模
        public float HeightTest(PlayerEntity player, float maxHeight)
        {
            var calsOut = player.cameraStateOutputNew;
            float radius = player.characterContoller.Value.radius;
            var center = calsOut.ArchorPosition + calsOut.ArchorOffset.normalized * (player.characterContoller.Value.height - radius);
            var direction = calsOut.ArchorOffset.normalized;
            RaycastHit hitInfo;
            if (Physics.CapsuleCast(center, center, radius, direction, out hitInfo, maxHeight,UnityLayers.SceneCollidableLayerMask))
            {
                if (hitInfo.distance > 0)
                {
                    var result = hitInfo.distance - TinyDistanceOffset + player.characterContoller.Value.height -
                                     radius;
                    return result;
                }
            }
            return -1;
        }
        
        private void CalcuWhenObserveFreeMove(PlayerEntity playerEntity, FreeMoveEntity observedFreeMove)
        {
            var camera = playerEntity.cameraFinalOutputNew;
            var calsOut = playerEntity.cameraStateOutputNew;

            camera.EulerAngle = calsOut.ArchorEulerAngle + calsOut.EulerAngle;

            var rotation = Quaternion.Euler(camera.EulerAngle);
            var archorPos = observedFreeMove.position.Value + observeOffset;
            float actualDistance = CameraUtility.ScatterCast(archorPos, rotation, observeDistance,
                                       _samplePoints, _collisionLayers);
            
            var finalPostOffset = -rotation.Forward() * actualDistance;
            camera.Position = archorPos + finalPostOffset;
        }

        private void CalcuWhenObservePlayer(PlayerEntity playerEntity, PlayerEntity observedPlayer)
        {
            if (!observedPlayer.hasObserveCamera)
                return ;

            var camera = playerEntity.cameraFinalOutputNew;
            var playerData = observedPlayer.observeCamera;

            camera.Position = playerData.CameraPosition.ShiftedVector3();
            camera.EulerAngle = playerData.CameraEularAngle;
        }
        
        private void UploadComponent(PlayerEntity player)
        {
            var input = player.cameraFinalOutputNew;
            var output = player.cameraStateUpload;
            
            output.Position = input.Position.ShiftedToFixedVector3();
            output.EulerAngle = input.EulerAngle;
            output.Fov = input.Fov;
            output.Far = input.Far;
            output.Near = input.Near;
            output.PlayerFocusPosition = input.PlayerFocusPosition.ShiftedToFixedVector3();
            
            output.ThirdPersonCameraPostion =
                player.appearanceInterface.Appearance.IsFirstPerson
                    ? player.thirdPersonDataForObserving.ThirdPersonOutput.Position
                    : player.cameraFinalOutputNew.Position;
        }
    }
}