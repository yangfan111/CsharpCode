using System.Collections;
using System.Collections.Generic;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.Configuration;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Player.CharacterState;
using Core.CharacterState;
using Core.CharacterState.Movement;
using Core.Compare;
using Core.Configuration;
using Core.GameModule.Interface;
using Core.HitBox;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;
using Utils.Configuration;
using XmlConfig;
using App.Shared.Player;
using Core.CameraControl;
using Core.CharacterController;
using App.Shared.GameModules.Player.Appearance;
using Core.CharacterController.ConcreteController;
using Utils.Singleton;
using Utils.Utils;

namespace App.Shared.GameModules.Player
{
    public class PlayerMoveSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerMoveSystem));

        private Contexts _contexts;

        private readonly float BeginSlowDownInWater = SingletonManager.Get<CharacterStateConfigManager>().BeginSlowDownInWater;
        private readonly float SteepLimitBegin = Mathf.Tan(Mathf.Deg2Rad * SingletonManager.Get<CharacterStateConfigManager>().SteepLimitBegin);
        private readonly float StopSlowDownInWater = SingletonManager.Get<CharacterStateConfigManager>().StopSlowDownInWater;
        private readonly float SteepLimitStop = Mathf.Tan(Mathf.Deg2Rad * SingletonManager.Get<CharacterStateConfigManager>().SteepLimitStop);
        private readonly float SteepAverRatio = SingletonManager.Get<CharacterStateConfigManager>().SteepAverRatio;
        private readonly float MaxEdgeAngle = 15.0f;

        public PlayerMoveSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = (PlayerEntity)owner.OwnerEntity;
            var updateComponent = player.moveUpdate;
            updateComponent.LastPosition = player.position.Value;

            if (!CanPlayerMove(player))
            {
                updateComponent.NeedUpdate = false;
                return;
            }

            var state = player.stateInterface.State;
            var postureInConfig = state.GetNextPostureState();
            updateComponent.Reset();
            int moveType;
            player.playerMove.IsGround = false;
            player.playerMove.IsCollided = false;

            if (postureInConfig == PostureInConfig.Swim)
            {
                SwimMove(player, cmd.FrameInterval * 0.001f, cmd);
                moveType = (int)MoveType.Water;
            }
            else if (postureInConfig == PostureInConfig.Dive)
            {
                DiveMove(player, cmd.FrameInterval * 0.001f, cmd);
                moveType = (int)MoveType.Water;
            }
            else if (player.playerSkyMove.IsMoveEnabled)
            {
                SkyMove(player, cmd.MoveHorizontal, cmd.MoveVertical, cmd.FrameInterval * 0.001f);
                moveType = (int)MoveType.Sky;
            }
            else
            {
                //_logger.InfoFormat("current seq:{0}-----------------------", cmd.Seq);
                if (SingletonManager.Get<MapConfigManager>().InWater(player.position.Value) && LimitSpeedInWater(player))
                //                  SingletonManager.Get<MapConfigManager>().WaterSurfaceHeight(player.position.Value) - player.position.Value.y >= BeginSlowDownInWater)
                {
                    MoveOnGround(player, cmd.FrameInterval * 0.001f, true);
                }
                else
                {
                    MoveOnGround(player, cmd.FrameInterval * 0.001f, false);
                    //SkyMove(player, cmd.MoveHorizontal, cmd.MoveVertical, cmd.FrameInterval * 0.001f);
                }
                moveType = (int)MoveType.Land;
            }

            SyncPlayerMove(updateComponent, moveType, player);
            player.skyMoveUpdate.IsMoveEnabled = player.playerSkyMove.IsMoveEnabled;
            //_logger.ErrorFormat("{0}: lastPos:{1},curPos:{2}",player.userCmd.Latest.Seq, updateComponent.LastPosition,player.position.Value);
        }

        private static void SyncPlayerMove(MoveUpdateComponent updateComponent, int moveType, PlayerEntity player)
        {
            updateComponent.MoveType = moveType;
            updateComponent.SpeedRatio = player.playerMove.SpeedRatio;
            updateComponent.IsGround = player.playerMove.IsGround;
            updateComponent.IsCollided = player.playerMove.IsCollided;
            updateComponent.MoveInWater = player.stateInterface.State.IsMoveInWater();
            updateComponent.ExceedSteepLimit = player.stateInterface.State.IsSteepSlope();
            updateComponent.Velocity = player.playerMove.Velocity;
            updateComponent.Rotation = player.RootGo().transform.eulerAngles;
            updateComponent.Dist = player.position.Value - updateComponent.LastPosition;
            updateComponent.ModelPitch = player.orientation.ModelPitch;
            updateComponent.ModelYaw = player.orientation.ModelYaw;
            updateComponent.MoveSpeedRatio = player.playerMove.MoveSpeedRatio;
            updateComponent.NeedUpdate = true;
        }

        private void JumpSpeedProject(ref Vector4 velocity, ICharacterControllerContext controller)
        {
            var surfaceNormal = controller.GetLastGroundNormal();
            var tmpVec = new Vector3(velocity.x, 0, velocity.z);
            var newVelocity = GetDirectionTangentToSurfaceCustom(tmpVec, surfaceNormal, controller.transform.forward) * tmpVec.magnitude;
            //_logger.InfoFormat("before:{2}, after transform:{0}, project:{1}", newVelocity.ToStringExt(), Vector3.ProjectOnPlane(newVelocity, Vector3.up), velocity);
            newVelocity = Vector3.ProjectOnPlane(newVelocity, Vector3.up);
//            DebugDraw.EditorDrawArrow(controller.GetLastGroundHitPoint(),
//                new Vector3(velocity.x, 0, velocity.z).normalized, new Color(0.4f, 0.9f, 1f), 3f);
//            DebugDraw.EditorDrawArrow(controller.GetLastGroundHitPoint(),
//                new Vector3(newVelocity.x, 0, newVelocity.z).normalized * 2f, new Color(1f, 0.2f, 1f), 3f);
            //_logger.InfoFormat("prev:{0}, after:{1}, surfaceNormal:{2}", velocity.ToStringExt(),newVelocity.ToStringExt(), surfaceNormal.ToStringExt());

            velocity.x = newVelocity.x;
            velocity.z = newVelocity.z;
        }

        public Vector3 GetDirectionTangentToSurfaceCustom(Vector3 direction, Vector3 surfaceNormal, Vector3 characterFoward)
        {
            Vector3 directionLeft = Vector3.Cross(
                direction,
                Vector3.up);

            if (CompareUtility.IsApproximatelyEqual(directionLeft, Vector3.zero))
            {
                directionLeft  = Vector3.Cross(
                    characterFoward,
                    Vector3.up);
            }
            return Vector3.Cross(surfaceNormal, directionLeft).normalized;
        }
        
        private void MoveOnGround(PlayerEntity player, float deltaTime, bool moveInWater)
        {
            var state = player.stateInterface.State;
            var postureInConfig = state.GetNextPostureState();
            var controller = player.characterContoller.Value;

            //旋转包围盒
            RotBox(player, deltaTime, postureInConfig, controller);

            var playerRoot = player.RootGo();
            var syncTransform = playerRoot.transform;
            var lastVel = Quaternion.Inverse(player.orientation.RotationYaw) * player.playerMove.Velocity.ToVector4();;

            Vector4 velocity = lastVel.ToVector4();
            Vector4 velocityOffset = Vector3.zero.ToVector4();
            float latestCollisionSlope = 0f;
            
            CalculateVelocity(ref velocity, ref velocityOffset, ref latestCollisionSlope, deltaTime, player,
                moveInWater, controller, lastVel);
            
            //坡度计算
            if (state.GetCurrentPostureState() == PostureInConfig.Jump && ((int)controller.collisionFlags & (int)UnityEngine.CollisionFlags.Sides) != 0)
            {
                JumpSpeedProject(ref velocity, controller);
            }
            //_logger.InfoFormat("curVel:{0},VelOffset:{1}", velocity.ToStringExt(), velocityOffset.ToStringExt());

            var scaledVel = (velocity);
            scaledVel.Scale(new Vector4(player.playerMove.MoveSpeedRatio,1, player.playerMove.MoveSpeedRatio,1));
            var dist = (scaledVel + velocityOffset) * deltaTime;
            PlayerMoveUtility.Move(_contexts, player, controller, dist, deltaTime);
            //_logger.InfoFormat("dist:{0}, velocity:{4},velocityOffset:{5} ,flag:{6}, nextCurState:{1}, controllerType:{2}, currentState:{3}", dist.ToStringExt(), postureInConfig, controller.GetCurrentControllerType(),state.GetCurrentPostureState(),velocity, velocityOffset, controller.collisionFlags);

            //落地或碰到天花板，速度降为0
            HandleCollision(ref velocity, player, controller, latestCollisionSlope);
            //_logger.InfoFormat("v.y:{0}, isGround:{1},latestCollisionSlope:{2}", player.playerMove.Velocity.y, player.playerMove.IsGround, latestCollisionSlope); 
            CalcuSlope(player, syncTransform);

            player.stateInterface.State.SetMoveInWater(moveInWater);
            player.playerMove.Velocity = velocity;
            player.position.Value = syncTransform.position;

            player.orientation.ModelPitch = YawPitchUtility.Normalize(syncTransform.rotation.eulerAngles.x);
            player.orientation.ModelYaw = YawPitchUtility.Normalize(syncTransform.rotation.eulerAngles.y);

        }

        private void RotBox(PlayerEntity player, float deltaTime, PostureInConfig postureInConfig,
            ICharacterControllerContext controller)
        {
            //旋转包围盒
            controller.SetCurrentControllerType(postureInConfig);
            controller.SetCharacterPosition(player.position.Value);
            controller.SetCharacterRotation(player.orientation.ModelView);
            controller.Rotate(player.orientation.RotationYaw, deltaTime);
        }

        private void CalcuSlope(PlayerEntity player, Transform syncTransform)
        {
            //坡度计算
            bool exceedSteepLimit = false;
            // 在宏观尺度, Move的距离为实际距离, 在微观尺度, Move的距离为水平距离
            var actualMovement = (syncTransform.position - player.position.Value);
            var horizontalComponent = Mathf.Sqrt(actualMovement.x * actualMovement.x + actualMovement.z * actualMovement.z);

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
                    (steep >= SteepLimitBegin) || ((steep > SteepLimitStop) && player.stateInterface.State.IsSlowDown());
            }

            player.stateInterface.State.SetSteepSlope(exceedSteepLimit);
        }

        private void HandleCollision(ref Vector4 velocity,PlayerEntity player, ICharacterControllerContext controller, 
            float latestCollisionSlope)
        {
            // 碰到了天花板
            if ((controller.collisionFlags & CollisionFlags.Above) != 0)
            {
                player.playerMove.IsCollided = true;
                //只有在上跳的时候，把速度减为0
                if (velocity.y > 0)
                {
                    velocity.y = 0f;
                }
            }

            //collisionFlags会同时返回Above和Below
            if ((controller.collisionFlags & CollisionFlags.Below) != 0
                || (controller.collisionFlags & CollisionFlags.Sides) != 0)
            {
                player.playerMove.IsCollided = true;
                // 一个人往地面上的半球跳跃时，碰撞结果都是CollisionFlags.Sides，因此isGrounded为false
                // 1.如果Y轴分量大于0，则人物继续上升
                // 2.如果人物不能在碰撞点站住，则会加速下滑
				// 3.latestCollisionSlope < controller.slopeLimit条件一直不满足，角色一直下落，但是没有向下移动，可以认为CharacterController认为已经落地了,靠斜面下滑到latestCollisionSlope < controller.slopeLimit来最终落地
                if (velocity.y < 0 && (controller.isGrounded || latestCollisionSlope < controller.slopeLimit))
                {
                    if (latestCollisionSlope < controller.slopeLimit)
                    {
                        velocity.y = 0;
                    }
                    else
                    {
                        velocity.y = Mathf.Min(0f,player.playerMove.Velocity.y);
                    }
                    player.playerMove.IsGround = true;
                }
            }
        }

        private void CalculateVelocity(ref Vector4 velocity, ref Vector4 velocityOffset, ref float latestCollisionSlope,
            float deltaTime, PlayerEntity player, bool moveInWater, ICharacterControllerContext controller,
            Vector3 lastVel)
        {
            //坡度计算
            var lastNormal = controller.GetLastGroundNormal();
            // 最近一次与世界碰撞时碰撞点的法向
            latestCollisionSlope = Vector3.Angle(lastNormal, Vector3.up);

            //Debug.DrawRay(controller.GetLastGroundHitPoint(), lastNormal.normalized * 7,Color.yellow);
            
            Vector3 slopeVec = SlopeSlide(player, velocity, lastNormal, deltaTime);

            //Debug.DrawRay(controller.GetLastGroundHitPoint(), slopeVec.normalized * 7,Color.cyan);
            
            var xzcomp = Mathf.Sqrt(slopeVec.x * slopeVec.x + slopeVec.z * slopeVec.z);

            float collisionSlope = xzcomp == 0 ? 0 : slopeVec.y / xzcomp;


            var offsetSlope = Vector3.zero;
            
            //速度计算与下滑处理
            int ledgeDetect = -1;
            if (latestCollisionSlope >= controller.slopeLimit && Vector3.Dot(lastNormal, Vector3.up) > 0.0f &&
                (controller.collisionFlags & CollisionFlags.Below) != 0  &&
                lastVel.y <= 0.0f && (ledgeDetect = LedgeDetect(controller.GetLastGroundHitPoint(), controller.GetLastGroundNormal(),
                controller.slopeLimit, controller.gameObject)) == 0
                )
            {
                // 沿斜面下滑
                velocity = slopeVec.ToVector4(); //SlopeSlide(player, -velocity.y, script.CollisionNormal, deltaTime);
                player.stateInterface.State.SetExceedSlopeLimit(true);
                //_logger.InfoFormat("slide slope!!!!!!, latestCollisionSlope:{0}", latestCollisionSlope);
            }
            else
            {
                var steepConfig = SingletonManager.Get<CharacterStateConfigManager>().SteepConfig;
                var buff = steepConfig.CalcSteepBuff(player.playerMove.Steep) + (moveInWater ? -0.3f : 0) +
                           player.playerMove.SpeedAffect;
                buff = buff < -1.0f ? -1.0f : buff;
                velocity = player.stateInterface.State.GetSpeed(lastVel, deltaTime, buff);
                velocityOffset = player.stateInterface.State.GetSpeedOffset(buff);
                velocity = player.orientation.RotationYaw * (velocity);

                if (velocity.y < 0 &&
                    (Mathf.Abs(collisionSlope) > Mathf.Tan(MaxEdgeAngle * Mathf.Deg2Rad)) &&
                    (controller.collisionFlags & CollisionFlags.Below) != 0 && (ledgeDetect == -1 ? LedgeDetect(controller.GetLastGroundHitPoint(), controller.GetLastGroundNormal(),
                        controller.slopeLimit, controller.gameObject):ledgeDetect) == 1)
                {
                    offsetSlope = slopeVec;
                    offsetSlope.y = 0;
                    velocity.y = slopeVec.y;
                    //_logger.InfoFormat("can not stand no ledge, ledgeDetect:{0}, collisionSlope:{1}, velocity:{2},offset:{3}, offsetSlope:{4},velocityOffset:{5}",
                     //   ledgeDetect,collisionSlope,
                      //  velocity.ToStringExt(),
                      //  (player.orientation.RotationYaw * (velocityOffset) + offsetSlope).ToStringExt(),
                       // offsetSlope.ToStringExt(),
                       // velocityOffset.ToStringExt());

                }
                
                else if (velocity.y < 0 &&
                    (Mathf.Abs(collisionSlope) <  Mathf.Tan(controller.slopeLimit * Mathf.Deg2Rad)) && //超出限制应正常滑落
                    (controller.collisionFlags & CollisionFlags.Below) != 0
                ) //判断人物未浮空                                                           
                {
                    velocity.y = 0;
                    velocity.y = collisionSlope * velocity.magnitude - 0.1f; //-0.1f 保证下坡时持续产生CollisionFlags.Below  
                    //_logger.InfoFormat("velocity:{0}, collisionSLocp:{1}, {2}, slopeVec:{3}, xzcomp:{4}", velocity.ToStringExt(), collisionSlope,Mathf.Tan(controller.slopeLimit * Mathf.Deg2Rad), slopeVec.ToStringExt() , xzcomp);
                }

                velocityOffset = player.orientation.RotationYaw * (velocityOffset) + offsetSlope;
                player.stateInterface.State.SetExceedSlopeLimit(false);
            }
        }

        public float MaxStepHeight = 0.25f;
        public float SecondaryProbesVertical = 0.02f;
        public float SecondaryProbesHorizontal = 0.001f;
        
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hitPoint"></param>
        /// <param name="hitNormal"></param>
        /// <param name="maxStableSlopeAngle"></param>
        /// <param name="gameObject"></param>
        /// <returns>0 not detect 1 detect</returns>
        private int LedgeDetect(Vector3 hitPoint, Vector3 hitNormal, float maxStableSlopeAngle, GameObject gameObject)
        {
            var prevLayer = gameObject.layer;
            IntersectionDetectTool.SetColliderLayer(gameObject, UnityLayers.TempPlayerLayer);
            var atCharacterUp = Vector3.up;
            var LedgeHandling = true;
            var InnerNormal = hitNormal;
            var OuterNormal = hitNormal;
            int LedgeDetected = 0;
            Vector3 innerHitDirection = Vector3.ProjectOnPlane(hitNormal, atCharacterUp).normalized;
            // Ledge handling
            if (LedgeHandling)
            {
                float ledgeCheckHeight = MaxStepHeight;

                bool isStableLedgeInner = false;
                bool isStableLedgeOuter = false;

                RaycastHit innerLedgeHit;
                if (IntersectionDetectTool.CharacterCollisionsRaycast(
                    hitPoint + (atCharacterUp * SecondaryProbesVertical) + (innerHitDirection * SecondaryProbesHorizontal), 
                    -atCharacterUp,
                    ledgeCheckHeight + SecondaryProbesVertical, 
                    out innerLedgeHit, 
                    IntersectionDetectTool._internalCharacterHits) > 0)
                {
                    InnerNormal = innerLedgeHit.normal;
                    isStableLedgeInner = IsStableOnNormal(innerLedgeHit.normal, maxStableSlopeAngle);
                    //Debug.DrawRay(hitPoint + (atCharacterUp * SecondaryProbesVertical) + (innerHitDirection * SecondaryProbesHorizontal), InnerNormal * 10, Color.magenta, 3f);
                }

                RaycastHit outerLedgeHit;
                if (IntersectionDetectTool.CharacterCollisionsRaycast(
                    hitPoint + (atCharacterUp * SecondaryProbesVertical) + (-innerHitDirection * SecondaryProbesHorizontal), 
                    -atCharacterUp,
                    ledgeCheckHeight + SecondaryProbesVertical, 
                    out outerLedgeHit, 
                    IntersectionDetectTool._internalCharacterHits) > 0)
                {
                    OuterNormal = outerLedgeHit.normal;
                    isStableLedgeOuter = IsStableOnNormal(outerLedgeHit.normal, maxStableSlopeAngle);
//                    Debug.DrawRay(
//                        hitPoint + (atCharacterUp * SecondaryProbesVertical) +
//                        (-innerHitDirection * SecondaryProbesHorizontal), OuterNormal * 10, Color.blue, 3f);
                }
                
                LedgeDetected = (isStableLedgeInner != isStableLedgeOuter) ? 1 : LedgeDetected;
                //if (LedgeDetected == 1)
                //{
                    //_logger.InfoFormat("ledge detected!!!,isStableLedgeInner:{0},isStableLedgeOuter:{1}",isStableLedgeInner,isStableLedgeOuter);
                //}
                //else
                //{
                    //_logger.InfoFormat("stable inner:{0}, outer:{1}", isStableLedgeInner, isStableLedgeOuter);
                //}
                //Debug.DrawRay(hitPoint, hitNormal * 8f, Color.green, 3.0f);
            }

            
            
            IntersectionDetectTool.SetColliderLayer(gameObject, prevLayer);
            
            return LedgeDetected;
        }
        
        private bool IsStableOnNormal(Vector3 normal, float maxStableSlopeAngle)
        {
            return Vector3.Angle(Vector3.up, normal) <= maxStableSlopeAngle;
        }
        
        

        private float CalcSwimXAngle(float vertical, float horizontal, float updown)
        {
            float retVertical = 0.0f;
            // process vertical
            if (vertical < 0)
            {
                retVertical = Mathf.Lerp(SwimCharacterController.SwimBack, 0, vertical + 1);
            }
            else
            {
                retVertical = Mathf.Lerp(0, SwimCharacterController.SwimFowrard, vertical);
            }

            return retVertical;


        }

        private void SwimMove(PlayerEntity player, float deltaTime, IUserCmd cmd)
        {
            var state = player.stateInterface.State;
            var postureInConfig = state.GetNextPostureState();

            var controller = player.characterContoller.Value;

            controller.SetCurrentControllerType(postureInConfig);
            
            controller.SetCharacterPosition(player.position.Value);
            
            var diveXAngle = CalcSwimXAngle(state.VerticalValue, state.HorizontalValue, state.UpDownValue);
            
            controller.SetCharacterRotation(new Vector3(diveXAngle,YawPitchUtility.Normalize(player.orientation.Yaw + player.orientation.PunchYaw), 0 ));
            
            var lastVel = Quaternion.Inverse(player.orientation.RotationYaw) * player.playerMove.Velocity.ToVector4();
            var velocity = player.stateInterface.State.GetSpeed(lastVel, deltaTime);
            velocity = player.orientation.RotationYaw * velocity.ToVector4();
            var velocityOffset = player.stateInterface.State.GetSpeedOffset();
            var dist = (velocity + velocityOffset) * deltaTime;

            if (velocityOffset.magnitude > 0 || (player.stateInterface.State.IsForth && dist.sqrMagnitude > 0 && player.orientation.Pitch >= (SingletonManager.Get<CameraConfigManager>().GetConfigByType(ECameraConfigType.ThirdPerson).PitchLimit.Max - 1)))
            {
                player.stateInterface.State.Dive();
                DiveMove(player, deltaTime, cmd);
                player.moveUpdate.BeginDive = true;
               
            }
            else
            {
                PlayerMoveUtility.Move(_contexts, player, player.characterContoller.Value, dist, deltaTime);

                player.playerMove.Velocity = velocity;
                player.position.Value = controller.transform.position;
            }

        }

        private void DiveMove(PlayerEntity player, float deltaTime, IUserCmd cmd)
        {
            var state = player.stateInterface.State;
            var postureInConfig = state.GetNextPostureState();

            var controller = player.characterContoller.Value;

            controller.SetCurrentControllerType(postureInConfig);

            
            var diveXAngle = CalcDiveXAngle(state.VerticalValue, state.HorizontalValue, state.UpDownValue);

            controller.SetCharacterPosition(player.position.Value);
            //_logger.InfoFormat("my epxected rot:x:{0},y:{1}, new:{2}", diveXAngle, YawPitchUtility.Normalize(player.orientation.Yaw + player.orientation.PunchYaw),YawPitchUtility.Normalize(Quaternion.Euler(diveXAngle,YawPitchUtility.Normalize(player.orientation.Yaw + player.orientation.PunchYaw), 0 )).ToStringExt() );
            controller.SetCharacterRotation(new Vector3(diveXAngle,YawPitchUtility.Normalize(player.orientation.Yaw + player.orientation.PunchYaw), 0));
            
            //_logger.InfoFormat("dive x angle:{0}, seq:{1}", diveXAngle, cmd.Seq);
            
//            _logger.InfoFormat("current posture:{0}, diveXAngle:{1}, inputRotation Yaw:{2}, curPos:{3}, expectPos:{4}", 
//                postureInConfig, 
//                diveXAngle,
//                YawPitchUtility.Normalize(player.orientation.Yaw + player.orientation.PunchYaw),
//                controller.transform.position.ToStringExt(),
//                player.position.Value.ToStringExt()
//                );
            
            var velocity = player.stateInterface.State.GetSpeed(Vector3.zero, deltaTime);
            velocity = player.orientation.RotationView * velocity.ToVector4();
            var velocityOffset = player.stateInterface.State.GetSpeedOffset();

            var dist = (velocity + velocityOffset) * deltaTime;

            PlayerMoveUtility.Move(_contexts, player, player.characterContoller.Value, dist, deltaTime);

            //_logger.InfoFormat("dive move dist:{0}, prev pos:{1}, after pos:{2}", dist.ToStringExt(), player.position.Value.ToStringExt(), controller.transform.position.ToStringExt());
            
            player.playerMove.Velocity = velocity;
            player.position.Value = controller.transform.position;
        }

        private void SkyMove(PlayerEntity player, float moveHorizontal, float moveVertical, float deltaTime)
        {
            var stage = player.playerSkyMove.MoveStage;
            if(!SharedConfig.IsServer)
            {
                PlayerSkyMoveStateMachine.GetStates()[(int)stage].Move(player, moveVertical, moveHorizontal, deltaTime);
            }
            else
            {
                PlayerSkyMoveStateMachine.GetStates()[(int)stage].ServerMove(player, moveVertical, moveHorizontal, deltaTime);
            }
            player.skyMoveUpdate.SkyMoveStage = player.playerSkyMove.MoveStage;
            player.skyMoveUpdate.SkyPosition = player.playerSkyMove.Position;
            player.skyMoveUpdate.SkyRotation = player.playerSkyMove.Rotation;
            player.skyMoveUpdate.SkyLocalPlayerPosition = player.playerSkyMove.LocalPlayerPosition;
            player.skyMoveUpdate.SkyLocalPlayerRotation = player.playerSkyMove.LocalPlayerRotation;
            player.skyMoveUpdate.Pitch = player.orientation.Pitch;
            player.skyMoveUpdate.Yaw = player.orientation.Yaw;
            player.skyMoveUpdate.Roll = player.orientation.Roll;
            player.skyMoveUpdate.GameState = player.gamePlay.GameState;
//            player.skyMoveUpdate.IsMoveEnabled = player.playerSkyMove.IsMoveEnabled;
        }

        private Vector4 GetVect4(Vector3 vec)
        {
            return new Vector4(vec.x, vec.y, vec.z, 1);
        }

        private float CalcDiveXAngle(float vertical, float horizontal, float updown)
        {
            float retVertical = 0.0f;
            float retUpDown = 0.0f;
            // process vertical
            if (vertical < 0)
            {
                retVertical = Mathf.Lerp(DiveCharacterController.DiveBack, 0, vertical + 1);
            }
            else
            {
                retVertical = Mathf.Lerp(0, DiveCharacterController.DiveFowrard, vertical);
            }

            if (updown < 0)
            {
                retUpDown = Mathf.Lerp(DiveCharacterController.DiveDown, 0, updown + 1);
            }
            else
            {
                retUpDown = Mathf.Lerp(0, DiveCharacterController.DiveUp, updown);
            }

            float absVertical = Mathf.Abs(vertical);
            float absUpdown = Mathf.Abs(updown);
            float total = absVertical + absUpdown;
            if (CompareUtility.IsApproximatelyEqual(total, 0))
            {
                return retVertical;
            }

            return retVertical * absVertical / total + retUpDown * absUpdown / total;


        }
        
        private Vector3 SlopeSlide(PlayerEntity player, Vector3 prevSpeed, Vector3 collisionNormal, float deltaTime)
        {
            if (CompareUtility.IsApproximatelyEqual(collisionNormal.normalized, Vector3.up))
            {
                return Vector3.zero;
            }
            prevSpeed.y = prevSpeed.y - SpeedManager.Gravity * deltaTime;

            var slopeDir = new Vector3();
            slopeDir.y = -Mathf.Sqrt(1 - collisionNormal.y * collisionNormal.y);

            var x = Mathf.Abs(collisionNormal.x);
            var z = Mathf.Abs(collisionNormal.z);
            var xzComponent = Mathf.Sqrt((1 - slopeDir.y * slopeDir.y) * (x + z) * (x + z) / (x * x + z * z));

            slopeDir.x = xzComponent * collisionNormal.x / (x + z);
            slopeDir.z = xzComponent * collisionNormal.z / (x + z);

            //slopeDir *= Mathf.Abs(newSpeed / slopeDir.y);
            slopeDir = slopeDir.normalized * prevSpeed.magnitude;
            return slopeDir;
        }

//        public static bool MoveByAnim(PlayerEntity player)
//        {
//            return player.hasThirdPersonAnimator && player.thirdPersonAnimator.UnityAnimator.applyRootMotion;
//        }

        /// <summary>
        /// 角色是否可以移动和旋转
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static bool CanPlayerMove(PlayerEntity player)
        {
            if ((player.hasCharacterContoller && !player.characterContoller.Value.enabled) ||
                (player.hasThirdPersonAnimator && player.thirdPersonAnimator.UnityAnimator.applyRootMotion))
            {
                return false;
            }

            if (player.IsOnVehicle() || PlayerStateUtil.HasPlayerState(EPlayerGameState.NotMove, player.gamePlay))
            {
                return false;
            }

            if (player.gamePlay.IsLifeState(EPlayerLifeState.Dead))
            {
                return false;
            }

            return true;
        }

        public bool LimitSpeedInWater(PlayerEntity player)
        {
            float depth = SingletonManager.Get<MapConfigManager>().WaterSurfaceHeight(player.position.Value) -
                          player.position.Value.y;

            return (depth >= BeginSlowDownInWater)
                   ||
                   (depth >= StopSlowDownInWater && player.stateInterface.State.IsSlowDown());
        }
    }
}