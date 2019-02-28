using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.GameModules.Camera.Utils;
using App.Shared.Player;
using Core.CameraControl;
using Core.Configuration;
using Core.EntityComponent;
using Core.GameModule.Interface;
using Core.GameTime;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using UnityEngine.AI;
using Utils.Compare;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Camera
{
    public class CameraPostUpdateSystem : IUserCmdExecuteSystem,IRenderSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(CameraPostUpdateSystem));
        private PlayerContext _playerContext;

        public const float Epsilon = 0.0001f;
        public readonly float CollisionRecoverySpeed = 5f;
        public readonly float MinCollisionDistance = 0f;

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

        private float _collisionOffsetStartDistance = 1.8f;
        private VehicleContext _vehicleContext;
        private FreeMoveContext _freeMoveContext;
        private int _baseCollisionLayers;
        private int _collisionLayers;
        
        public CameraPostUpdateSystem(PlayerContext playContext, VehicleContext vehicleContext, FreeMoveContext freeMoveContext)
        {
            _playerContext = playContext;
            _vehicleContext = vehicleContext;
            _freeMoveContext = freeMoveContext;
            _baseCollisionLayers = UnityLayers.SceneCollidableLayerMask;
        }
        
        public void OnRender()
        {
            var playerEntity = _playerContext.flagSelfEntity;
            
            if (playerEntity.hasCameraArchor && playerEntity.cameraArchor.Active)
            {
                InternalExecute(playerEntity);
            }
            else
            {
                UnityEngine.Camera.main.transform.position =
                    playerEntity.cameraFinalOutputNew.Position = playerEntity.position.Value;
            }
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var playerEntity = owner.OwnerEntity as PlayerEntity;
            InternalExecute(playerEntity);
            CopyFinalOutputToUploadComponent(playerEntity.cameraFinalOutputNew, playerEntity.cameraStateUpload);
        }

        private void InternalExecute(PlayerEntity playerEntity)
        {
            if (playerEntity.gamePlay.IsObserving())
                CalcuWhenObserving(playerEntity);
            else
                CalcuForNormal(playerEntity);
        }
        
        private void CopyFinalOutputToUploadComponent(CameraFinalOutputNewComponent input,
            CameraStateUploadComponent output)
        {
            output.Position = input.Position;
            output.EulerAngle = input.EulerAngle;
            output.Fov = input.Fov;
            output.Far = input.Far;
            output.Near = input.Near;
            output.PlayerFocusPosition = input.PlayerFocusPosition;
        }

        private void UpdateCollisionLayerMask(PlayerEntity playerEntity)
        {
            bool needCollisionWithCar =
                playerEntity.stateInterface.State.GetActionKeepState() != ActionKeepInConfig.Drive &&
                playerEntity.gamePlay.GameState != Components.GameState.AirPlane;
            _collisionLayers = needCollisionWithCar
                ? _baseCollisionLayers
                : _baseCollisionLayers & ~UnityLayers.VehicleLayerMask;
        }

        private void CalcuForNormal(PlayerEntity playerEntity)
        {
            if (null == playerEntity) return;
            if (!playerEntity.hasCameraObj) return;
            if (!playerEntity.hasCameraFinalOutputNew) return;
            if (!playerEntity.hasCameraStateOutputNew) return;
            
            UpdateCollisionLayerMask(playerEntity);
            var punchRotation = new Vector3(2 * playerEntity.orientation.PunchPitch,
                2 * playerEntity.orientation.PunchYaw, 0);
            
            playerEntity.cameraStateOutputNew.ArchorPosition =
                playerEntity.cameraArchor.ArchorPosition;
            UpdateCollisions(playerEntity.cameraStateOutputNew, playerEntity.cameraFinalOutputNew, punchRotation,
                playerEntity);
            SingletonManager.Get<DurationHelp>().Position = playerEntity.cameraStateOutputNew.ArchorPosition;
        }

        private void UpdateCollisions(CameraStateOutputNewComponent calsOut, CameraFinalOutputNewComponent camera,
            Vector3 punchRotation, PlayerEntity player)
        {
            Vector3 lToCamera = calsOut.Offset;
            var clientTime = player.time.ClientTime;

            float lNewDistance = lToCamera.magnitude;
          
            bool collided = false;

            var startingRotation = Quaternion.Euler(calsOut.ArchorEulerAngle + calsOut.EulerAngle + punchRotation);
            var realArchorStart = calsOut.ArchorOffset;

            if(player.stateInterface.State.GetActionState() != ActionInConfig.Gliding &&
               player.stateInterface.State.GetActionState() != ActionInConfig.Parachuting &&
               player.gamePlay.GameState != Components.GameState.AirPlane )
            TestArchorOffset(calsOut, player, ref realArchorStart);
            
            var archorRotation = Quaternion.Euler(0, calsOut.ArchorEulerAngle.y, 0);
            var postOffsetFactor = ActualStartingPosition(
                calsOut.ArchorPosition + archorRotation * realArchorStart, archorRotation,
                calsOut.ArchorPostOffset +calsOut.PostOffset, _collisionLayers);

            var startingPosition = calsOut.FinalArchorPosition =
                calsOut.ArchorPosition + archorRotation * (realArchorStart +
                                                           calsOut.ArchorPostOffset * postOffsetFactor +
                                                           calsOut.PostOffset * postOffsetFactor);

            float actualDistance = ActualDistance(startingRotation, startingPosition, lNewDistance, _collisionLayers);
            var factor = 1f;
            if (actualDistance < lNewDistance)
            {
                collided = true;
                if (actualDistance < _collisionOffsetStartDistance)
                {  
                    factor = Mathf.Lerp(_collisionOffsetStartDistance,actualDistance,actualDistance/lNewDistance) / lNewDistance;
                    startingPosition = calsOut.ArchorPosition + archorRotation * (realArchorStart +
                                                                                  calsOut.ArchorPostOffset * factor *
                                                                                  postOffsetFactor +
                                                                                  calsOut.PostOffset * postOffsetFactor *
                                                                                  factor);
                    actualDistance = ActualDistance(startingRotation, startingPosition, lNewDistance, _collisionLayers);
                    if (actualDistance >= lNewDistance) collided = false;
                }             
            }
            
            var finalPostOffset = calsOut.Offset.normalized * actualDistance;

            if (collided && !player.appearanceInterface.Appearance.IsFirstPerson)
            {
                camera.LastCollisionOffset = finalPostOffset;
                camera.LastCollisionTime = clientTime;
            }
            else if (clientTime - camera.LastCollisionTime < camera.PostTransitionTime &&
                     !player.appearanceInterface.Appearance.IsFirstPerson)
            {
                finalPostOffset = PositionLerp(camera.LastCollisionOffset, finalPostOffset,
                    (float) (clientTime - camera.LastCollisionTime) / camera.PostTransitionTime);
            }

            camera.PlayerFocusPosition = startingPosition;
            camera.Position = startingPosition + startingRotation * (finalPostOffset);
            camera.EulerAngle = calsOut.ArchorEulerAngle + calsOut.EulerAngle + punchRotation;
            camera.EulerAngle.x = YawPitchUtility.Normalize( camera.EulerAngle.x);
            camera.EulerAngle.y = YawPitchUtility.Normalize( camera.EulerAngle.y);
            camera.EulerAngle.z = YawPitchUtility.Normalize( camera.EulerAngle.z);
            camera.Fov = calsOut.Fov;
            camera.Far = calsOut.Far;
            camera.Near = calsOut.Near;

#if UNITY_EDITOR
            var p1 = calsOut.ArchorPosition;
            var p2 = calsOut.ArchorPosition + archorRotation * calsOut.ArchorOffset;
            var p3 = calsOut.ArchorPosition +
                     archorRotation * (calsOut.ArchorOffset + calsOut.ArchorPostOffset * factor * postOffsetFactor);
            var p4 = calsOut.ArchorPosition +
                     archorRotation * (calsOut.ArchorOffset + calsOut.ArchorPostOffset * factor * postOffsetFactor +
                                       calsOut.PostOffset * postOffsetFactor * factor);
            var p5 = p4 + startingRotation * (calsOut.Offset.normalized * actualDistance);
            Debug.DrawLine(p1, p2, Color.red);
            Debug.DrawLine(p2, p3, Color.green);
            Debug.DrawLine(p3, p4, Color.blue);
            Debug.DrawLine(p4, p5, Color.yellow);
#endif
        }
         
        private float ActualStartingPosition(Vector3 start, Quaternion rotation, Vector3 ofset, int collisionLayers)
        {
            var end = start + rotation * ofset;
            var dist = (end - start).magnitude;
            var dir = (end - start).normalized;
            RaycastHit lViewHit;

            Debug.DrawLine(start, end, Color.red);
            if (Physics.Raycast(start, dir, out lViewHit, dist, collisionLayers))
            {
                var hitDistance = lViewHit.distance;
               
                var colDis = Mathf.Max(hitDistance, MinCollisionDistance) - DistanceOffset - RaycastOffset;
                if (colDis < dist)
                    return colDis / dist;
            }

            return 1;
        }

        private float ActualDistance(Quaternion startingRotation, Vector3 startingPosition, float lNewDistance,
            int collisionLayers)
        {
            float actualDistance = lNewDistance;
            Vector3 lNewDirection = -startingRotation.Forward();
            for (int i = 0; i < _samplePoints.Count; i++)
            {
                RaycastHit lViewHit;
                var samplingPoint = _samplePoints[i];

                var rayCastStart = startingPosition;
                var xdelta = startingRotation.Right() * samplingPoint.x;
                var ydelta = startingRotation.Up() * samplingPoint.y;
                var zdelta = startingRotation.Forward() * samplingPoint.z;
                rayCastStart += xdelta;
                rayCastStart += ydelta;
                rayCastStart += zdelta;
                var castDir = lNewDirection;
                castDir += xdelta * RaycastDirFactor;
                castDir += ydelta * RaycastDirFactor;
                castDir += zdelta * RaycastDirFactor;
                //DebugDraw.DebugArrow(startingPosition, castDir, Color.red);
                Debug.DrawLine(rayCastStart, rayCastStart + castDir * lNewDistance, Color.magenta);
                if (Physics.Raycast(rayCastStart, castDir, out lViewHit, lNewDistance, collisionLayers))
                {
                    //_logger.DebugFormat("collided actual distance {0}", lViewHit.distance);
                    var hitDistance = lViewHit.distance;
                    var colDis = Mathf.Max(hitDistance, MinCollisionDistance) - DistanceOffset;
                    if (colDis < actualDistance)
                    {
                        actualDistance = colDis;
                    }
                }
            }
            return actualDistance;
        }

        public bool TestArchorOffset(CameraStateOutputNewComponent calsOut, PlayerEntity player, ref Vector3 startPoint)
        {
            float radius = player.characterContoller.Value.radius;
            var center = calsOut.ArchorPosition + calsOut.ArchorOffset.normalized * (player.characterContoller.Value.height - radius);
            var direction = calsOut.ArchorOffset.normalized;
            var maxDistance = calsOut.ArchorOffset.magnitude - player.characterContoller.Value.height; 
            RaycastHit hitInfo;
            
            if (Physics.CapsuleCast(center,center,radius, direction,out hitInfo, maxDistance))
            {
                var realdict = hitInfo.distance;
                if (realdict <= maxDistance)
                {
                    startPoint.y = hitInfo.point.y - calsOut.ArchorPosition.y - DistanceOffset;
                    return true;
                }
            }
            return false;
        }
        
        private Vector3 PositionLerp(Vector3 orig, Vector3 cur, float ratio)
        {
            Vector3 result = Vector3.zero;
            result.x = Mathf.Lerp(orig.x, cur.x, ratio);
            result.y = Mathf.Lerp(orig.y, cur.y, ratio);
            result.z = Mathf.Lerp(orig.z, cur.z, ratio);
            return result;
        }
        
        private void CalcuWhenObserving(PlayerEntity playerEntity)
        {
            int observedObjId = playerEntity.gamePlay.CameraEntityId;
            
            var observedPlayer = _playerContext.GetEntityWithEntityKey(new EntityKey(observedObjId, (short)EEntityType.Player));
            if (observedPlayer != null)
            {
                CalcuWhenObservePlayer(playerEntity, observedPlayer);
                return;
            }

            var observedFreeMove =
                _freeMoveContext.GetEntityWithEntityKey(new EntityKey(observedObjId, (short) EEntityType.FreeMove));
            if (observedFreeMove != null)
            {
                CalcuWhenObserveFreeMove(playerEntity, observedFreeMove);
            }

        }
        
        private void CalcuWhenObserveFreeMove(PlayerEntity playerEntity, FreeMoveEntity observedFreeMove)
        {
            var camera = playerEntity.cameraFinalOutputNew;
            var calsOut = playerEntity.cameraStateOutputNew;
            
            camera.EulerAngle = calsOut.ArchorEulerAngle + calsOut.EulerAngle;
            camera.EulerAngle.x = YawPitchUtility.Normalize( camera.EulerAngle.x);
            camera.EulerAngle.y = YawPitchUtility.Normalize( camera.EulerAngle.y);
            camera.EulerAngle.z = YawPitchUtility.Normalize( camera.EulerAngle.z);
            
            Vector3 offset = new Vector3(0, 2f, -2f);
            var rotation = Quaternion.Euler(camera.EulerAngle);
            float actualDistance = ActualDistance(rotation, observedFreeMove.position.Value, offset.magnitude, _collisionLayers);
            var finalPostOffset = offset.normalized * actualDistance;
            camera.Position = observedFreeMove.position.Value + rotation * (finalPostOffset);
            
            camera.Fov = playerEntity.cameraFinalOutputNew.Fov;
            
            playerEntity.position.Value = playerEntity.RootGo().transform.position = observedFreeMove.position.Value;
        }

        private void CalcuWhenObservePlayer(PlayerEntity playerEntity, PlayerEntity observedPlayer)
        {
            if (!observedPlayer.hasObserveCamera)
                return ;
            
            var camera = playerEntity.cameraFinalOutputNew;
            var playerData = observedPlayer.observeCamera;

            camera.Fov = playerData.Fov;
            camera.Position = playerData.CameraPosition;
            camera.EulerAngle = playerData.CameraEularAngle;

            playerEntity.position.Value = playerData.PlayerPosition;
            playerEntity.RootGo().transform.position = playerData.PlayerPosition;
        }
    }
}