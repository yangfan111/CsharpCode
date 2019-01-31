using App.Shared.Configuration;
using App.Shared.GameModules.Player.CharacterState;
using App.Shared.Player;
using Core.CameraControl;
using Core.CharacterController;
using Core.Compare;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using ECM.Components;
using Poly2Tri;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class LandHandler
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(LandHandler));

        private static readonly float SteepLimitBegin =
            Mathf.Tan(Mathf.Deg2Rad * SingletonManager.Get<CharacterStateConfigManager>().SteepLimitBegin);

        private static readonly float SteepLimitStop =
            Mathf.Tan(Mathf.Deg2Rad * SingletonManager.Get<CharacterStateConfigManager>().SteepLimitStop);

        private static readonly float SteepAverRatio =
            SingletonManager.Get<CharacterStateConfigManager>().SteepAverRatio;

        private static readonly SpeedCalculator _speedCalculator = new SpeedCalculator();

        private static readonly float DefaultSpeed = -0.1f;
        
        

        public static void Move(Contexts contexts, PlayerEntity player, float deltaTime)
        {
            var controller = player.characterContoller.Value;

            RotBox(player, deltaTime);
            
            _speedCalculator.Reset();
            _speedCalculator.GetValueFromPlayer(player,deltaTime);
            _speedCalculator.CalcuCurSlope(player);
            _speedCalculator.CalcuCurSpeed(player, deltaTime);

            var dist = CalcuDistToMove(player, _speedCalculator);
            
            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.ClientMove);
            PlayerMoveUtility.Move(contexts, player, controller, dist, deltaTime);
            SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.ClientMove);

            ChangeVelocityOnYAxisAfterCollision(player, _speedCalculator.normalAngle);
            CalcuNewSteep(player);
            UpdatePlayerComponent(player);
        }

        private static void UpdatePlayerComponent(PlayerEntity player)
        {
            player.stateInterface.State.SetMoveInWater(_speedCalculator.IsLimitSpeedInWater);
            var controller = player.characterContoller.Value;
            player.playerMove.Velocity = _speedCalculator.CurVelocity;
            var syncTransform = player.RootGo().transform;
            player.position.Value = syncTransform.position;
            player.orientation.ModelPitch = YawPitchUtility.Normalize(syncTransform.rotation.eulerAngles.x);
            player.orientation.ModelYaw = YawPitchUtility.Normalize(syncTransform.rotation.eulerAngles.y);
        }

        private static Vector4 CalcuDistToMove(PlayerEntity player, SpeedCalculator calculator)
        {
            var scaledVel = calculator.CurVelocity;
            scaledVel.Scale(new Vector4(player.playerMove.MoveSpeedRatio, 1, player.playerMove.MoveSpeedRatio, 1));
            var dist = (scaledVel + _speedCalculator.VelocityOffset) * calculator.deltaTime;
            return dist;
        }


        private static void RotBox(PlayerEntity player, float deltaTime)
        {
            var state = player.stateInterface.State;
            var postureInConfig = state.GetNextPostureState();
            var controller = player.characterContoller.Value;
            controller.SetCurrentControllerType(postureInConfig);
            controller.SetCharacterPosition(player.position.Value);
            controller.SetCharacterRotation(player.orientation.ModelView);
            controller.Rotate(player.orientation.RotationYaw, deltaTime);
        }

        private static void CalcuNewSteep(PlayerEntity player)
        {
            var syncTransform = player.RootGo().transform;
            //坡度计算
            bool exceedSteepLimit = false;
            // 在宏观尺度, Move的距离为实际距离, 在微观尺度, Move的距离为水平距离
            var actualMovement = (syncTransform.position - player.position.Value);
            var horizontalComponent =
                Mathf.Sqrt(actualMovement.x * actualMovement.x + actualMovement.z * actualMovement.z);

            var steep = CompareUtility.IsApproximatelyEqual(horizontalComponent, 0.0f)
                ? 0
                : actualMovement.y / horizontalComponent; //+ (1 - ratio) * lastSteep;
            float lastSteep = player.playerMove.SteepAverage;
            float aveSteep = steep * SteepAverRatio + (1 - SteepAverRatio) * lastSteep;
            player.playerMove.SteepAverage = aveSteep;
            player.playerMove.Steep = steep;
            steep = aveSteep;

            if (horizontalComponent > 0)
            {
                exceedSteepLimit =
                    (steep >= SteepLimitBegin) ||
                    ((steep > SteepLimitStop) && player.stateInterface.State.IsSlowDown());
            }

            player.stateInterface.State.SetSteepSlope(exceedSteepLimit);
        }

        private static void ChangeVelocityOnYAxisAfterCollision(PlayerEntity player, float latestCollisionSlope)
        {
            var controller = player.characterContoller.Value;
            var velocity_y = _speedCalculator.CurVelocity.y;
            // 碰到了天花板
            if ((controller.collisionFlags & CollisionFlags.Above) != 0)
            {
                player.playerMove.IsCollided = true;
                //只有在上跳的时候，把速度减为0
                if (velocity_y > 0)
                {
                    velocity_y = 0f;
                }
            }

            _logger.DebugFormat("ground info:isOnGround:{0}, isValidGround:{1}, isinLedge:{2}, isOutLedge:{3}, isOnStep:{4},groundNormal:{5}, angle:{6}", _speedCalculator.groundInfo.isOnGround, _speedCalculator.groundInfo.isValidGround
            ,_speedCalculator.groundInfo.isOnLedgeSolidSide, _speedCalculator.groundInfo.isOnLedgeEmptySide, _speedCalculator.groundInfo.isOnStep, _speedCalculator.groundInfo.groundNormal, Vector3.Angle(Vector3.up, _speedCalculator.groundInfo.groundNormal));
            //collisionFlags会同时返回Above和Below
            if (controller.isGrounded
                || (controller.collisionFlags & CollisionFlags.Sides) != 0)
            {
                player.playerMove.IsCollided = true; 
            }
            
            // 一个人往地面上的半球跳跃时，碰撞结果都是CollisionFlags.Sides，因此isGrounded为false
            // 1.如果Y轴分量大于0，则人物继续上升
            // 2.如果人物不能在碰撞点站住，则会加速下滑
            // 3.latestCollisionSlope < controller.slopeLimit条件一直不满足，角色一直下落，但是没有向下移动，可以认为CharacterController认为已经落地了,靠斜面下滑到latestCollisionSlope < controller.slopeLimit来最终落地
            if (velocity_y < 0 && controller.isGrounded)
            {

                if (_speedCalculator.groundInfo.IsSlideSlopeGround(controller.slopeLimit))
                {
                    velocity_y = Mathf.Min(DefaultSpeed, velocity_y);
                }
                else
                {
                    velocity_y = DefaultSpeed;
                }
                player.playerMove.IsGround = true;
            }
            
            

            _speedCalculator.CurVelocity.y = velocity_y;
            //_logger.InfoFormat("speed y:{0}", velocity_y);
        }

    }


    public class SpeedCalculator
    {
        
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SpeedCalculator));
        
        private static readonly float BeginSlowDownInWater =
            SingletonManager.Get<CharacterStateConfigManager>().BeginSlowDownInWater;

        private static readonly float StopSlowDownInWater =
            SingletonManager.Get<CharacterStateConfigManager>().StopSlowDownInWater;
        
        private static readonly float deltaVel = 0.1f;
        
        public Vector3 lastVelocity;

        public GroundHit groundInfo;
        // 最近一次与世界碰撞时碰撞点的法向
        public float normalAngle;
        public Vector3 lastNormal;

        public Vector4 CurVelocity;
        public Vector4 VelocityOffset;
        public float deltaTime;
        public Vector3 direction;
        public float velocityTan;

        public bool IsLimitSpeedInWater;

        public void Reset()
        {
            lastVelocity = Vector3.zero;
            normalAngle = 0;
            lastNormal = Vector3.zero;

            CurVelocity = VelocityOffset = Vector4.zero;
            deltaTime = 0;

            direction = Vector3.zero;
            velocityTan = 0;
            IsLimitSpeedInWater = false;
        }

        public void GetValueFromPlayer(PlayerEntity player,float time)
        {
            lastVelocity = Quaternion.Inverse(player.orientation.RotationYaw) * player.playerMove.Velocity.ToVector4();
            CurVelocity = lastVelocity.ToVector4();
            normalAngle = Vector3.Angle(lastNormal, Vector3.up);
            deltaTime = time;
            groundInfo = player.characterContoller.Value.GetGroundHit;
            lastNormal = groundInfo.groundNormal;

        }

        public void CalcuCurSlope(PlayerEntity player)
        {
            var controller = player.characterContoller.Value;
            direction = GetDirectionWhenSlideAlongSteep(controller.transform.forward);
            var xzcomp = Mathf.Sqrt(direction.x * direction.x + direction.z * direction.z);
            float veloctySlope = xzcomp == 0 ? 0 : direction.y / xzcomp;
            velocityTan = veloctySlope;
            
            if (IsSlideAloneSlope(controller))
            {
                player.stateInterface.State.SetExceedSlopeLimit(true);
            }
            else
            {
                player.stateInterface.State.SetExceedSlopeLimit(false);
            }

        }

        private Vector3 GetDirectionWhenSlideAlongSteep(Vector3 forward)
        {
            var prevSpeed = CurVelocity;
            var collisionNormal = lastNormal;
            if (CompareUtility.IsApproximatelyEqual(collisionNormal.normalized, Vector3.up))
            {
                return Vector3.zero;
            }

            if (CompareUtility.IsApproximatelyEqual(prevSpeed.x, 0f, 0.001f) &&
                CompareUtility.IsApproximatelyEqual(prevSpeed.z, 0f, 0.001f))
            {
                prevSpeed.x = forward.x;
                prevSpeed.z = forward.z;
            }

            prevSpeed.y = prevSpeed.y - SpeedManager.SlideSlopeGravity * deltaTime;

            var slopeDir = new Vector3();
            slopeDir.y = -Mathf.Sqrt(1 - collisionNormal.y * collisionNormal.y);

            var x = Mathf.Abs(collisionNormal.x);
            var z = Mathf.Abs(collisionNormal.z);
            var xzComponent = Mathf.Sqrt((1 - slopeDir.y * slopeDir.y) * (x + z) * (x + z) / (x * x + z * z));

            slopeDir.x = xzComponent * collisionNormal.x / (x + z);
            slopeDir.z = xzComponent * collisionNormal.z / (x + z);

            slopeDir *= Mathf.Abs(prevSpeed.y / slopeDir.y);
            return slopeDir;
        }

        public void CalcuCurSpeed(PlayerEntity player, float deltaTime)
        {
            var buff = CalcuBuff(player);
            var offsetSlope = Vector3.zero;
            if (IsSlideAloneSlope(player.characterContoller.Value))
            {
                offsetSlope = HandleSlideAloneSlope(direction);
                Logger.DebugFormat("slide alone slope, vec:{0}, offset:{1}", CurVelocity, offsetSlope);

            }
            else
            {
                CalcuSpeedIfNotSlide(player, deltaTime, buff, lastVelocity);
                if (IsHeadingOnPlain(player.characterContoller.Value)) //贴合地面                                          
                {
                    var vefore = CurVelocity;
                    var veoff = offsetSlope;
                    CurVelocity.y = velocityTan * CurVelocity.magnitude - 0.1f; 
                    Logger.DebugFormat("IsHeadingOnPlain, vec:{0}, offset:{1}, prev:{2}, prev offset:{3}, velocityTan:{4}", CurVelocity, offsetSlope,vefore,veoff, velocityTan);
                }
            }

            VelocityOffset = player.orientation.RotationYaw * (VelocityOffset) + offsetSlope;

            if (player.stateInterface.State.GetNextPostureState() == PostureInConfig.Jump &&
                ((int) player.characterContoller.Value.collisionFlags & (int) UnityEngine.CollisionFlags.Sides) != 0)
            {
                JumpSpeedProject(player.characterContoller.Value);
            }
        }

        private float CalcuBuff(PlayerEntity player)
        {
            IsLimitSpeedInWater = LimitSpeedInWater(player);
            var steepConfig = SingletonManager.Get<CharacterStateConfigManager>().SteepConfig;
            var buff = steepConfig.CalcSteepBuff(player.playerMove.Steep) + (IsLimitSpeedInWater ? -0.3f : 0) +
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
                   (depth >= StopSlowDownInWater && player.stateInterface.State.IsSlowDown());
        }

        private bool IsSlideAloneSlope( ICharacterControllerContext controller)
        {
            return groundInfo.IsSlideSlopeGround(controller.slopeLimit) && Vector3.Dot(lastNormal, Vector3.up) > 0.0f &&
                   lastVelocity.y <= 0.0f;
        }

        private Vector3 HandleSlideAloneSlope(Vector3 slopeVec)
        {
            Vector3 offsetSlope;
            CurVelocity = new Vector4(CurVelocity.x, slopeVec.y, CurVelocity.z, 1f);
            offsetSlope = new Vector3(-CurVelocity.x + slopeVec.x, 0f, -CurVelocity.z + slopeVec.z);
            return offsetSlope;
        }

        private void CalcuSpeedIfNotSlide(PlayerEntity player, float deltaTime, float buff, Vector3 lastVel)
        {
            VelocityOffset = player.stateInterface.State.GetSpeedOffset(buff);
            CurVelocity = player.stateInterface.State.GetSpeed(lastVel, deltaTime, buff);
            CurVelocity = player.orientation.RotationYaw * (CurVelocity);
        }

        private bool IsHeadingOnPlain( ICharacterControllerContext controller)
        {
            return CurVelocity.y < 0 &&
                   groundInfo.isOnGround && groundInfo.isValidGround;
        }

        private void JumpSpeedProject(ICharacterControllerContext controller)
        {
            var surfaceNormal = controller.GetLastHitNormal();
            var tmpVec = new Vector3(CurVelocity.x, 0, CurVelocity.z);
            var newVelocity = GetDirectionTangentToSurfaceCustom(tmpVec, surfaceNormal, controller.transform.forward) *
                              tmpVec.magnitude;
            newVelocity = Vector3.ProjectOnPlane(newVelocity, Vector3.up);
            CurVelocity.x = newVelocity.x;
            CurVelocity.z = newVelocity.z;
        }

        public Vector3 GetDirectionTangentToSurfaceCustom(Vector3 direction, Vector3 surfaceNormal,
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