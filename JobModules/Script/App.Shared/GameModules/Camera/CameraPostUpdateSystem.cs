using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.GameModules.Camera.Utils;
using Core.CameraControl;
using Core.GameModule.Interface;
using Core.GameTime;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using UnityEngine.AI;
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

        private float _collisionOffsetStartDistance = 2f;
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
            InteralExecute(_playerContext.flagSelfEntity);
        }
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var playerEntity = owner.OwnerEntity as PlayerEntity;
            InteralExecute(playerEntity);
            CopyFinalOutputToUploadComponent(playerEntity.cameraFinalOutputNew, playerEntity.cameraStateUpload);
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

        private void InteralExecute(PlayerEntity playerEntity)
        {
            if (null == playerEntity) return;
            if (!playerEntity.hasCameraObj) return;
            if (!playerEntity.hasCameraFinalOutputNew) return;
            if (!playerEntity.hasCameraStateOutputNew) return;
            
            playerEntity.cameraStateOutputNew.ArchorPosition =
                playerEntity.cameraArchor.ArchorPosition;
            var punchRotation = new Vector3(2 * playerEntity.orientation.PunchPitch,
                2 * playerEntity.orientation.PunchYaw, 0);
            UpdateCollisionLayerMask(playerEntity);
            UpdateCollisions(playerEntity.cameraStateOutputNew, playerEntity.cameraFinalOutputNew, punchRotation);
            SingletonManager.Get<DurationHelp>().Position = playerEntity.cameraStateOutputNew.ArchorPosition;
        }

        private void UpdateCollisionLayerMask(PlayerEntity playerEntity)
        {
            bool needCollisionWithCar =
                playerEntity.stateInterface.State.GetActionKeepState() != ActionKeepInConfig.Drive;
            _collisionLayers = needCollisionWithCar
                ? _baseCollisionLayers
                : _baseCollisionLayers & ~UnityLayers.VehicleLayerMask;
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

        private void UpdateCollisions(CameraStateOutputNewComponent calsOut, CameraFinalOutputNewComponent camera,
            Vector3 punchRotation)
        {
            Vector3 lToCamera = calsOut.Offset;

            float lNewDistance = lToCamera.magnitude;
          
            bool collided = false;

            var archorRotation = Quaternion.Euler(0, calsOut.ArchorEulerAngle.y, 0);
            var postOffsetFactor = ActualStartingPosition(
                calsOut.ArchorPosition + archorRotation * calsOut.ArchorOffset, archorRotation,
                calsOut.ArchorPostOffset +calsOut.PostOffset, _collisionLayers);
            var startingPosition = calsOut.FinalArchorPosition =
                calsOut.ArchorPosition + archorRotation * (calsOut.ArchorOffset +
                                                           calsOut.ArchorPostOffset * postOffsetFactor +
                                                           calsOut.PostOffset * postOffsetFactor);
            var startingRotation = Quaternion.Euler(calsOut.ArchorEulerAngle + calsOut.EulerAngle + punchRotation);

            float actualDistance = ActualDistance(startingRotation, startingPosition, lNewDistance, _collisionLayers);
            var factor = 1f;
            if (actualDistance < lNewDistance)
            {
                if (actualDistance < _collisionOffsetStartDistance)
                {
                    factor = actualDistance / lNewDistance;
                    startingPosition = calsOut.ArchorPosition + archorRotation * (calsOut.ArchorOffset +
                                                                                  calsOut.ArchorPostOffset * factor *
                                                                                  postOffsetFactor +
                                                                                  calsOut.PostOffset * postOffsetFactor *
                                                                                  factor);
                    actualDistance = ActualDistance(startingRotation, startingPosition, lNewDistance, _collisionLayers);
                }
            }


            camera.PlayerFocusPosition = startingPosition;
            camera.Position = startingPosition + startingRotation * (calsOut.Offset.normalized * actualDistance);
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
            Debug.DrawLine(p1, p2, Color .red);
            Debug.DrawLine(p2, p3, Color.green);
            Debug.DrawLine(p3, p4, Color.blue);
            Debug.DrawLine(p4, p5, Color.yellow);

#endif
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


       
    }
}