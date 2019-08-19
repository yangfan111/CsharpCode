using System;
using System.Collections.Generic;
using App.Shared.Components.FreeMove;
using App.Shared.Components.Player;
using App.Shared.GameModules.Camera.Utils;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityQuaternion;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityTime;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform;
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

        private Vector3 observeOffset = SingletonManager.Get<CameraConfigManager>().Config.GetRoleConfig().ObserveConfig.Offset;
        private float observeDistance =
            SingletonManager.Get<CameraConfigManager>().Config.GetRoleConfig().ObserveConfig.ObserveDistance;

        /// <summary>
        /// 碰撞检测的各个分布点的分布距离
        /// </summary>
        private const float RaycastOffset = 0.1f;

        private readonly List<Vector3> _samplePoints = new List<Vector3>()
        {
            Vector3.zero,
            new Vector3(0, RaycastOffset, 0),
            new Vector3(0, -RaycastOffset, 0),
            new Vector3(RaycastOffset, 0, 0),
            new Vector3(-RaycastOffset, 0, 0)
        };
        
        private FreeMoveContext _freeMoveContext;
        private int _baseCollisionLayers;
        private int _collisionLayers;

        private SpringArm _archoroffsetArm;
        private SpringArm _postOffsetArm;
        private SpringArm _offsetArm;

        private int LastTime;

        public CameraPostUpdateSystem(Contexts context) : base(context)
        {
            _playerContext = context.player;
            _freeMoveContext = context.freeMove;
            _baseCollisionLayers = UnityLayers.CameraCollidableLayerMask;

            _archoroffsetArm = new SpringArm();
            _postOffsetArm = new SpringArm();
            _offsetArm = new SpringArm();

            _archoroffsetArm.Set(0.2f, 0.2f, 5, _baseCollisionLayers);
            _postOffsetArm.Set(0.12f, 0.5f, 5, _baseCollisionLayers);
            _offsetArm.Set(0.1f, 0.02f, 5, _baseCollisionLayers);
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
                UpdateCollisionLayerMask(observedPlayer);
                var punchRotation = new Vector3(2 * observedPlayer.orientation.PunchPitch,
                    observedPlayer.orientation.PunchYaw, observedPlayer.orientation.FireRoll);

                UpdateCollisions(observedPlayer.cameraStateOutputNew, player.cameraFinalOutputNew, punchRotation,
                    observedPlayer, true, cmd.ClientTime);
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

        protected override void ExecWhenNormal(PlayerEntity player, IUserCmd cmd)
        {
            if (!player.hasCameraObj) return;
            if (!player.hasCameraFinalOutputNew) return;
            if (!player.hasCameraStateOutputNew) return;

            UpdateCollisionLayerMask(player);
            var punchRotation = new Vector3(2 * player.orientation.PunchPitch,
                player.orientation.PunchYaw, player.orientation.FireRoll);

            UpdateCollisions(player.cameraStateOutputNew, player.cameraFinalOutputNew, punchRotation,
                player, !player.appearanceInterface.Appearance.IsFirstPerson, cmd.ClientTime);

            SingletonManager.Get<DurationHelp>().Position = player.cameraStateOutputNew.ArchorPosition;
        }

        protected override void FinalExec(PlayerEntity player, IUserCmd cmd)
        {
            UploadComponent(player);
        }
        
        private void UpdateCollisionLayerMask(PlayerEntity playerEntity)
        {
            bool needCollisionWithCar =
                playerEntity.hasStateInterface &&
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
            Vector3 punchRotation, PlayerEntity player, bool doLag, int curTime)
        {
            var realRotation = Quaternion.Euler(calsOut.ArchorEulerAngle + calsOut.EulerAngle + punchRotation);
            var index = (player.gamePlay.JobAttribute == (int) EJobAttribute.EJob_Variant) ? 0.75f : 1f;

            var postOffset = calsOut.ArchorPostOffset + calsOut.PostOffset;
            var realArchorOffset = calsOut.ArchorOffset;
            var realPostOffset = postOffset;
            var archorRotation = Quaternion.Euler(0, calsOut.ArchorEulerAngle.y, 0);

            //计算头顶位置
            _archoroffsetArm.Offset = realArchorOffset / 2 * index;
            var heightTestStart = calsOut.ArchorPosition + archorRotation * realArchorOffset / 2 * index;
            _archoroffsetArm.Update(heightTestStart, archorRotation, curTime - LastTime, doLag, calsOut.NeedDetectDistance);
            realArchorOffset = (_archoroffsetArm.LastLoc - calsOut.ArchorPosition) ;

            //计算锚点位置
            _postOffsetArm.Offset = realPostOffset * index;
            _postOffsetArm.Update(_archoroffsetArm.LastLoc, archorRotation, curTime - LastTime, false, calsOut.NeedDetectDistance);
            realPostOffset = (_postOffsetArm.LastLoc - _archoroffsetArm.LastLoc);
            var postOffsetFactor = Mathf.Max( realPostOffset.magnitude / postOffset.magnitude, 1);
            var startingPosition = calsOut.FinalArchorPosition =
                calsOut.ArchorPosition + postOffsetFactor * realArchorOffset + realPostOffset;
            
            //相机位置计算
            _offsetArm.Offset = calsOut.Offset * index;
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
        
        private void CalcuWhenObserveFreeMove(PlayerEntity playerEntity, FreeMoveEntity observedFreeMove)
        {
            var camera = playerEntity.cameraFinalOutputNew;
            var calsOut = playerEntity.cameraStateOutputNew;

            CalcuFreeMoveEularAngle(observedFreeMove, camera, calsOut);

            CalcuFreeMovePos(observedFreeMove, camera);
        }

        private void CalcuFreeMovePos(FreeMoveEntity observedFreeMove, CameraFinalOutputNewComponent camera)
        {
            if (observedFreeMove==null || !observedFreeMove.hasPosition)
                return;
            
            var archorPos = observedFreeMove.position.Value +
                            (observedFreeMove.hasFreeMoveController ? Vector3.zero : observeOffset);
            var rotation = Quaternion.Euler(camera.EulerAngle);

            if (observedFreeMove.hasFreeMoveController && observedFreeMove.freeMoveController.ControllType ==
                (byte) EFreeMoveControllType.FixFocusPos)
            {
                camera.Position = archorPos;
            }
            else
            {
                float actualDistance = CameraUtility.ScatterCast(archorPos, rotation, observeDistance,
                    _samplePoints, _collisionLayers);
                var finalPostOffset = -rotation.Forward() * actualDistance;
                camera.Position = archorPos + finalPostOffset;
            }

        }

        private static void CalcuFreeMoveEularAngle(FreeMoveEntity observedFreeMove, CameraFinalOutputNewComponent camera,
            CameraStateOutputNewComponent calsOut)
        {
            if (observedFreeMove.hasFreeMoveController && observedFreeMove.freeMoveController.ControllType ==
                (byte) EFreeMoveControllType.FixFocusPos)
            {
                var aimAt = observedFreeMove.freeMoveController.FocusOnPosition.ShiftedVector3();
                var vect = aimAt - observedFreeMove.position.Value;
                camera.EulerAngle = Quaternion.LookRotation(vect).eulerAngles;
            }
            else camera.EulerAngle = calsOut.ArchorEulerAngle + calsOut.EulerAngle;
        }
        
        private void UploadComponent(PlayerEntity player)
        {
            var input = player.cameraFinalOutputNew;
            var output = player.cameraStateUpload;
            
            output.Position = player.GetShiftCameraPos(input.Position);
            output.EulerAngle = input.EulerAngle;
            output.Fov = input.Fov;
            output.Far = input.Far;
            output.Near = input.Near;
        }
    }
}