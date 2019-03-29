using System;
using System.Collections.Generic;
using App.Shared.Components.Player;
using App.Shared.Configuration;
using App.Shared.GameModules.Player.Actions;
using App.Shared.GameModules.Player.Animation;
using Core.Animation;
using Core.Appearance;
using Core.CharacterState;
using Core.Fsm;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using App.Shared.GameModules.Player.Appearance;
using App.Shared.GameModules.Player.Appearance.AnimationEvent;
using App.Shared.Player;
using Core.Common;
using Utils.Appearance;
using Utils.CharacterState;
using Core.WeaponAnimation;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;
using XmlConfig;
using App.Shared.GameModules.Weapon;
using Core.AnimatorClip;
using Core.CharacterController;
using Core.CharacterState.Posture;
using ECM.Components;
using Utils.Compare;

namespace App.Shared.GameModules.Player.CharacterState
{
    public class PlayerStateUpdateSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerStateUpdateSystem));

        private Contexts _contexts;

        private readonly FsmOutputBaseSystem _fsmOutputs = new FsmOutputBaseSystem();
        private readonly FsmInputCreator _inputCreator = new FsmInputCreator();
        private readonly AnimationMonitor _animMonitor = new AnimationMonitor();
        private readonly AnimatorPoseReplayer _poseReplayer = new AnimatorPoseReplayer();
        private readonly WeaponAnimationController _weaponAnim = new WeaponAnimationController();

        public PlayerStateUpdateSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity playerEntity = (PlayerEntity)owner.OwnerEntity;
            CheckPlayerLifeState(playerEntity);

            if (cmd.PredicatedOnce)
            {
                return;
            }

            RewindNetworkAnimator(cmd, playerEntity);
            var stateManager = playerEntity.stateInterface.State;
            var animatorClipManager = playerEntity.animatorClip.ClipManager;
            playerEntity.stateInterVar.Reset();
            _inputCreator.Reset();


            // cmd到FsmInput
            _inputCreator.CreateCommands(cmd, new FilterState {Posture = stateManager.GetCurrentPostureState()},
                playerEntity, _contexts);
            PostureInterruptAction(playerEntity, cmd);

            var commandsContainer = _inputCreator.CommandsContainer;

            // 记录状态更新前动画状态,以第三人称为准
            // 人物的移动在状态更新之后
            _animMonitor.MonitorBeforeFsmUpdate(commandsContainer,
                playerEntity.thirdPersonAnimator.UnityAnimator,
                playerEntity.playerMove.IsGround);
            //_logger.InfoFormat("land:{2}:IsGround:{0},IsExceedSlopeLimit:{1}", playerEntity.playerMove.IsGround,
            //     playerEntity.stateInterface.State.IsExceedSlopeLimit(),playerEntity.playerMove.IsGround && !playerEntity.stateInterface.State.IsExceedSlopeLimit() );

            // AnimationMonitor会产生Freefall并对Freefall进行处理，所以要在AnimationMonitor.MonitorBeforeFsmUpdate的后面
            AnimationTest(playerEntity, commandsContainer);

            UpdateStateResponseToInput(cmd, stateManager, commandsContainer, playerEntity);

            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.Animator);
            // 播放动画
            if (!playerEntity.gamePlay.IsLifeState(EPlayerLifeState.Dead))
            {
                playerEntity.thirdPersonAnimator.UnityAnimator.Update(cmd.FrameInterval * 0.001f);
                playerEntity.firstPersonAnimator.UnityAnimator.Update(cmd.FrameInterval * 0.001f);
            }
            SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.Animator);

            // 记录状态更新后动画状态，生成FsmInput
            _animMonitor.MonitorAfterFsmUpdate(commandsContainer, playerEntity.thirdPersonAnimator.UnityAnimator,
                playerEntity.firstPersonAnimator.UnityAnimator);


            UpdateWeaponAnimation(commandsContainer, playerEntity);

            UpdateStateResponseToAnimation(stateManager, commandsContainer, animatorClipManager, playerEntity);

            if (_fsmOutputs.NeedUpdateP1())
            {
                playerEntity.firstPersonAnimator.UnityAnimator.Update(0f);
            }

            if (_fsmOutputs.NeedUpdateP3())
            {
                playerEntity.thirdPersonAnimator.UnityAnimator.Update(0f);
                //_logger.InfoFormat("update p3!!!!!!!!");
            }

            WriteNetworkAnimation(cmd, playerEntity);

            SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.StateCallBackInvoke);
            stateManager.TryAnimationBasedCallBack(commandsContainer);
            SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.StateCallBackInvoke);

            CollectAnimationCallBack(stateManager, playerEntity);

            //_logger.InfoFormat("seq:{0},{1}",cmd.Seq, playerEntity.stateInterVar.PrintCommandsCount());

            //_logger.InfoFormat("net work component hash:{0},{3}, SnapshotId:{1}, seq:{2}", playerEntity.networkAnimator.GetHashCode(), cmd.SnapshotId, cmd.Seq, playerEntity.networkAnimator.ToString());
            //_logger.InfoFormat("state component hash:{0}, SnapshotId:{1}, seq:{2}", playerEntity.state, cmd.SnapshotId, cmd.Seq);
        }

        private void WriteNetworkAnimation(IUserCmd cmd, PlayerEntity playerEntity)
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.StateWriteAnimation);
                // 更新动画播放进度，按需写入数据
                AnimatorChange(NetworkAnimatorUtil.GetAnimatorLayers(playerEntity.firstPersonAnimator.UnityAnimator,
                    playerEntity.fpAnimStatus.AnimatorLayers,
                    _fsmOutputs.AnimatorP1ChangedTrigger()), playerEntity.fpAnimStatus, cmd);

                AnimatorChange(NetworkAnimatorUtil.GetAnimatorLayers(playerEntity.thirdPersonAnimator.UnityAnimator,
                    playerEntity.networkAnimator.AnimatorLayers,
                    _fsmOutputs.AnimatorP3ChangedTrigger()), playerEntity.networkAnimator, cmd);
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.StateWriteAnimation);
            }
        }

        private void UpdateStateResponseToAnimation(ICharacterState stateManager,
            IAdaptiveContainer<IFsmInputCommand> commandsContainer,
            AnimatorClipManager animatorClipManager, PlayerEntity playerEntity)
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.StateUpdateResponseToAnimation);

                _fsmOutputs.ResetOutput();
                // 更新状态机
                stateManager.Update(commandsContainer, 0, _fsmOutputs.AddOutput, FsmUpdateType.ResponseToAnimation);
                // 更新Clip速率
                animatorClipManager.Update(_fsmOutputs.AddOutput,
                    playerEntity.thirdPersonAnimator.UnityAnimator,
                    playerEntity.firstPersonAnimator.UnityAnimator,
                    playerEntity.WeaponController().HeldWeaponAgent.ConfigId);

                // 更新Animator的Param
                _fsmOutputs.SetOutput(playerEntity);
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.StateUpdateResponseToAnimation);
            }
        }

        private void UpdateWeaponAnimation(IAdaptiveContainer<IFsmInputCommand> commandsContainer,
            PlayerEntity playerEntity)
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.StateUpdateWeaponAnimation);
                // 更新武器动画
                _weaponAnim.FromAvatarAnimToWeaponAnimProgress(commandsContainer,
                    playerEntity.appearanceInterface.Appearance.GetWeaponP1InHand(),
                    playerEntity.appearanceInterface.Appearance.GetWeaponP3InHand(),
                    playerEntity.networkWeaponAnimation);
                _weaponAnim.FromWeaponAnimProgressToWeaponAnim(
                    playerEntity.appearanceInterface.Appearance.GetWeaponP1InHand(),
                    playerEntity.appearanceInterface.Appearance.GetWeaponP3InHand(),
                    playerEntity.networkWeaponAnimation);
                // 武器动画结束
                _weaponAnim.WeaponAnimFinishedUpdate(commandsContainer,
                    playerEntity.appearanceInterface.Appearance.GetWeaponP1InHand(),
                    playerEntity.appearanceInterface.Appearance.GetWeaponP3InHand(),
                    playerEntity.networkWeaponAnimation);
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.StateUpdateWeaponAnimation);
            }
        }

        private void UpdateStateResponseToInput(IUserCmd cmd, ICharacterState stateManager,
            IAdaptiveContainer<IFsmInputCommand> commandsContainer, PlayerEntity playerEntity)
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.StateUpdateResponseToInput);
                _fsmOutputs.ResetOutput();
                // 更新状态机
                stateManager.Update(commandsContainer, cmd.FrameInterval, _fsmOutputs.AddOutput,
                    FsmUpdateType.ResponseToInput);
                // 更新手臂动画
                playerEntity.characterBoneInterface.CharacterBone.SetWeaponPitch(_fsmOutputs.AddOutput,
                    playerEntity.characterBone.WeaponPitch);
                // 更新一、三人称Animator
                _fsmOutputs.SetOutput(playerEntity);

                if (!SharedConfig.IsServer &&
                    playerEntity.thirdPersonAnimator.UnityAnimator.cullingMode != AnimatorCullingMode.AlwaysAnimate)
                    _logger.WarnFormat("wrong culling mode: {0}",
                        playerEntity.thirdPersonAnimator.UnityAnimator.cullingMode);
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.StateUpdateResponseToInput);
            }
        }


        private void RewindNetworkAnimator(IUserCmd cmd, PlayerEntity playerEntity)
        {
            // 预测回滚
            if (playerEntity.fpAnimStatus.NeedRewind)
            {
                _logger.WarnFormat("rewind fpAnimStatus:{0}!!!", playerEntity.entityKey.Value);

                _poseReplayer.ReplayPose(playerEntity.fpAnimStatus.AnimatorLayers,
                    playerEntity.fpAnimStatus.AnimatorParameters,
                    playerEntity.firstPersonAnimator.UnityAnimator);

                // if rewind for history not saved, should not update
                if (playerEntity.fpAnimStatus.BaseClientTime != 0)
                    playerEntity.firstPersonAnimator.UnityAnimator.Update(
                        (cmd.ClientTime - playerEntity.networkAnimator.BaseClientTime - cmd.FrameInterval) * 0.001f);
            }

            if (playerEntity.networkAnimator.NeedRewind)
            {
                _logger.WarnFormat("rewind networkAnimator:{0}!!!", playerEntity.entityKey.Value);

                _poseReplayer.ReplayPose(playerEntity.networkAnimator.AnimatorLayers,
                    playerEntity.networkAnimator.AnimatorParameters,
                    playerEntity.thirdPersonAnimator.UnityAnimator);

                // if rewind for history not saved, should not update
                if (playerEntity.networkAnimator.BaseClientTime != 0)
                {
                    playerEntity.thirdPersonAnimator.UnityAnimator.Update(
                        (cmd.ClientTime - playerEntity.networkAnimator.BaseClientTime - cmd.FrameInterval) * 0.001f);
                    //_logger.InfoFormat("rewind for history not saved, should not update, seq:{0}, SnapshotId:{1}", cmd.Seq, cmd.SnapshotId);
                }
            }
        }

        private static void CollectAnimationCallBack(ICharacterState stateManager, PlayerEntity playerEntity)
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.StateUpdateEventCollect);
                stateManager.CollectAnimationCallback((animationCommandType, fsmType) =>
                {
                    playerEntity.stateInterVar.AnimationCallbackCommands.Commands.Add(
                        new KeyValuePair<short, float>(animationCommandType, fsmType));
                });
                stateManager.ClearAnimationCallback();

                //动画回调
                var firstPersonEvent = playerEntity.firstPersonModel.Value.GetComponent<AnimationClipEvent>();
                if (firstPersonEvent != null)
                {
                    foreach (KeyValuePair<short, AnimationEventParam> keyValuePair in firstPersonEvent.EventParams)
                    {
                        playerEntity.stateInterVar.FirstPersonAnimationEventCallBack.Commands.Add(
                            new KeyValuePair<short, AnimationEventParam>(keyValuePair.Key, keyValuePair.Value));
                    }

                    firstPersonEvent.EventParams.Clear();
                }

                var thirdPersonEvent = playerEntity.thirdPersonModel.Value.GetComponent<AnimationClipEvent>();
                if (thirdPersonEvent != null)
                {
                    foreach (KeyValuePair<short, AnimationEventParam> keyValuePair in thirdPersonEvent.EventParams)
                    {
                        playerEntity.stateInterVar.ThirdPersonAnimationEventCallBack.Commands.Add(
                            new KeyValuePair<short, AnimationEventParam>(keyValuePair.Key, keyValuePair.Value));
                    }

                    thirdPersonEvent.EventParams.Clear();
                }
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.StateUpdateEventCollect);
            }
        }

        private void AnimationTest(PlayerEntity playerEntity, IAdaptiveContainer<IFsmInputCommand> commandsContainer)
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.StateUpdateTest);
                SprintDisableTest(playerEntity.stateInterface.State, commandsContainer);
                StandCrouchDisableTest(playerEntity, commandsContainer);
                ProneDisableTest(playerEntity, commandsContainer);
                FreeFallTest(playerEntity, commandsContainer);
                WaterPostureDownDisableTest(playerEntity, commandsContainer);
                JumpDisableTest(playerEntity, commandsContainer);
                LandJumpDisableTest(playerEntity, commandsContainer);
                MoveJumpTest(playerEntity, commandsContainer);
                SlideTest(playerEntity, commandsContainer);
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.StateUpdateTest);
            }
        }

        private void SlideTest(PlayerEntity playerEntity, IAdaptiveContainer<IFsmInputCommand> commandsContainer)
        {
            var characterState = playerEntity.stateInterface.State;
            
            var state = characterState.GetNextPostureState();
            if (state != PostureInConfig.Slide)
            {
                if (characterState.IsSlide())
                {
                    characterState.Slide();
                }
            }
            else
            {
                if (!characterState.IsSlide() && playerEntity.playerMove.IsGround)
                {
                    characterState.SlideEnd();
                }
            }
        }


        private void MoveJumpTest(PlayerEntity playerEntity, IAdaptiveContainer<IFsmInputCommand> commandsContainer)
        {
            var state = playerEntity.stateInterface.State.GetNextPostureState();
            var moveState = playerEntity.stateInterface.State.GetNextMovementState();
            if (!((state == PostureInConfig.Land || state == PostureInConfig.Stand) && moveState == MovementInConfig.Sprint))
            {
                return;
            }

            IFsmInputCommand jumpCommand = null;

            bool forthExist = false;
            bool leftExist = false;
            bool rightExist = false;
            
            for (int i = 0; i < commandsContainer.Length; i++)
            {
                var v = commandsContainer[i];
                if (v.Type == FsmInput.Jump)
                {
                    jumpCommand = v;
                }
                else if (v.Type == FsmInput.Forth)
                {
                    forthExist = true;
                }
                else if (v.Type == FsmInput.Left)
                {
                    leftExist = true;
                }
                else if (v.Type == FsmInput.Right)
                {
                    rightExist = true;
                }
            }

            if (jumpCommand != null && forthExist  && CheckJumpSpeed(playerEntity))
            {
                jumpCommand.AdditioanlValue = AnimatorParametersHash.Instance.JumpStateMove;

                if (leftExist)
                {
                    jumpCommand.AlternativeAdditionalValue = AnimatorParametersHash.Instance.MoveJumpStateLF;
                }
                else if (rightExist)
                {
                    jumpCommand.AlternativeAdditionalValue = AnimatorParametersHash.Instance.MoveJumpStateRF;
                }
                else
                {
                    jumpCommand.AlternativeAdditionalValue = AnimatorParametersHash.Instance.MoveJumpStateNormal;
                }
            }
        }

        private static readonly float MinMoveJumpSpeed = 2f;
        
        private bool CheckJumpSpeed(PlayerEntity playerEntity)
        {
            return playerEntity.playerMove.MoveVel >= MinMoveJumpSpeed;
        }

        private void LandJumpDisableTest(PlayerEntity playerEntity,
            IAdaptiveContainer<IFsmInputCommand> commandsContainer)
        {
            var state = playerEntity.stateInterface.State.GetNextPostureState();
            if (!((state == PostureInConfig.Land || state == PostureInConfig.Stand) &&
                  playerEntity.stateInterface.State.IsExceedSlopeLimit()))
            {
                return;
            }

            testCommand.Clear();

            for (int i = 0; i < commandsContainer.Length; i++)
            {
                var v = commandsContainer[i];
                if (v.Type == FsmInput.Jump)
                {
                    testCommand.Add(v);
                }
            }

            if (testCommand.Count == 0)
            {
                return;
            }

            foreach (IFsmInputCommand command in testCommand)
            {
                _logger.InfoFormat(
                    "chang command:{0} to none, because current state:{1} can not jump because the land is IsExceedSlopeLimit!",
                    command.Type,
                    state);
                command.Type = FsmInput.None;
            }

            testCommand.Clear();
        }

        private void JumpDisableTest(PlayerEntity playerEntity, IAdaptiveContainer<IFsmInputCommand> commandsContainer)
        {
            var state = playerEntity.stateInterface.State.GetNextPostureState();
            if (state != PostureInConfig.Stand || playerEntity.playerMove.IsGround)
            {
                return;
            }

            testCommand.Clear();

            for (int i = 0; i < commandsContainer.Length; i++)
            {
                var v = commandsContainer[i];
                if (v.Type == FsmInput.Jump)
                {
                    testCommand.Add(v);
                }
            }

            if (testCommand.Count == 0)
            {
                return;
            }

            var isHit = IsHitGround(playerEntity, ProbeDist);

            if (!isHit)
            {
                foreach (IFsmInputCommand command in testCommand)
                {
                    _logger.InfoFormat(
                        "chang command:{0} to none, because current state:{1} can not jump because ground is empty!",
                        command.Type,
                        state);
                    command.Type = FsmInput.None;
                }

                //Debug.DrawLine(outHit.point, outHit.normal, Color.red, 5000.0f);
            }

            testCommand.Clear();
        }

        private static bool IsHitGround(PlayerEntity playerEntity, float dist)
        {
            var gameObject = playerEntity.RootGo();
            var prevLayer = gameObject.layer;
            IntersectionDetectTool.SetColliderLayer(gameObject, UnityLayerManager.GetLayerIndex(EUnityLayerName.User));
            var playerPosition = gameObject.transform.position;
            var playerRotation = gameObject.transform.rotation;
            var controller = playerEntity.characterContoller.Value;
            var valueRadius = playerEntity.characterContoller.Value.radius;
            var isHit = false;
            RaycastHit outHit;

            if (UseSphereCast(controller))
            {
                //use sphere cast
                var position = playerPosition + playerRotation * new Vector3(0f, valueRadius, 0f);

//            DebugDraw.DebugWireSphere(startPoint, Color.red, CastRadius, 1f);
//            DebugDraw.DebugWireSphere(startPoint + new Vector3(0,targetHeight - CastRadius - LiftUp,0), Color.magenta, CastRadius, 1f);
                isHit = PhysicsCastHelper.SphereCast(position, CastRadius, Vector3.down, out outHit, dist,
                    UnityLayers.AllCollidableLayerMask, QueryTriggerInteraction.Ignore, 0.1f);
                if (!isHit)
                {
                    //DebugDraw.DebugWireSphere(position + (isHit ? Vector3.down * outHit.distance : Vector3.down * dist),isHit ? Color.green : (playerEntity.characterContoller.Value.isGrounded ? Color.magenta : Color.red) ,CastRadius, isHit ? 0f:60f);
                }
            }
            else
            {
                Vector3 point1, point2;
                float radius;
                var height = controller.height;
                radius = controller.radius;
                var center = controller.center;
                PhysicsCastHelper.GetCapsule(controller.transform.position, controller.transform.rotation, height, radius, center, controller.direction, out point1, out point2);
                isHit = PhysicsCastHelper.CapsuleCast(point1, point2, radius,Vector3.down, out outHit, dist,
                    UnityLayers.AllCollidableLayerMask, QueryTriggerInteraction.Ignore, 0.1f);
                if (!isHit && false)
                {
                    PhysicsCastHelper.GetDebugDrawTypeCapsule(controller.transform.position, controller.transform.rotation, height, radius, center, controller.direction, out point1, out point2);
                    DebugDraw.DebugCapsule(point1 + (isHit ? Vector3.down * outHit.distance : Vector3.down * dist), 
                        point2 + (isHit ? Vector3.down * outHit.distance : Vector3.down * dist),
                        isHit ? Color.green : (playerEntity.characterContoller.Value.isGrounded ? Color.magenta : Color.red),
                        radius,
                        isHit ? 0f:60f
                        );
                }
            }
            
            
            IntersectionDetectTool.SetColliderLayer(gameObject, prevLayer);
            return isHit;
        }

        private static bool UseSphereCast(ICharacterControllerContext controller)
        {
            return CompareUtility.IsApproximatelyEqual(controller.direction, Vector3.up);
        }

        private void AnimatorChange(bool isChange, AbstractNetworkAnimator networkAnimator, IUserCmd cmd)
        {
            if (isChange)
            {
                networkAnimator.NeedChangeServerTime = true;
                networkAnimator.BaseServerTime = -1;
                networkAnimator.BaseClientTime = cmd.ClientTime;
                //_logger.InfoFormat("baseServerTime change to -1!!!,p3Change:{1} ,seq:{0}, new change:{2}", cmd.Seq, p3Change,_fsmOutputs.AnimatorP3ChangedTrigger());
            }
            else
            {
                networkAnimator.NeedChangeServerTime = false;
            }
        }

        private void FreeFallTest(PlayerEntity player, IAdaptiveContainer<IFsmInputCommand> commands)
        {
            var freeFallTest = (!player.playerMove.IsGround &&
                                !(player.stateInterface.State.GetActionState() == ActionInConfig.Gliding ||
                                  player.stateInterface.State.GetActionState() == ActionInConfig.Parachuting)) &&
                               player.playerMove.Velocity.y < -SpeedManager.Gravity;
            if (freeFallTest || StandToFreefallTest(player))
            {
                var item = commands.GetAvailableItem(command => { return command.Type == FsmInput.None; });
                item.Type = FsmInput.Freefall;
                _logger.DebugFormat("freefall test true, set FsmInput.None to Fsminput.Freefall, freeFallTest:{0}, state cur posture:{1}, next pos:{2},state movement:{3}", freeFallTest,
                    player.stateInterface.State.GetCurrentPostureState(),
                    player.stateInterface.State.GetNextPostureState(),
                    player.stateInterface.State.GetCurrentMovementState());
                return;
            }
			ClimbUpCollisionTest.ClimbStateFreeFallTest(player, commands);
        }

        private static bool StandToFreefallTest(PlayerEntity player)
        {
//            return player.characterContoller.Value.collisionFlags == CollisionFlags.None &&
//                    !IsHitGround(player, ProbeDist);
            bool isHitGround = IsHitGround(player, ProbeDist);
            bool isGrounded = player.characterContoller.Value.isGrounded;
            var ret = !isGrounded &&
                      !isHitGround && player.stateInterface.State.GetCurrentPostureState() != PostureInConfig.Jump;
            {
                //_logger.DebugFormat("isGrounded:{0}, isHitGround:{1}, ret:{2}",isGrounded, isHitGround, ret);
            }
            return ret;
        }

        /// <summary>
        /// 站立到蹲下低于水面高度，不能下蹲
        /// 蹲下到趴下低于水面搞对，不能趴下
        /// </summary>
        /// <param name="player"></param>
        /// <param name="commands"></param>
        private void WaterPostureDownDisableTest(PlayerEntity player, IAdaptiveContainer<IFsmInputCommand> commands)
        {
            if (SingletonManager.Get<MapConfigManager>().InWater(player.position.Value))
            {
                var inWaterDepth = SingletonManager.Get<MapConfigManager>().WaterSurfaceHeight(player.position.Value) -
                                   player.position.Value.y;

                bool disableCrouch = false;
                bool disableProne = false;
                // 切换到趴或者蹲低于水面
                if (inWaterDepth > AnimatorParametersHash.FirstPersonCrouchCameraHeight)
                {
                    disableCrouch = true;
                    disableProne = true;
                }

                // 切换趴低于水面
                if (inWaterDepth > AnimatorParametersHash.FirstPersonProneCameraHeight)
                {
                    disableProne = true;
                }

                for (int i = 0; i < commands.Length; ++i)
                {
                    var v = commands[i];
                    if (v.Type == FsmInput.Crouch && disableCrouch)
                    {
                        v.Type = FsmInput.None;
                        player.tip.TipType = ETipType.CanNotCrouch;
                    }

                    if (v.Type == FsmInput.Prone && disableProne)
                    {
                        v.Type = FsmInput.None;
                        player.tip.TipType = ETipType.CanNotProne;
                    }
                }
            }
        }


        private void SprintDisableTest(ICharacterState state, IAdaptiveContainer<IFsmInputCommand> commands)
        {
            bool slowDown = false;
            if (state.IsMoveInWater() || state.IsSteepSlope())
            {
                for (int i = 0; i < commands.Length; ++i)
                {
                    var v = commands[i];
                    if (v.Type == FsmInput.Sprint)
                    {
                        v.Type = FsmInput.Run;
                        _logger.DebugFormat("sprint to run due to move in water or is steep slope!");
                        slowDown = true;
                    }
                }
            }

            state.SetBeenSlowDown(slowDown);
        }


        private static readonly float LiftUp = 0.1f;
        private static readonly float CastRadius = 0.3f;
        private static readonly float ProbeDist = 1.4f;
        private List<IFsmInputCommand> testCommand = new List<IFsmInputCommand>();
        private List<FsmInput> testCondition = new List<FsmInput>();

        /// <summary>
        /// <p>角色切换姿势会引起包围盒变化，当切换后的包围盒大于当前空间时，会无法切换姿势</p>
        /// 如：玩家站立175cm，当前空间只有160cm则无法站立。蹲趴同理
        /// </summary>
        /// <param name="playerEntity"></param>
        /// <param name="commandsContainer"></param>
        private void StandCrouchDisableTest(PlayerEntity playerEntity,
            IAdaptiveContainer<IFsmInputCommand> commandsContainer)
        {
            var state = playerEntity.stateInterface.State.GetNextPostureState();
            if (!(state == PostureInConfig.Crouch || state == PostureInConfig.Prone))
            {
                return;
            }

            // crouchDisable
            testCondition.Clear();
            if (state == PostureInConfig.Crouch)
            {
                // to stand
                testCondition.Add(FsmInput.Jump);
                testCondition.Add(FsmInput.Crouch);
            }
            else
            {
                // to stand
                testCondition.Add(FsmInput.Jump);
                testCondition.Add(FsmInput.Prone);
                // to crouch
                testCondition.Add(FsmInput.Crouch);
            }

            testCommand.Clear();
            for (int i = 0; i < commandsContainer.Length; i++)
            {
                var v = commandsContainer[i];
                if (testCondition.Contains(v.Type))
                {
                    testCommand.Add(commandsContainer[i]);
                    _logger.DebugFormat("match type:{0}, state:{1}", v.Type, state);
                }
            }

            if (testCommand.Count == 0)
            {
                return;
            }

            float targetHeight = 0.0f;
            bool containsCrouch = false;
            foreach (IFsmInputCommand command in testCommand)
            {
                if (command.Type == FsmInput.Crouch)
                {
                    containsCrouch = true;
                }
            }

            bool toStand = false;
            // to stand
            if (state == PostureInConfig.Crouch || (state == PostureInConfig.Prone && !containsCrouch))
            {
                targetHeight = SingletonManager.Get<CharacterStateConfigManager>()
                    .GetCharacterControllerCapsule(PostureInConfig.Stand)
                    .Height;
                toStand = true;
            }
            // to crouch
            else
            {
                targetHeight = SingletonManager.Get<CharacterStateConfigManager>()
                    .GetCharacterControllerCapsule(PostureInConfig.Crouch)
                    .Height;
                toStand = false;
            }

            var gameObject = playerEntity.RootGo();
            var prevLayer = gameObject.layer;
            IntersectionDetectTool.SetColliderLayer(gameObject, UnityLayerManager.GetLayerIndex(EUnityLayerName.User));
            var startPoint = gameObject.transform.position;
            //UnityLayers.
            // a shift lift up
            startPoint.y += CastRadius;
            RaycastHit outHit;

//            DebugDraw.DebugWireSphere(startPoint, Color.red, CastRadius, 1f);
//            DebugDraw.DebugWireSphere(startPoint + new Vector3(0,targetHeight - CastRadius - LiftUp,0), Color.magenta, CastRadius, 1f);

            if (PhysicsCastHelper.SphereCast(startPoint, CastRadius, Vector3.up, out outHit, targetHeight - CastRadius,
                UnityLayers.AllCollidableLayerMask, QueryTriggerInteraction.Ignore, LiftUp))
            {
                foreach (IFsmInputCommand command in testCommand)
                {
                    _logger.InfoFormat(
                        "chang command:{0} to none, because current state:{1} can not stand up!, collider name:{2}, collid point:{3}, collider normal:{4}",
                        command.Type,
                        state,
                        outHit.collider.gameObject.name,
                        outHit.point,
                        outHit.normal);
                    command.Type = FsmInput.None;
                }

                if (toStand)
                {
                    playerEntity.tip.TipType = ETipType.CanNotStand;
                }

                //Debug.DrawLine(outHit.point, outHit.normal, Color.red, 5000.0f);
            }

            IntersectionDetectTool.SetColliderLayer(gameObject, prevLayer);
            testCommand.Clear();
            testCondition.Clear();
        }


        private static readonly float ProneOffset = 0.1f;
        private static readonly float RadiusOffset = -0.05f;

        /// <summary>
        /// 距离过近不能趴下
        /// </summary>
        /// <param name="playerEntity"></param>
        /// <param name="commandsContainer"></param>
        private void ProneDisableTest(PlayerEntity playerEntity, IAdaptiveContainer<IFsmInputCommand> commandsContainer)
        {
            var state = playerEntity.stateInterface.State.GetNextPostureState();
            if (!(state == PostureInConfig.Stand || state == PostureInConfig.Crouch))
            {
                return;
            }

            // crouchDisable
            testCondition.Clear();
            testCondition.Add(FsmInput.Prone);
            testCommand.Clear();
            for (int i = 0; i < commandsContainer.Length; i++)
            {
                var v = commandsContainer[i];
                if (testCondition.Contains(v.Type))
                {
                    testCommand.Add(commandsContainer[i]);
                    _logger.DebugFormat("match type:{0}, state:{1}, in ProneDisableTest", v.Type, state);
                }
            }

            if (testCommand.Count == 0)
            {
                return;
            }


            _logger.DebugFormat("prone test!!!");

            var gameObject = playerEntity.RootGo();
            var prevLayer = gameObject.layer;
            IntersectionDetectTool.SetColliderLayer(gameObject, UnityLayerManager.GetLayerIndex(EUnityLayerName.User));

            var positionValue = playerEntity.position.Value;

            var crouchHeight =
                SingletonManager.Get<CharacterStateConfigManager>()
                    .GetCharacterControllerCapsule(PostureInConfig.Crouch).Height;
            var radius =
                SingletonManager.Get<CharacterStateConfigManager>()
                    .GetCharacterControllerCapsule(PostureInConfig.Crouch).Radius + RadiusOffset;
            var newCenter = new Vector3(positionValue.x, positionValue.y + crouchHeight - radius, positionValue.z);
            var distHemi =
                SingletonManager.Get<CharacterStateConfigManager>().GetCharacterControllerCapsule(PostureInConfig.Stand)
                    .Height * 0.5f - radius - ProneOffset;
            var topHemi = newCenter + playerEntity.orientation.RotationYaw.Forward().normalized * distHemi;
            var bottomHemi = newCenter - playerEntity.orientation.RotationYaw.Forward().normalized * distHemi;
            //DebugDraw.EditorDrawCapsule(bottomHemi, topHemi, radius, Color.red, 1f, false);
            //_logger.InfoFormat("topHemi:{0}, bottomHei:{1},distHemi:{2}, crouchHeight:{3}, radius:{4}", topHemi.ToStringExt(), bottomHemi.ToStringExt(), distHemi,crouchHeight, radius);
            int nbUnfilteredHits = Physics.OverlapCapsuleNonAlloc(
                bottomHemi, topHemi, radius,
                IntersectionDetectTool._internalColliders,
                UnityLayers.AllCollidableLayerMask,
                QueryTriggerInteraction.Ignore);
            if (nbUnfilteredHits > 0)
            {
                foreach (IFsmInputCommand command in testCommand)
                {
                    command.Type = FsmInput.None;
                }

                playerEntity.tip.TipType = ETipType.CanNotProne;
                for (int i = 0; i < nbUnfilteredHits; ++i)
                {
                    _logger.InfoFormat("can not prone due to collider:{0}",
                        IntersectionDetectTool._internalColliders[i].name);
                }
            }

            IntersectionDetectTool.SetColliderLayer(gameObject, prevLayer);
            testCommand.Clear();
            testCondition.Clear();
        }

        private void PostureInterruptAction(PlayerEntity player, IUserCmd cmd)
        {
            // 打断当前动作
            if (cmd.IsProne)
            {
                player.stateInterface.State.InterruptAction();
            }
        }

        #region LifeState

        private void CheckPlayerLifeState(PlayerEntity player)
        {
            if (null == player || null == player.playerGameState) return;
            var gameState = player.playerGameState;
            switch (gameState.CurrentPlayerLifeState)
            {
                case PlayerLifeStateEnum.Reborn:
                    Reborn(player);
                    break;
                case PlayerLifeStateEnum.Revive:
                    Revive(player);
                    break;
                case PlayerLifeStateEnum.Dying:
                    Dying(player);
                    break;
                case PlayerLifeStateEnum.Dead:
                    Dead(player);
                    break;
            }
        }
        
        private void Reborn(PlayerEntity player)
        {
            if (null == player) return;
            var stateManager = player.stateInterface.State;
            if (null == stateManager) return;
            stateManager.PlayerReborn();
        }
        
        private void Revive(PlayerEntity player)
        {
            if (null == player) return;
            var stateManager = player.stateInterface.State;
            if (null == stateManager) return;
            stateManager.Revive();
        }

        private void Dying(PlayerEntity player)
        {
            if (null == player) return;
            var stateManager = player.stateInterface.State;
            if (null == stateManager) return;
            stateManager.Dying();
        }

        private void Dead(PlayerEntity player)
        {
            if (null == player) return;
            var stateManager = player.stateInterface.State;
            if (null == stateManager) return;
            stateManager.PlayerReborn();
            _logger.InfoFormat("PlayerUpdateSystemDead");
        }

        #endregion
    }
}
