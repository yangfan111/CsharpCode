﻿using System;
using App.Shared.Configuration;
using App.Shared.GameModules.Player.CharacterState;
using App.Shared.Player;
using Core.CameraControl;
using Core.CharacterController;
using Core.Compare;
using Core.Utils;
using ECM.Components;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class LandHandler
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(LandHandler));
        private static readonly float SteepAverRatio =
            SingletonManager.Get<CharacterStateConfigManager>().SteepAverRatio;

        private static readonly SpeedCalculator SpeedCalculator = new SpeedCalculator();

        private static readonly float DefaultSpeed = -0.1f;
        private static readonly float CompareMaxError = 0.00001f;


        public static void Move(Contexts contexts, PlayerEntity player, float deltaTime)
        {
            var controller = player.characterContoller.Value;

            SyncTransformFromComponent(player);
            
            // 分离角色
            //ResolveOverlapWithPlayer(contexts, player);
            
            // 旋转角色
            RotateCharacter(player, deltaTime);
            
            var finalVec = SpeedCalculator.CalcSpeed(player, deltaTime);
            
            var dist = finalVec * deltaTime;
            
            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.ClientMove);
            // 移动角色
            PlayerMoveUtility.Move(contexts, player, controller, dist, deltaTime);
            SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.ClientMove);

            SetIsCollider(player);
            ChangeVelocityYAfterCollision(player);
            CalcGroundSteep(player);
            CalcMoveVel(player, deltaTime);
            UpdatePlayerComponent(player);
        }

        /// <summary>
        /// 判断是否接触
        /// </summary>
        /// <param name="player"></param>
        private static void SetIsCollider(PlayerEntity player)
        {
            var controller = player.characterContoller.Value;
            if (controller.isGrounded
                || (controller.collisionFlags & CollisionFlags.Sides) != 0 || (controller.collisionFlags & CollisionFlags.Above) != 0)
            {
                player.playerMove.IsCollided = true; 
            }
        }

        private static void ResolveOverlapWithPlayer(Contexts contexts, PlayerEntity player)
        {
            Vector3 point1, point2;
            float radius;
            var controller = player.characterContoller.Value;
            if (controller.GetCurrentControllerType() != CharacterControllerType.UnityCharacterController)
            {
                return;
            }

            var characterController = controller.RealValue as CharacterController;

            if (characterController == null)
            {
                return;
            }
            
            var height = controller.height;
            radius = controller.radius;
            var center = controller.center;
            
            PhysicsCastHelper.GetCapsule(controller.transform.position, controller.transform.rotation, height, radius, center, controller.direction, out point1, out point2);
            var colliders = PhysicsCastHelper.CollidersArray;
            var hits = PhysicsCastHelper.CapsuleOverlap(
                point1,
                point2,
                radius,
                colliders,
                UnityLayerManager.GetLayerMask(EUnityLayerName.Player),
                QueryTriggerInteraction.Ignore,
                collider =>
                {
                    //和角色做分离，趴下不需要，自带分离
                    if (collider == controller.GetCollider() || !(collider is CapsuleCollider && (collider as CapsuleCollider).direction == 1))
                    {
                        return true;
                    }
                    return false;
                });

            int MaxStep = 1;
            // calculate penetration
            for (int i = 0; i < hits; ++i)
            {
                Vector3 resolutionDirection = Vector3.up;
                float resolutionDistance = 0f;
                Transform overlappedTransform = colliders[i].GetComponent<Transform>();
                for (int j = 0; j < MaxStep; ++j)
                {
                    if (Physics.ComputePenetration(characterController,
                        characterController.transform.position,
                        characterController.transform.rotation,
                        colliders[i],
                        colliders[i].transform.position,
                        colliders[i].transform.rotation,
                        out resolutionDirection,
                        out resolutionDistance
                    ))
                    {
                        characterController.Move(new Vector3(0,-0.01f,0));                        
                        _logger.DebugFormat(
                            "ResolveOverlapWithPlayer ResolveOverlapWithPlayer resolutionDirection:{0}, resolutionDistance:{1}, hitCollider:{2}",
                            resolutionDirection.ToStringExt(), resolutionDistance, overlappedTransform.name);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 计算角色实际的移动速度
        /// </summary>
        /// <param name="player"></param>
        /// <param name="deltaTime"></param>
        private static void CalcMoveVel(PlayerEntity player, float deltaTime)
        {
            var newPos = player.RootGo().transform.position;
            newPos.y = 0;
            var prevPos = player.position.Value;
            prevPos.y = 0;
            player.playerMove.MoveVel = Vector3.Distance(newPos, prevPos) / deltaTime;
        }

        private static void UpdatePlayerComponent(PlayerEntity player)
        {
            player.stateInterface.State.SetMoveInWater(SpeedCalculator.IsLimitSpeedInWater);
            player.playerMove.Velocity = SpeedCalculator.CurrentVelocity;
            var syncTransform = player.RootGo().transform;
            player.position.Value = syncTransform.position;
            player.orientation.ModelPitch = YawPitchUtility.Normalize(syncTransform.rotation.eulerAngles.x);
            player.orientation.ModelYaw = YawPitchUtility.Normalize(syncTransform.rotation.eulerAngles.y);
        }

        /// <summary>
        /// 同步角色的位置和旋转
        /// </summary>
        /// <param name="player"></param>
        private static void SyncTransformFromComponent(PlayerEntity player)
        {
            var state = player.stateInterface.State;
            var postureInConfig = state.GetNextPostureState();
            var controller = player.characterContoller.Value;
            controller.SetCurrentControllerType(postureInConfig);
            controller.SetCharacterPosition(player.position.Value);
            controller.SetCharacterRotation(player.orientation.ModelView);
        }

        /// <summary>
        /// 旋转角色
        /// </summary>
        /// <param name="player"></param>
        /// <param name="deltaTime"></param>
        private static void RotateCharacter(PlayerEntity player, float deltaTime)
        {
            ICharacterControllerContext controller= player.characterContoller.Value;
            controller.Rotate(player.orientation.RotationYaw, deltaTime);
        }

        /// <summary>
        /// 计算地面坡度
        /// </summary>
        /// <param name="player"></param>
        private static void CalcGroundSteep(PlayerEntity player)
        {
            var syncTransform = player.RootGo().transform;
            //坡度计算
            bool exceedSteepLimit = false;
            // 在宏观尺度, Move的距离为实际距离, 在微观尺度, Move的距离为水平距离
            var actualMovement = (syncTransform.position - player.position.Value);
            var horizontalComponent =
                Mathf.Sqrt(actualMovement.x * actualMovement.x + actualMovement.z * actualMovement.z);

            var steep = CompareUtility.IsApproximatelyEqual(horizontalComponent, 0.0f, CompareMaxError)
                ? 0
                : actualMovement.y / horizontalComponent; //+ (1 - ratio) * lastSteep;
            
            float aveSteep = steep * SteepAverRatio + (1 - SteepAverRatio) * player.playerMove.SteepAverage;
            player.playerMove.SteepAverage = aveSteep;
            player.playerMove.Steep = steep;


            player.stateInterface.State.SetSteepAngle(aveSteep);
        }

        private static void ChangeVelocityYAfterCollision(PlayerEntity player)
        {
            var controller = player.characterContoller.Value;
            var velocityY = SpeedCalculator.CurrentVelocity.y;
            
            // 碰到了天花板
            if ((controller.collisionFlags & CollisionFlags.Above) != 0)
            {
                //只有在上跳的时候，把速度减为0
                if (velocityY > 0)
                {
                    velocityY = 0f;
                }
            }

            _logger.DebugFormat("ground info:isOnGround:{0}, isValidGround:{1}, isinLedge:{2}, isOutLedge:{3}, isOnStep:{4},groundNormal:{5}, angle:{6}", SpeedCalculator.GroundInfo.isOnGround, SpeedCalculator.GroundInfo.isValidGround
            ,SpeedCalculator.GroundInfo.isOnLedgeSolidSide, SpeedCalculator.GroundInfo.isOnLedgeEmptySide, SpeedCalculator.GroundInfo.isOnStep, SpeedCalculator.GroundInfo.groundNormal, Vector3.Angle(Vector3.up, SpeedCalculator.GroundInfo.groundNormal));
            
            // 碰到地面的情况处理
            // 1.还在下滑过程中，y速度继续向下降
            // 2.在地面上，y速度降为0
            if (velocityY < 0 && controller.isGrounded)
            {
                if ( SpeedCalculator.KeepSlide)
                {
                    velocityY = Mathf.Min(DefaultSpeed, velocityY);
                }
                else
                {
                    velocityY = DefaultSpeed;
                }
                player.playerMove.IsGround = true;
            }

            SpeedCalculator.CurrentVelocity.y = velocityY;
        }

    }


    public class SpeedCalculator
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SpeedCalculator));
        
        private static readonly float BeginSlowDownInWater =
            SingletonManager.Get<CharacterStateConfigManager>().BeginSlowDownInWater;

        private static readonly float StopSlowDownInWater =
            SingletonManager.Get<CharacterStateConfigManager>().StopSlowDownInWater;
        
        private static readonly float DeltaVel = 0.1f;
        private static readonly float TOLERANCE = 0.001f;
        
        public Vector3 LastVelocity;

        public GroundHit GroundInfo;
        public Vector3 LastNormal;

        /// <summary>
        /// 计算当前的速度
        /// </summary>
        public Vector4 CurrentVelocity;
        /// <summary>
        /// 计算当前的速度的偏移值
        /// </summary>
        public Vector4 CurrentVelocityOffset;
        /// <summary>
        /// 当前的速度的偏移值，未经过旋转
        /// </summary>
        private Vector4 CurrentVelocityOffsetRaw;
        public float DeltaTime;
        /// <summary>
        /// 沿着斜面下滑的方向
        /// </summary>
        public Vector3 SlideDirection;
        /// <summary>
        /// tan的斜面角度
        /// </summary>
        public float SlideDirectionTan;

        /// <summary>
        /// 是否保持斜坡滑落状态
        /// </summary>
        public bool KeepSlide { get; private set; }

        public bool IsLimitSpeedInWater;
        private static readonly float SlideSlopeOffsetUp = 8f;
		private static readonly float SlideSlopeOffsetLow = 0f;
        private static readonly float SlideSlopeMinSpeed = 2.5f;
        private static readonly float SlideMaxAngle = 90.0f;
        private static readonly float MinSlideVecValue = 0.01f;

        /// <summary>
        /// 初始化变量
        /// </summary>
        public void Reset()
        {
            LastVelocity = Vector3.zero;
            LastNormal = Vector3.zero;

            CurrentVelocity = CurrentVelocityOffset = CurrentVelocityOffsetRaw = Vector4.zero;
            DeltaTime = 0;

            SlideDirection = Vector3.zero;
            SlideDirectionTan = 0;
            IsLimitSpeedInWater = false;
            KeepSlide = false;
        }

        /// <summary>
        /// 计算角色移动速度
        /// </summary>
        /// <param name="player"></param>
        /// <param name="deltaTime"></param>
        public Vector3 CalcSpeed(PlayerEntity player, float deltaTime)
        {
            Reset();
            SetPlayerPrevInfo(player,deltaTime);
            CalcSlideSlopeDirection(player);
            SetIsExceedSlopeLimit(player);
            CalcCurrentSpeed(player);
            return CalcFinalSpeed(player, deltaTime);
        }

        private RaycastHit hitInfo;

        private Vector3 CalcFinalSpeed(PlayerEntity player, float deltaTime)
        {
            var scaledVel = CurrentVelocity;
            scaledVel.Scale(new Vector4(player.playerMove.MoveSpeedRatio, 1, player.playerMove.MoveSpeedRatio, 1));
            var finalSpeed = scaledVel + CurrentVelocityOffset;

            if (finalSpeed.magnitude > 0 && player.stateInterface.State.GetNextPostureState() != PostureInConfig.Jump && GroundInfo.isValidGround)
            {
                finalSpeed.y = -0.3f / deltaTime;
            }

            return finalSpeed;
        }

        /// <summary>
        /// 设置角色上一帧的信息
        /// </summary>
        /// <param name="player"></param>
        /// <param name="time"></param>
        private void SetPlayerPrevInfo(PlayerEntity player,float time)
        {
            LastVelocity = Quaternion.Inverse(player.orientation.RotationYaw) * player.playerMove.Velocity.ToVector4();
            CurrentVelocity = LastVelocity.ToVector4();
            DeltaTime = time;
            GroundInfo = player.characterContoller.Value.GetGroundHit;
            LastNormal = GroundInfo.surfaceNormal;
            //Logger.InfoFormat("angle:{0}", Vector3.Angle(LastNormal, Vector3.up));
        }

        /// <summary>
        /// 根据上一帧法线的方向，计算沿着障碍我移动的方向
        /// </summary>
        /// <param name="player"></param>
        private void CalcSlideSlopeDirection(PlayerEntity player)
        {
            var controller = player.characterContoller.Value;
            SlideDirection = GetDirectionWhenSlideAlongSteep(controller.transform.forward);
            var xzcomp = Mathf.Sqrt(SlideDirection.x * SlideDirection.x + SlideDirection.z * SlideDirection.z);
            SlideDirectionTan = Math.Abs(xzcomp) < TOLERANCE ? 0 : SlideDirection.y / xzcomp;
        }

        /// <summary>
        /// 是否需要下滑
        /// </summary>
        /// <param name="player"></param>
        private void SetIsExceedSlopeLimit(PlayerEntity player)
        {
            var controller = player.characterContoller.Value;
            if (IsSlideAloneSlope(controller, SlideSlopeOffsetLow))
            {
                player.stateInterface.State.SetExceedSlopeLimit(true);
            }
            else
            {
                player.stateInterface.State.SetExceedSlopeLimit(false);
            }
        }

        /// <summary>
        /// 获取沿着斜面下滑的高度
        /// </summary>
        /// <param name="forward"></param>
        /// <returns></returns>
        private Vector3 GetDirectionWhenSlideAlongSteep(Vector3 forward)
        {
            var prevSpeed = CurrentVelocity;
            var collisionNormal = LastNormal;
            if (CompareUtility.IsApproximatelyEqual(collisionNormal.normalized, Vector3.up) ||
                CompareUtility.IsApproximatelyEqual(collisionNormal.normalized, Vector3.zero))
            {
                return Vector3.zero;
            }

            if (CompareUtility.IsApproximatelyEqual(prevSpeed.x, 0f, 0.001f) &&
                CompareUtility.IsApproximatelyEqual(prevSpeed.z, 0f, 0.001f))
            {
                prevSpeed.x = forward.x;
                prevSpeed.z = forward.z;
            }

            prevSpeed.y = prevSpeed.y - SpeedManager.SlideSlopeGravity * DeltaTime;

            var slopeDir = new Vector3();
            slopeDir.y = -Mathf.Sqrt(1 - collisionNormal.y * collisionNormal.y);

            var x = Mathf.Abs(collisionNormal.x);
            var z = Mathf.Abs(collisionNormal.z);
            var xzComponent = Mathf.Sqrt((1 - slopeDir.y * slopeDir.y) * (x + z) * (x + z) / (x * x + z * z));

            slopeDir.x = xzComponent * collisionNormal.x / (x + z);
            slopeDir.z = xzComponent * collisionNormal.z / (x + z);

            slopeDir *= Mathf.Abs(prevSpeed.y / slopeDir.y);
            
            // debug
            if (IsVector3Evil(slopeDir))
            {
                Logger.ErrorFormat("slopeDir calc is illegle, slopeDir:{0}, prevSpeed:{1}, LastNormal:{2}, forward:{3}, set slopeDir to zero!!!\ngroundInfo:{4}", slopeDir.ToStringExt(), CurrentVelocity.ToStringExt(), LastNormal.ToStringExt(), forward.ToStringExt(),
                    GroundInfo.ToString());
                slopeDir = Vector3.zero;
            }
            
            return slopeDir;
        }

        public static bool IsVector3Evil(Vector3 vector)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (float.IsNaN(vector[0]) || float.IsInfinity(vector[0]))
                {
                    return true;
                }
            }

            return false;
        }
        
        

        /// <summary>
        /// 计算当前速度
        /// </summary>
        /// <param name="player"></param>
        private void CalcCurrentSpeed(PlayerEntity player)
        {
            var currentBuff = CalcuBuff(player);
            var offsetSlope = Vector3.zero;
            // 需要在斜面下滑
            if (IsSlideAloneSlope(player.characterContoller.Value, SlideSlopeOffsetLow))
            {
                offsetSlope = HandleSlideAloneSlope(CalcOnPlayerSpeed(SlideDirection,player.characterContoller.Value.transform.forward));
                KeepSlide = true;
                // 只有在速度大于SlideSlopeMinSpeed时才播放下滑动画
                if (Mathf.Abs(CurrentVelocity.y) > SlideSlopeMinSpeed )
                {
					player.stateInterface.State.SetSlide(true);
                    Logger.DebugFormat("set ani to slide, curvelocity:{0},IsSlideAloneSlope:{1}", CurrentVelocity.ToStringExt(), IsSlideAloneSlope(player.characterContoller.Value, SlideSlopeOffsetUp));
                }
                Logger.DebugFormat("set to move slide, slide alone slope, vec:{0}, Angle:{1}, slopeVec:{2}", CurrentVelocity.ToStringExt(), Vector3.Angle(Vector3.up, GroundInfo.groundNormal), SlideDirection.ToVector4());
            }
            else if (IsSlideFreefall(player))
            {
                KeepSlide = true;
                player.stateInterface.State.SetSlide(true);
                CalcuSpeedIfNotSlide(player, DeltaTime, currentBuff, LastVelocity);
                Logger.DebugFormat("freefall slide, curvelocity:{0}", CurrentVelocity.ToStringExt());
            }
            else
            {
                KeepSlide = false;
                player.stateInterface.State.SetSlide(false);
                CalcuSpeedIfNotSlide(player, DeltaTime, currentBuff, LastVelocity);
                if (IsHeadingOnPlain(player)) //贴合地面                                          
                {
                    CurrentVelocity.y = SlideDirectionTan * CurrentVelocity.magnitude - 0.1f;
                }
            }

            // 计算最终的速度偏移
            CurrentVelocityOffset = player.orientation.RotationYaw * CurrentVelocityOffsetRaw + offsetSlope;

            if (IsJumpHitScene(player))
            {
                CurrentVelocity = JumpSpeedProject(player.characterContoller.Value.transform, player.characterContoller.Value.GetCharacterControllerHitInfo(HitType.Forward).HitNormal, CurrentVelocity);
                Logger.DebugFormat("project speed, curvelocity:{0}", CurrentVelocity.ToStringExt());
            }
            
            // debug
            if (IsVector3Evil(CurrentVelocity))
            {
                Logger.ErrorFormat("final CurrentVelocity calc is illegle, slopeDir:{0}, prevSpeed:{1}, LastNormal:{2}, set slopeDir to zero!!!, GroundInfo:{3}", CurrentVelocity.ToStringExt(), LastVelocity.ToStringExt(), LastNormal.ToStringExt(),
                    GroundInfo.ToString());
                CurrentVelocity = Vector3.zero;
            }
        }
        

        /// <summary>
        /// 跳跃过程是否碰到场景
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static bool IsJumpHitScene(PlayerEntity player)
        {
            return player.stateInterface.State.GetNextPostureState() == PostureInConfig.Jump &&
                   player.characterContoller.Value.GetCharacterControllerHitInfo(HitType.Forward).Valid;
        }

        /// <summary>
        /// 下滑到空中
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool IsSlideFreefall(PlayerEntity player)
        {
            return player.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Slide &&
                   !GroundInfo.isOnGround && SharedConfig.EnableSlide;
        }

        /// <summary>
        /// 计算移动速度buff
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private float CalcuBuff(PlayerEntity player)
        {
            IsLimitSpeedInWater = LimitSpeedInWater(player);
            var steepConfig = SingletonManager.Get<CharacterStateConfigManager>().SteepConfig;
            var buff = steepConfig.CalcSteepBuff(player.playerMove.SteepAverage) + (IsLimitSpeedInWater ? -0.3f : 0) +
                       player.playerMove.SpeedAffect;
            buff = buff < -1.0f ? -1.0f : buff;
            return buff;
        }
        
        public static bool LimitSpeedInWater(PlayerEntity player)
        {
            float depth = SingletonManager.Get<MapConfigManager>().WaterSurfaceHeight(player.position.Value) -
                          player.position.Value.y;

            return (depth >= BeginSlowDownInWater)
                   ||
                   (depth >= StopSlowDownInWater && player.stateInterface.State.GetSteepSlowDown() >= PlayerStateUpdateSystem.LimitSprint);
        }

        private bool IsSlideAloneSlope( ICharacterControllerContext controller, float angleOffset)
        {
            return GroundInfo.IsSlideSlopeGround(controller.slopeLimit + angleOffset) && Vector3.Dot(LastNormal, Vector3.up) > 0.0f &&
                   LastVelocity.y <= 0.0f && SharedConfig.EnableSlide && Vector3.Angle(LastNormal, Vector3.up) <= SlideMaxAngle;
        }

        private Vector3 CalcOnPlayerSpeed(Vector3 origionVec, Vector3 defaultForward)
        {
            Vector3 ret = origionVec;
            if (GroundInfo.IsOnPlayer())
            {
                Vector3 vec = new Vector3(origionVec.x, 0, origionVec.z);
                if (vec.magnitude < MinSlideVecValue)
                {
                    ret = defaultForward * Mathf.Max(Mathf.Abs(origionVec.y), MinSlideVecValue);
                }
            }
            return ret;
        }

        private Vector3 HandleSlideAloneSlope(Vector3 slopeVec)
        {
            Vector3 ret;
//            CurVelocity = new Vector4(CurVelocity.x, slopeVec.y, CurVelocity.z, 1f);
//            offsetSlope = new Vector3(-CurVelocity.x + slopeVec.x, 0f, -CurVelocity.z + slopeVec.z);
            CurrentVelocity = new Vector4(slopeVec.x, slopeVec.y, slopeVec.z, 1f);
            ret = Vector3.zero;
            return ret;
        }

        /// <summary>
        /// 计算速度
        /// </summary>
        /// <param name="player"></param>
        /// <param name="deltaTime"></param>
        /// <param name="buff"></param>
        /// <param name="lastVel"></param>
        private void CalcuSpeedIfNotSlide(PlayerEntity player, float deltaTime, float buff, Vector3 lastVel)
        {
            CurrentVelocityOffsetRaw = player.stateInterface.State.GetSpeedOffset(buff);
            CurrentVelocity = player.orientation.RotationYaw * player.stateInterface.State.GetSpeed(lastVel, deltaTime, buff);
        }

        /// <summary>
        /// 是否需要贴着斜面走
        /// </summary>
        /// <returns></returns>
        private bool IsHeadingOnPlain(PlayerEntity player)
        {
            return CurrentVelocity.y < 0 &&
                   GroundInfo.isOnGround && GroundInfo.isValidGround && Vector3.Angle(LastNormal, Vector3.up) <= player.characterContoller.Value.slopeLimit;
        }

        /// <summary>
        /// 水平方向减速
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="lastHitNormal"></param>
        /// <param name="vel"></param>
        /// <returns></returns>
        private static Vector3 JumpSpeedProject(Transform transform, Vector3 lastHitNormal, Vector3 vel)
        {
            var surfaceNormal = lastHitNormal;
            var tmpVec = new Vector3(vel.x, 0, vel.z);
            var newVelocity = GetDirectionTangentToSurfaceCustom(tmpVec, surfaceNormal, transform.forward) *
                              tmpVec.magnitude;
            //DebugDraw.DebugArrow(transform.position, surfaceNormal, Color.red, 20f);
            //DebugDraw.DebugArrow(transform.position, tmpVec, Color.green, 20f);
            //DebugDraw.DebugArrow(transform.position, newVelocity, Color.blue, 20f);
            newVelocity = Vector3.ProjectOnPlane(newVelocity, Vector3.up);
            newVelocity = tmpVec.normalized * newVelocity.magnitude;
            //DebugDraw.DebugArrow(transform.position, newVelocity, Color.magenta, 20f);
            vel.x = newVelocity.x;
            vel.z = newVelocity.z;
            return vel;
        }

        private static Vector3 GetDirectionTangentToSurfaceCustom(Vector3 direction, Vector3 surfaceNormal,
            Vector3 characterFoward)
        {
            Vector3 directionLeft = Vector3.Cross(
                direction,
                Vector3.up);

            if (CompareUtility.IsApproximatelyEqual(directionLeft, Vector3.zero))
            {
                directionLeft = Vector3.Cross(
                    characterFoward,
                    Vector3.up);
            }

            return Vector3.Cross(surfaceNormal, directionLeft).normalized;
        }
    }
}