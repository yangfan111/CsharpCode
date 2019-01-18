using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components;
using App.Shared.Components.Player;
using App.Shared.Configuration;
using App.Shared.Player;
using Core.CameraControl;
using Core.Configuration;
using Core.HitBox;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using log4net.Repository.Hierarchy;
using UnityEngine;
using Utils.Appearance;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;
using Object = System.Object;

namespace App.Shared.GameModules.Player
{
    public enum SkyMoveStage
    {
        Start = 0,
        Gliding,
        Parachuting,
        Landing,
        Count
    }

    public interface IPlayerSkyMoveState
    {
        void Update(Contexts contexts, PlayerEntity player, IUserCmd cmd);
        void ServerUpdate(Contexts contexts, PlayerEntity player, IUserCmd cmd);
        void Move(PlayerEntity player, float moveVertical, float moveHorizontal, float deltaTime);
        void ServerMove(PlayerEntity player, float moveVertical, float moveHorizontal, float deltaTime);
    }

    public abstract class PlayerSkyMoveState : IPlayerSkyMoveState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSkyMoveState));


        public void Update(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            if(ValidateState(contexts, player))
                UpdateState(contexts, player, cmd);
        }

        public void ServerUpdate(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            if(ServerValidateState(contexts, player))
                ServerUpdateState(contexts, player, cmd);
        }

        public abstract void Move(PlayerEntity player, float moveVertical, float moveHorizontal, float deltaTime);
        public abstract void ServerMove(PlayerEntity player, float moveVertical, float moveHorizontal, float deltaTime);

        //process client rewind
        protected abstract bool ValidateState(Contexts contexts, PlayerEntity player);
        protected abstract void UpdateState(Contexts contexts, PlayerEntity player, IUserCmd cmd);
        
        protected abstract bool ServerValidateState(Contexts contexts, PlayerEntity player);
        protected abstract void ServerUpdateState(Contexts contexts, PlayerEntity player, IUserCmd cmd);

        protected void OpenParachute(Contexts contexts, PlayerEntity player)
        {
            var playerSkyMove = player.playerSkyMove;

            _logger.DebugFormat("OpenParachute-----------------------------------------------------------------,{0}," +
                                " IsWaitForAttach:{1},IsParachuteAttached:{2} ",playerSkyMove.IsReadyForAttachParachute(),playerSkyMove.IsWaitForAttach, playerSkyMove.IsParachuteAttached);

            if (playerSkyMove.IsReadyForAttachParachute())
            {
                player.stateInterface.State.Parachuting(() =>
                {
                    _logger.DebugFormat("Parachuting call " +
                                        "back-----------------------------------------------------------------");
                    PlayerSkyMoveUtility.AttachParachute(contexts, player, false);
                });
               // player.soundManager.Value.PlayOnce(EPlayerSoundType.OpenParachute);
                playerSkyMove.IsWaitForAttach = true;
            }
            else if (!playerSkyMove.IsParachuteLoading && playerSkyMove.Parachute == null)
            {
                PlayerSkyMoveUtility.DelayLoadParachute(player, contexts);
                playerSkyMove.IsParachuteLoading = true;
            }
        }

        public static bool IsSkyMoveEnabled(PlayerEntity player)
        {
            if (SharedConfig.IsOffline)
            {
                var playerSkyMove = player.playerSkyMove;
                bool isEnabled = playerSkyMove.IsMoveEnabled;
                if (isEnabled)
                {
                    if (!playerSkyMove.IsMoving)
                    {
                        var position = player.position.Value;
                        isEnabled = IsAboveGround(player, position, 2000.0f);
                    }

                    return isEnabled;
                }

                return false;
            }

            return player.gamePlay.GameState == GameState.AirPlane || player.gamePlay.GameState == GameState.Gliding || player.gamePlay.GameState == GameState.JumpPlane;
        }

        public static void ValidateStateAfterUpdate(Contexts contexts, PlayerEntity player)
        {
            if (!player.playerSkyMove.IsMoveEnabled && player.playerSkyMove.IsParachuteAttached)
            {
                PlayerSkyMoveUtility.DetachParachute(contexts, player);
            }
        }

        protected static bool IsAboveGround(PlayerEntity player, Vector3 position, float height)
        {
          
            Vector3 p1, p2;
            float radius;
            PlayerEntityUtility.GetCapsule(player, position, out p1, out p2, out radius);

            var end = position - Vector3.up * height;
            if (SingletonManager.Get<MapConfigManager>().InWater(end))
            {
                return false;
            }

            return !Physics.CapsuleCast(p1, p2, radius, -Vector3.up, height, UnityLayers.SceneCollidableLayerMask);
        }
    }


    public class PlayerSkyStartState : PlayerSkyMoveState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSkyDiveMoveState));

        public override void Move(PlayerEntity player, float moveVertical, float moveHorizontal, float deltaTime)
        {

        }

        public override void ServerMove(PlayerEntity player, float moveVertical, float moveHorizontal, float deltaTime)
        {
            
        }

        protected override bool ValidateState(Contexts contexts, PlayerEntity player)
        {
            if (player.playerSkyMove.GameState != -1)
            {
                player.gamePlay.GameState = player.playerSkyMove.GameState;
            }
            
            if (player.playerSkyMove.IsParachuteAttached)
            {
                _logger.Warn("Sky Start State is not valid.");
                PlayerSkyMoveUtility.DetachParachute(contexts, player);
//                player.playerSkyMove.MoveStage = (int) SkyMoveStage.Parachuting;
//                return false;
            }
            return true;
        }

        protected override void UpdateState(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {

            if (!player.playerSkyMove.IsMoving)
            {
                if (SharedConfig.IsOffline)
                {
					if(cmd.IsPDown)
                    	JumpToSky(player);
                }
                else if (player.gamePlay.GameState == GameState.JumpPlane || player.gamePlay.GameState == GameState.Gliding)
                {
                    JumpToSky(player);
                }
            }
        }

        protected override bool ServerValidateState(Contexts contexts, PlayerEntity player)
        {
            return ValidateState(contexts, player);
        }

        protected override void ServerUpdateState(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            if (!player.playerSkyMove.IsMoving)
            {
                if (SharedConfig.IsOffline)
                {
                    if(cmd.IsPDown)
                        ServerJumpToSky(player);
                }
                else if (player.gamePlay.GameState == GameState.JumpPlane)
                {
                    ServerJumpToSky(player);
                }
            }
        }

        private void ServerJumpToSky(PlayerEntity player)
        {
            _logger.InfoFormat("SKyDive: Start To Gliding State");
            //player.gamePlay.GameState = GameState.Gliding;
        }

        private void JumpToSky(PlayerEntity player)
        {
            _logger.InfoFormat("SKyDive: Start To Gliding State");
            player.stateInterface.State.Gliding();
            player.stateInterface.State.Freefall();
            player.playerSkyMove.IsMoving = true;
            player.playerSkyMove.MoveStage = (int)SkyMoveStage.Gliding;
            player.playerSkyMove.GameState = GameState.Gliding;
            player.gamePlay.GameState = GameState.Gliding;
        }
    }

    public abstract class PlayerSkyDiveMoveState : PlayerSkyMoveState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSkyDiveMoveState));

        protected float YawSpeed;
        private float PitchSpeed;
        private float RollSpeed;
        private float RollBackSpeed;
        protected float RollVelocityDamper;
        protected float MaxRollAngle;
        protected float IdlePitchAngle;
        protected float MaxPitchUpAngle;
        protected float MaxPitchDownAngle;
        private float Gravity;
        protected float GravityDamper;
        protected float HorizontalDamper;
        protected float AirResistance;
        protected float AirKeyInputResistance;
        private float Acceleration;
        protected float MaxGravityVelocity;
        protected float MaxPitchUpVerticalVelocity;
        protected float MaxPitchDownVerticalVelocity;

        protected float MaxSwingVelocity;
        protected float SwingAcceleration;
        protected float SwingDeacceleration;
        protected float SwingAirResistance;

        protected bool IsInitialized = false;

        protected void Init(CharacterSkyMoveConfig moveConfig)
        {
            IsInitialized = true;

            PitchSpeed = moveConfig.SkyPitchSpeed;
            RollSpeed = moveConfig.SkyRollSpeed;
            RollBackSpeed = moveConfig.SkyRollBackSpeed;

            Gravity = moveConfig.SkyGravity;
            Acceleration = moveConfig.SkyAcceleration;
        }

        protected Vector3 SkyMove(Vector3 originAngles, PlayerEntity player, bool isGliding, Vector3 forwardMove, Vector3 rightMove, float deltaTime,
           out Vector3 newAngles)
        {
            // _logger.ErrorFormat("pos {0} rot {1} velocity {2} ", syncTransform.position.ToString("f10"),
            //     syncTransform.eulerAngles.ToString("f10"), playerMove.Velocity.ToString("f10"));
            //calc player rotationa
            var playerSkyMove = player.playerSkyMove;
            newAngles = CalcSkyMovePlayerRotation(ref originAngles, isGliding,
                forwardMove, rightMove, deltaTime);

            //_logger.ErrorFormat("Angles is {0}, original angles is {1}", angles.ToString("f10"), originAngles.ToString("f10"));
            //calc Vertical velocity
            var playerMove = player.playerMove;
            var originVerticalVelocity = new Vector2(playerSkyMove.ExtraVerticalVelocity, playerMove.Velocity.y);
            var verticalVelocity = CalcSkyMoveVerticalVelocity(originVerticalVelocity, newAngles, forwardMove, deltaTime);
            playerSkyMove.ExtraVerticalVelocity = verticalVelocity.x;

            //            //_logger.ErrorFormat("verticalVelocity is {0}", verticalVelocity.ToString("f10"));
            //            //calc Horizontal velocity
            //            syncTransform.localEulerAngles = angles;
            float newSwingVelocity;
            var horizontalVelocity = CalcSkyMoveHorizontalVelocity(isGliding, originAngles, newAngles,
                playerMove.Velocity, playerSkyMove.SwingVelocity, verticalVelocity.y,
                 forwardMove, rightMove, deltaTime, out newSwingVelocity);
            playerSkyMove.SwingVelocity = newSwingVelocity;

            // _logger.ErrorFormat("horizontalVelocity is {0}", verticalVelocity.ToString("f10"));
            //calc compound velocity
            var compoundVelocity = horizontalVelocity;
            compoundVelocity.y = verticalVelocity.y;
            return compoundVelocity;

            //            Debug.LogForma`t("angle from {0} to {1} , velocity H {2}, V {3}, D {4}",
            //                originAngles.ToString("f8"),
            //                angles.ToString("f8"),
            //                Mathf.Sqrt(compoundVelocity.x * compoundVelocity.x + compoundVelocity.z * compoundVelocity.z),
            //                compoundVelocity.y,
            //                compoundVelocity.ToString("f8"));
        }

        private Vector3 CalcSkyMovePlayerRotation(ref Vector3 originAngles, bool isGliding,
            Vector3 forwardMove, Vector3 rightMove, float deltaTime)
        {
            originAngles.x = YawPitchUtility.Normalize(originAngles.x);
            originAngles.y = YawPitchUtility.Normalize(originAngles.y);
            originAngles.z = YawPitchUtility.Normalize(originAngles.z);

            var rotation = Quaternion.Euler(originAngles);

            //roll move (around z)
            var right = rotation.Right();
            var rollFrac = Vector3.Dot(right, rightMove);
            var targetRoll = -rollFrac * MaxRollAngle;
            var rspeed = targetRoll * originAngles.z < 0 || Mathf.Abs(targetRoll) < Mathf.Abs(originAngles.z) ? RollBackSpeed : RollSpeed;
            var roll = Mathf.MoveTowards(originAngles.z, targetRoll, rspeed * deltaTime);
            

            //pitch move (around x)
            var forward = rotation.Forward();
            var pitchFrac = Vector3.Dot(forward, forwardMove);
            var pitchUpAngle = MaxPitchUpAngle;
            var pitchDownAngle = MaxPitchDownAngle;

            float targetPitch = 0;
            if (pitchFrac <= 0)
            {
                targetPitch = pitchFrac * pitchUpAngle;
            }
            else
            {
                if (isGliding)
                {
                    var forwardMoveDir = forwardMove.normalized;
                    targetPitch = Mathf.Acos(Vector3.Dot(-Vector3.up, forwardMoveDir)) * Mathf.Rad2Deg;
                    if (targetPitch + IdlePitchAngle > 90)
                    {
                        targetPitch = pitchFrac * pitchDownAngle;
                    }
                    else
                    {
                        targetPitch = 90 - targetPitch - IdlePitchAngle;
                        targetPitch = Mathf.Clamp(targetPitch, 0, MaxPitchDownAngle);
                    }
                }
                else
                {
                    targetPitch = pitchFrac * pitchDownAngle;
                }
                
            }
            
            targetPitch += IdlePitchAngle;
           // Debug.LogFormat("targetpitch is {0} {1}", targetPitch, originAngles.x);

            var ptichSpeedFrac = (originAngles.x <= IdlePitchAngle ? Mathf.Abs(pitchUpAngle) : Mathf.Abs(pitchDownAngle)) /
                             (Mathf.Abs(pitchUpAngle) + Mathf.Abs(pitchDownAngle));
            var pitch = Mathf.MoveTowards(originAngles.x, targetPitch, ptichSpeedFrac * PitchSpeed * deltaTime);

            //yaw move (around y)
            var yaw = originAngles.y - YawSpeed * roll / MaxRollAngle;

            return new Vector3(pitch, yaw, roll);
        }


        private Vector2 CalcSkyMoveVerticalVelocity(Vector2 originVelocity, Vector3 targetEulerAngles, Vector3 forwardMove, float deltaTime)
        {
            var gravity = -Gravity;
            //calc gravity velocity
            var currentExtraVerticalVelocity = originVelocity.x;

            var gravityVelocity = originVelocity.y - originVelocity.x;
            var velGravitySqr = gravityVelocity * gravityVelocity * 0.001f; 
            gravityVelocity += Mathf.Min(-0.1f, gravity + velGravitySqr * AirResistance) * deltaTime;
            if (GravityDamper > 0)
            {
                if (gravityVelocity < -MaxGravityVelocity)
                {
                    gravityVelocity = Mathf.MoveTowards(gravityVelocity, -MaxGravityVelocity, GravityDamper * deltaTime);
                }
                else if (gravityVelocity > 0.0f)
                {
                    gravityVelocity = 0.0f;
                }
            }
            else
            {
                gravityVelocity = Mathf.Clamp(gravityVelocity, -MaxGravityVelocity, 0.0f);
            }


            //calc extra vertical controll velocity accoring to rotation
            var targetExtraVerticalVelocity =
               CalcSkyMoveExtraVerticalVelocity(targetEulerAngles.x);
            var velSqrFactor = originVelocity.y * originVelocity.y * 0.001f;
            var acceleration = Acceleration;
            if (targetExtraVerticalVelocity <= currentExtraVerticalVelocity)
            {
                acceleration = Mathf.Max(0.1f, acceleration - AirKeyInputResistance * velSqrFactor);
            }
            else
            {
                acceleration = acceleration * 0.5f + AirKeyInputResistance * velSqrFactor;
            }

            var extraVerticalVelocity = Mathf.MoveTowards(currentExtraVerticalVelocity, targetExtraVerticalVelocity,
                acceleration * deltaTime);
            extraVerticalVelocity = extraVerticalVelocity > -gravityVelocity * 0.5f ? -gravityVelocity * 0.5f : extraVerticalVelocity;

            var verticalVelocity = gravityVelocity + extraVerticalVelocity;
            if (forwardMove.sqrMagnitude.Equals(0) && 
                extraVerticalVelocity > currentExtraVerticalVelocity &&
                verticalVelocity > -MaxGravityVelocity)
            {
                extraVerticalVelocity = 0.0f;
            }

            return new Vector2(extraVerticalVelocity, verticalVelocity);
        }
  

        private Vector3 CalcSkyMoveHorizontalVelocity(bool isGliding, Vector3 eulerAngles, Vector3 targetAngles, 
            Vector3 currentVelocity, float currentSwingVelocity, float targetVerticalVelocity, 
            Vector3 forwardMove, Vector3 rightMove,
            float deltaTime, out float newSwingVelocity)
        {
            
            var targetHorizontalForwardVelocity = 0.0f;
            if (targetAngles.x > IdlePitchAngle)
            {
                //target pitch is clamp to [0, 45]
                var targetPitch = Mathf.Abs(targetAngles.x);
                targetPitch = targetPitch > 180 ? targetPitch - 180 : targetPitch;
                targetPitch = targetPitch > 45 && targetPitch < 135 ? Mathf.Abs(targetPitch - 90) : 45;
                targetHorizontalForwardVelocity =
                    Mathf.Abs(targetVerticalVelocity * Mathf.Max(Mathf.Tan(targetPitch * Mathf.Deg2Rad),
                                  Mathf.Tan(IdlePitchAngle * Mathf.Deg2Rad)));
            }
            else
            {
                targetHorizontalForwardVelocity = Mathf.Abs(targetVerticalVelocity * Mathf.Tan(IdlePitchAngle * Mathf.Deg2Rad));
            }

            //Debug.LogWarningFormat("Target Angle x {0} VVel {1} HVel {2}", targetAngles.x, targetVerticalVelocity, targetHorizontalForwardVelocity);

            var targetHorizontalUpVehlocity = 0.0f;
            if (RollVelocityDamper > 0)
            {
                targetHorizontalUpVehlocity = Mathf.Abs(targetVerticalVelocity * Mathf.Tan(targetAngles.z * Mathf.Deg2Rad * RollVelocityDamper));
            }

            var rotation = Quaternion.Euler(eulerAngles);
            var forward = rotation.Forward();
            forward.y = 0;
            forward = forward.normalized;
            var up = Vector3.zero;
            if (RollVelocityDamper > 0)
            {
                up = rotation.Up();
                up.y = 0;
                up -= Vector3.Dot(forward, up) * forward;
                up = up.normalized;
            }

            var horizontalDamper = rightMove.sqrMagnitude > 0 ? HorizontalDamper : 1.0f;
            var targetHorizontalVelocity = (up * targetHorizontalUpVehlocity + forward * targetHorizontalForwardVelocity) * horizontalDamper;
            //Debug.LogFormat("Horizontal Damper {0}", horizontalDamper);

            currentVelocity.y = 0;
            if (!currentSwingVelocity.Equals(0))
            {
                currentVelocity -= currentVelocity.normalized * currentSwingVelocity;
            }
            var horizontalVelocity = Mathf.MoveTowards(currentVelocity.magnitude, targetHorizontalVelocity.magnitude,
                Acceleration * deltaTime);

            var maxPitchAngle = MaxPitchDownAngle;
            maxPitchAngle += IdlePitchAngle;
            maxPitchAngle = maxPitchAngle > 45 ? 45 : maxPitchAngle;
            var maxHorizontalVelocity = Mathf.Abs(targetVerticalVelocity * Mathf.Tan(maxPitchAngle * Mathf.Deg2Rad));
            maxHorizontalVelocity = Mathf.Min(maxHorizontalVelocity, Mathf.Abs(targetVerticalVelocity));
            if (horizontalVelocity > maxHorizontalVelocity)
            {
                horizontalVelocity = maxHorizontalVelocity;
            }

            //            Debug.LogFormat("F {0}, U {1} Final {2}, {3}, {4} Forward {5} currentVelocity {6}", targetHorizontalForwardVelocity, targetHorizontalUpVehlocity, horizontalVelocity, targetHorizontalVelocity.ToString("f10"),
            //                maxPitchAngle, forward.ToString("f10"), currentVelocity.ToString("f10"));

            var velocity = horizontalVelocity * targetHorizontalVelocity.normalized;
            newSwingVelocity = 0.0f;
            if (!isGliding)
            {
                newSwingVelocity = CalcSwingVelocity(eulerAngles, currentSwingVelocity, forwardMove, deltaTime);

                if (!newSwingVelocity.Equals(0))
                {
                    var swingDireciton = forward;
                    swingDireciton.y = 0.0f;
                    swingDireciton = forward.normalized;
                    velocity += swingDireciton * newSwingVelocity;
                }
            }

            return velocity;
        }

        private float CalcSkyMoveExtraVerticalVelocity(float pitch)
        {
            var extraVerticalVelocity = pitch <= IdlePitchAngle
                ? Mathf.Abs((pitch - IdlePitchAngle) / MaxPitchUpAngle) * MaxPitchUpVerticalVelocity
                : -Mathf.Abs((pitch - IdlePitchAngle) / MaxPitchDownAngle) * MaxPitchDownVerticalVelocity;

            return Mathf.Clamp(extraVerticalVelocity, -MaxPitchDownVerticalVelocity, MaxPitchUpVerticalVelocity);
        }

        private float CalcSwingVelocity(Vector3 originAngles, float originalSwingVelocity, Vector3 forwardMove, float deltaTime)
        {
            var rotation = Quaternion.Euler(originAngles);
            var forward = rotation.Forward();
            var updot = Vector3.Dot(forward, Vector3.up);
            var accelDamper = Vector3.Dot(forward, forwardMove);

            var acceleration = 0.0f;
            if (updot < 0 && accelDamper < 0)
            {
                //swing forward 
                acceleration = SwingAcceleration * Mathf.Abs(updot) * 100.0f;
            }
            else if (updot > 0 && accelDamper > 0) 
            {
                //swing backward
                acceleration = -SwingDeacceleration * Mathf.Abs(updot) * 100.0f;
            }

            var swingVelocity = originalSwingVelocity +  acceleration * deltaTime;
            swingVelocity *= Mathf.Max(0.0f, 1 - SwingAirResistance * deltaTime);
            swingVelocity = Mathf.Clamp(swingVelocity, -MaxSwingVelocity, MaxSwingVelocity);

            //Debug.LogFormat("Swing Velocity is {0}, forward {1}, acceleration {2} updot {3}, accelDamper {4}, deltaTime {5}", 
             //   swingVelocity, forward, acceleration, updot, accelDamper, deltaTime);

            return swingVelocity;

        }
    }

    public class PlayerSkyGlidingState : PlayerSkyDiveMoveState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSkyGlidingState));

        public override void Move(PlayerEntity player, float moveVertical, float moveHorizontal, float deltaTime)
        {
//            if (player.playerSkyMove.IsWaitForAttach)
//            {
//                return;
//            }

            Init();

            SyncPoseFromComponent(player);

            var syncTransform = player.RootGo().transform;
            var originAngles = player.playerSkyMove.Rotation.eulerAngles;
            originAngles.y = syncTransform.eulerAngles.y;
            var mainCameraRotation = Quaternion.Euler(player.cameraFinalOutputNew.EulerAngle);
          
            var forwardMove = mainCameraRotation.Forward() * moveVertical;
            var rightMove = mainCameraRotation.Right() * moveHorizontal;
            Vector3 newAngles;
            var velocity = SkyMove(originAngles, player, true, forwardMove, rightMove, deltaTime, out newAngles);
            
            PlayerGlidingMove(player, syncTransform, newAngles, velocity, deltaTime);

            SyncPoseToComponent(player);
        }

        public override void ServerMove(PlayerEntity player, float moveVertical, float moveHorizontal, float deltaTime)
        {
            Move(player, moveVertical, moveHorizontal, deltaTime);
        }

        protected void Init()
        {
            if (IsInitialized)
            {
                return;
            }

            var moveConfig = SingletonManager.Get<CharacterStateConfigManager>().SkyMoveConfig;

            Init(moveConfig);

            YawSpeed = 0;
           
            RollVelocityDamper = moveConfig.GlidinRollVelocityDamper;
            MaxRollAngle = moveConfig.MaxGlidingRollAngle;

            IdlePitchAngle = 0;
            MaxPitchUpAngle = moveConfig.MaxGlidingPitchUpAngle;
            MaxPitchDownAngle = 90;

            AirResistance = moveConfig.GlidingAirResistance;
            AirKeyInputResistance = moveConfig.GlidingAirKeyInputResistance;

            GravityDamper = 0;
            HorizontalDamper = 1.0f;
            MaxGravityVelocity = moveConfig.MaxGlidingGravityVelocity;
            MaxPitchUpVerticalVelocity = moveConfig.MaxGlidingPitchUpVerticalVelocity;
            MaxPitchDownVerticalVelocity = moveConfig.MaxGlidingPitchDownVerticalVelocity;
        }



        private void SyncPoseFromComponent(PlayerEntity player)
        {
            var syncTransform = player.RootGo().transform;
            syncTransform.position = player.position.Value;

            syncTransform.eulerAngles = new Vector3(0, player.orientation.Yaw, 0);
        }

        private void SyncPoseToComponent(PlayerEntity player)
        {
            var syncTransform = player.RootGo().transform;

            player.position.Value = syncTransform.position;
            var angles = syncTransform.eulerAngles;
            player.orientation.Yaw = YawPitchUtility.Normalize(angles.y);
        }

        private void PlayerGlidingMove(PlayerEntity player, Transform syncTransform, Vector3 newAngles, Vector3 velocity, float deltaTime)
        {
            var playerMove = player.playerMove;
            playerMove.Velocity = velocity;
            syncTransform.position += playerMove.Velocity* deltaTime;
            player.playerSkyMove.Rotation = Quaternion.Euler(newAngles);
        }

        protected override bool ValidateState(Contexts contexts, PlayerEntity player)
        {
            player.gamePlay.GameState = player.playerSkyMove.GameState;
            
            if (player.playerSkyMove.IsParachuteAttached)
            {
                _logger.Warn("Sky Gliding State is not valid.");
                PlayerSkyMoveUtility.DetachParachute(contexts, player);
//                player.playerSkyMove.MoveStage = (int) SkyMoveStage.Parachuting;
//                return false;
            }

            return true;
        }

        protected override void UpdateState(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            player.playerSkyMove.ParachuteTime += cmd.FrameInterval * 0.001f;

            var moveConfig = SingletonManager.Get<CharacterStateConfigManager>().SkyMoveConfig;
            if (cmd.IsUseAction && player.playerSkyMove.ParachuteTime > moveConfig.ParachuteTime)
            {
                if (SharedConfig.IsOffline)
                {
                    OpenParachute(contexts, player);
                }
                else if (player.gamePlay.GameState == GameState.Gliding)
                {
                    OpenParachute(contexts, player);
                }
            }

            if (player.playerSkyMove.IsReadyForAttachParachute() ||
                !IsAboveGround(player, player.position.Value, moveConfig.MinParachuteHeight))
            {
                OpenParachute(contexts, player);
            }
        }

        protected override bool ServerValidateState(Contexts contexts, PlayerEntity player)
        {
            return ValidateState(contexts, player);
        }

        protected override void ServerUpdateState(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            player.playerSkyMove.ParachuteTime += cmd.FrameInterval * 0.001f;
            var moveConfig = SingletonManager.Get<CharacterStateConfigManager>().SkyMoveConfig;
            if (cmd.IsUseAction && player.playerSkyMove.ParachuteTime > moveConfig.ParachuteTime)
            {
                if (SharedConfig.IsOffline)
                {
                    ServerOpenParachute(contexts, player);
                }
                else if (player.gamePlay.GameState == GameState.Gliding)
                {
                    ServerOpenParachute(contexts, player);
                }
            }

            if (player.playerSkyMove.IsReadyForAttachParachute() ||
                !IsAboveGround(player, player.position.Value, moveConfig.MinParachuteHeight))
            {
                ServerOpenParachute(contexts, player);
            }
        }

        private void ServerOpenParachute(Contexts contexts, PlayerEntity player)
        {
            var playerSkyMove = player.playerSkyMove;

            _logger.DebugFormat("server " +
                                "OpenParachute-----------------------------------------------------------------,{0}," +
                                " IsWaitForAttach:{1},IsParachuteAttached:{2} ",playerSkyMove.IsReadyForAttachParachute(),playerSkyMove.IsWaitForAttach, playerSkyMove.IsParachuteAttached);

            if (playerSkyMove.IsReadyForAttachParachute())
            {
                player.stateInterface.State.Parachuting(() =>
                {
                    _logger.DebugFormat("server Parachuting call " +
                                        "back-----------------------------------------------------------------");
                    PlayerSkyMoveUtility.AttachParachute(contexts, player, false);
                });
            }
            else if (!playerSkyMove.IsParachuteLoading && playerSkyMove.Parachute == null)
            {
                PlayerSkyMoveUtility.DelayLoadParachute(player, contexts);
            }
        }
    }

    public class PlayerSkyParachutingState : PlayerSkyDiveMoveState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSkyParachutingState));

        private const float StepTime = 0.016f;

        public override void Move(PlayerEntity player, float moveVertical, float moveHorizontal, float deltaTime)
        {
            //            if (player.stateInterface.State.GetActionState() == ActionInConfig.ParachutingOpen)
            //            {
            //                return;
            //            }
            Init();

            var remainDeltaTime = player.playerSkyMove.RemainDeltaTime  + deltaTime;
            while (remainDeltaTime > StepTime)
            {
                SyncPoseFromComponent(player);
                // _logger.ErrorFormat("start pos {0} rot {1}, orot {2}", syncTransform.position.ToString("f10"), syncTransform.eulerAngles.ToString("f10"), eulerAngles.ToString("f10"));
                //  Debug.LogFormat("PlayerPosition is {0}", player.characterGameObject.PlayerRoot.transform.position.ToString("f11"));

                var syncTransform = player.playerSkyMove.Parachute;

                Vector3 newAngles;
                var forwardMove = syncTransform.forward * moveVertical;
                var rightMove = syncTransform.right * moveHorizontal;
                var velocity = SkyMove(syncTransform.eulerAngles, player, false, forwardMove, rightMove, StepTime, out newAngles);

                PlayerParachutingMove(player, syncTransform, newAngles, velocity, StepTime);

                SyncPoseToComponent(player);

                remainDeltaTime -= StepTime;
            }

            player.playerSkyMove.RemainDeltaTime = remainDeltaTime;
        }

        public override void ServerMove(PlayerEntity player, float moveVertical, float moveHorizontal, float deltaTime)
        {
            Move(player, moveVertical, moveHorizontal, deltaTime);
        }

        protected void Init()
        {
            if (IsInitialized)
            {
                return;
            }

            var moveConfig = SingletonManager.Get<CharacterStateConfigManager>().SkyMoveConfig;

            Init(moveConfig);

            YawSpeed = moveConfig.SkyYawSpeed;

            RollVelocityDamper = 0;
            MaxRollAngle = moveConfig.MaxParachuteRollAngle;

            IdlePitchAngle = moveConfig.ParachuteIdlePitchAngle;
            MaxPitchUpAngle = moveConfig.MaxParachutePitchUpAngle;
            MaxPitchDownAngle = moveConfig.MaxParachutePitchDownAngle;

            AirResistance = 0.0f;
            AirKeyInputResistance = 0.0f;

            GravityDamper = moveConfig.ParachuteGravityDamper;
            HorizontalDamper = moveConfig.ParachuteHorizontalDamper;

            MaxGravityVelocity = moveConfig.MaxParachuteGravityVelocity;
            MaxPitchUpVerticalVelocity = moveConfig.MaxParachutePitchUpVerticalVelocity;
            MaxPitchDownVerticalVelocity = moveConfig.MaxParachutePitchDownVerticalVelocity;

            MaxSwingVelocity = moveConfig.MaxParachuteSwingVelocity;
            SwingAcceleration = moveConfig.ParachuteSwingAcceleration;
            SwingDeacceleration = moveConfig.ParachuteSwingDeacceleration;
            SwingAirResistance = moveConfig.ParachuteSwingAirResistance;
        }


        private void PlayerParachutingMove(PlayerEntity player, Transform syncTransform, Vector3 newAngles, Vector3 velocity, float deltaTime)
        {
            syncTransform.eulerAngles = newAngles;

            var controller = player.characterContoller.Value;
            var radius = controller.radius;
            var midHeight = controller.height - 2 * radius;
            var p1 = controller.transform.position + controller.center - Vector3.up * 0.5f * midHeight;
            var p2 = p1 + Vector3.up * midHeight;

            bool hitted = false;
            //player move
            var dist = velocity * deltaTime;
            var direction = dist.normalized;
            var distance = dist.magnitude;
            distance = distance > 0.01f ? distance : 0;
            var layermask = UnityLayers.SceneCollidableLayerMask;
            if (distance > 0)
            {
                RaycastHit hitInfo;
                var playerDist = distance;
                var playerHitted = Physics.CapsuleCast(p1, p2, radius, direction, out hitInfo, distance, layermask);
                if (playerHitted)
                {
                    playerDist = hitInfo.distance;
                }

                var parachute = player.playerSkyMove.Parachute;
                var rb = parachute.GetComponent<Rigidbody>();
                var parachuteDist = distance;
                bool parachuteHitted = false;
                if (rb != null)
                {
                    parachuteHitted = rb.SweepTest(direction, out hitInfo, distance);
                    if (parachuteHitted)
                    {
                        parachuteDist = hitInfo.distance;
                    }
                }

                hitted = playerHitted || parachuteHitted;
                distance = Mathf.Min(playerDist, parachuteDist);
            }


            if (distance <= 0.01f)
            {
                player.playerSkyMove.MotionLessTime += deltaTime;
            } 

            if (hitted)
            {
                // _logger.ErrorFormat("hitted!!!");
                velocity = Vector3.zero;
            }
            //_logger.InfoFormat("direction is {0} dist {1}, deltaTime is {2},vel is {3}", direction.ToString("f10"), distance, deltaTime, velocity.ToString("f10"));
            syncTransform.position += direction * distance;
            player.playerSkyMove.Velocity = velocity;
        }

        protected override bool ValidateState(Contexts contexts, PlayerEntity player)
        {
            player.gamePlay.GameState = player.playerSkyMove.GameState;
            
            if (!player.playerSkyMove.IsParachuteAttached)
            {
                _logger.Warn("Sky Parachuting State is not valid.");
                PlayerSkyMoveUtility.AttachParachute(contexts, player, true);
                if (!player.playerSkyMove.IsParachuteAttached)
                {
                    if (CheckLandCondition(player))
                    {
                        _logger.InfoFormat("SKyDive Rollback: Parachuting To Landing State");
                        player.playerSkyMove.MoveStage = (int)SkyMoveStage.Landing;
                    }
                    else
                    {
                        _logger.InfoFormat("SKyDive Rollback: Parachuting To Gliding State");
                         player.playerSkyMove.MoveStage = (int)SkyMoveStage.Gliding;
                    }

                    return false;
                }
            }

            return true;
        }

        protected override void UpdateState(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            CheckAndLand(contexts, player);
        }

        protected override bool ServerValidateState(Contexts contexts, PlayerEntity player)
        {
            return ValidateState(contexts, player);
        }

        protected override void ServerUpdateState(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            if (CheckLandCondition(player))
            {
                CommonSkyMoveLand(contexts, player);
            }
        }

        private void CheckAndLand(Contexts contexts, PlayerEntity player)
        {
            if (CheckLandCondition(player))
            {
                SkyMoveLand(contexts, player);
            }
        }

        private bool CheckLandCondition(PlayerEntity player)
        {
            var position = player.RootGo().transform.position;
            return !IsAboveGround(player, position, 5.0f) ||
                   player.playerSkyMove.MotionLessTime > 3.0f;
        }

        private void CommonSkyMoveLand(Contexts contexts, PlayerEntity player)
        {
            _logger.DebugFormat("SKyDive: Detach Parachute To Landing State");

            var playerSkyMove = player.playerSkyMove;
            player.stateInterface.State.ParachutingEnd();
            PlayerSkyMoveUtility.DetachParachute(contexts, player);
        }
        
        private void SkyMoveLand(Contexts contexts, PlayerEntity player)
        {
            CommonSkyMoveLand(contexts, player);
            _logger.InfoFormat("SkyDive: Move To Landing State.");
            player.playerSkyMove.MoveStage = (int)SkyMoveStage.Landing;
        }

        private void SyncPoseFromComponent(PlayerEntity player)
        {
            var syncTransform = player.playerSkyMove.Parachute.transform;
            syncTransform.position = player.playerSkyMove.Position;
            syncTransform.rotation = player.playerSkyMove.Rotation;
            player.playerMove.Velocity = player.playerSkyMove.Velocity;
            var playerSyncTransform = player.RootGo().transform;
            player.position.Value = playerSyncTransform.position;
        }

        private void SyncPoseToComponent(PlayerEntity player)
        {
            var playerSyncTransform = player.RootGo().transform;
            player.position.Value = playerSyncTransform.position;
            //_logger.DebugFormat("parachute moveing:{0}", player.position.Value);
            var angles = playerSyncTransform.eulerAngles;
            player.orientation.Pitch = YawPitchUtility.Normalize(angles.x);
            player.orientation.Yaw = YawPitchUtility.Normalize(angles.y);
            player.orientation.Roll = YawPitchUtility.Normalize(angles.z);

            var syncTransform = player.playerSkyMove.Parachute.transform;
            player.playerSkyMove.Position = syncTransform.position;
            player.playerSkyMove.Rotation = syncTransform.rotation;
        }
    }

    public class PlayerSkyLandingState : PlayerSkyMoveState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerSkyLandingState));

        public override void Move(PlayerEntity player, float moveVertical, float moveHorizontal, float deltaTime)
        {
            var velocity = player.playerMove.Velocity;
            var dist = velocity.y * deltaTime;
            var position = player.RootGo().transform.position;
            if (IsAboveGround(player, position, Mathf.Abs(dist)))
            {
                SyncPoseFromComponent(player);

                PlayerLandingMove(player, deltaTime);

                SyncPoseToComponent(player);
            }
            else
            {
                EndSkyMove(player);
            }
        }

        public override void ServerMove(PlayerEntity player, float moveVertical, float moveHorizontal, float deltaTime)
        {
            Move(player, moveVertical, moveHorizontal, deltaTime);
        }

        protected virtual void SyncPoseFromComponent(PlayerEntity player)
        {
            var syncTransform = player.RootGo().transform;
            syncTransform.position = player.position.Value;
            syncTransform.eulerAngles = new Vector3(player.orientation.Pitch, player.orientation.Yaw, player.orientation.Roll);
        }

        protected virtual void SyncPoseToComponent(PlayerEntity player)
        {
            var syncTransform = player.RootGo().transform;

            player.position.Value = syncTransform.position;
            var angles = syncTransform.eulerAngles;
            player.orientation.Pitch = YawPitchUtility.Normalize(angles.x);
            player.orientation.Yaw = YawPitchUtility.Normalize(angles.y);
            player.orientation.Roll = YawPitchUtility.Normalize(angles.z);

        }

        protected override void UpdateState(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            var velocity = player.playerMove.Velocity;
            velocity.x = velocity.z = 0.0f;
            if (velocity.y >= -1.0f)
            {
                EndSkyMove(player);
            }
        }

        protected override bool ServerValidateState(Contexts contexts, PlayerEntity player)
        {
            return ValidateState(contexts, player);
        }

        protected override void ServerUpdateState(Contexts contexts, PlayerEntity player, IUserCmd cmd)
        {
            var velocity = player.playerMove.Velocity;
            velocity.x = velocity.z = 0.0f;
            if (velocity.y >= -1.0f)
            {
                //player.gamePlay.GameState = GameState.Normal;
            }
        }

        protected override bool ValidateState(Contexts contexts, PlayerEntity player)
        {
            player.gamePlay.GameState = player.playerSkyMove.GameState;
            
            if (player.playerSkyMove.IsParachuteAttached)
            {
                _logger.Warn("Sky Landing State is not valid.");
                PlayerSkyMoveUtility.DetachParachute(contexts, player);
//                player.playerSkyMove.MoveStage = (int)SkyMoveStage.Parachuting;
//                return false;
            }

            return true;
        }

        private void EndSkyMove(PlayerEntity player)
        {
            _logger.DebugFormat("end sky move----------------------------------------");
            if (!SharedConfig.IsServer)
            {
                player.playerSkyMove.IsMoving = false;
                player.playerSkyMove.IsMoveEnabled = false;
                player.playerSkyMove.MotionLessTime = 0;
                player.playerSkyMove.MoveStage = (int) SkyMoveStage.Start;
                player.playerSkyMove.GameState = GameState.Normal;
                
                _logger.DebugFormat("set stage to 0!!!------------------------");
            }
            player.gamePlay.GameState = GameState.Normal;
            _logger.DebugFormat("set normal to 0----------------------------------------");
        }

        private void PlayerLandingMove(PlayerEntity player, float deltaTime)
        {
            var velocity = player.playerMove.Velocity;
            velocity.x = velocity.z = 0.0f;

            var moveConfig = SingletonManager.Get<CharacterStateConfigManager>().SkyMoveConfig;
            var syncTransform = player.RootGo().transform;
            syncTransform.position += deltaTime  * velocity.y * Vector3.up;
            velocity.y = Mathf.MoveTowards(velocity.y, 0.0f, moveConfig.SkyLandDeacceleration * deltaTime);
            player.playerMove.Velocity = velocity;
        }
    }


    public class PlayerSkyMoveStateMachine
    {
        private static IPlayerSkyMoveState[] _states = null;

        public static IPlayerSkyMoveState[] GetStates()
        {
            if (_states == null)
            {
                _states = new IPlayerSkyMoveState[(int) SkyMoveStage.Count];
                _states[(int)SkyMoveStage.Start] = new PlayerSkyStartState();
                _states[(int)SkyMoveStage.Gliding] = new PlayerSkyGlidingState();
                _states[(int) SkyMoveStage.Parachuting] = new PlayerSkyParachutingState();
                _states[(int)SkyMoveStage.Landing] = new PlayerSkyLandingState();
            }

            return _states;
        }
    }
}
