using App.Shared.Components.Player;
using App.Shared.Configuration;
using App.Shared.GameModules.Player.Actions;
using App.Shared.GameModules.Player.Animation;
using App.Shared.GameModules.Player.Appearance.AnimationEvent;
using App.Shared.Player;
using Core.Animation;
using Core.AnimatorClip;
using Core.CharacterController;
using Core.CharacterState;
using Core.CharacterState.Posture;

using Core.Fsm;
using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core.WeaponAnimation;
using ECM.Components;
using System.Collections.Generic;
using Core;
using UnityEngine;
using Utils.Appearance;
using Utils.Compare;
using Utils.Configuration;
using Utils.Singleton;
using Utils.Utils;
using XmlConfig;
using System;
using System.Text;

namespace App.Shared.GameModules.Player.CharacterState
{
    public class PlayerStateUpdateSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PlayerStateUpdateSystem));

        private Contexts _contexts;

        private readonly FsmOutputBaseSystem _fsmOutputs = new FsmOutputBaseSystem();
        private readonly FsmInputCreator _inputCreator = new FsmInputCreator();
        private readonly AnimationMonitor _animMonitor = new AnimationMonitor();
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

            var stateManager = playerEntity.stateInterface.State;
            var animatorClipManager = playerEntity.animatorClip.ClipManager;
            playerEntity.stateInterVar.Reset();
            _inputCreator.Reset();
            
            if(Input.GetKeyDown(KeyCode.Alpha8))
                stateManager.InterruptSwitchWeapon();


            // cmd到FsmInput
            _inputCreator.CreateCommands(cmd, new FilterState {Posture = stateManager.GetCurrentPostureState()},
                playerEntity, _contexts);
            PostureInterruptAction(playerEntity, cmd);
            // jump to prone or crouch disable
            JumpTipTest(playerEntity, cmd);

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

                NetworkAnimatorUtil.GetAnimatorLayers(playerEntity.firstPersonAnimator.UnityAnimator,
                    playerEntity.fpAnimStatus.AnimatorLayers);
                AnimatorChange(playerEntity.fpAnimStatus, cmd);
//                流量优化用
//                playerEntity.fpAnimStatus.ConvertStructureDataToCompressData();

                NetworkAnimatorUtil.GetAnimatorLayers(playerEntity.thirdPersonAnimator.UnityAnimator,
                    playerEntity.networkAnimator.AnimatorLayers);
                AnimatorChange(playerEntity.networkAnimator, cmd);
//                流量优化用
//                playerEntity.networkAnimator.ConvertStructureDataToCompressData();
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
                stateManager.Update(commandsContainer, 0, _fsmOutputs.AddOutput, FsmUpdateType.ResponseToAnimation,
                    playerEntity.fsmInputRelateInterface.Relate.GetFsmInputLimits());
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
                    FsmUpdateType.ResponseToInput, playerEntity.fsmInputRelateInterface.Relate.GetFsmInputLimits());
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
                ChangeRoleTest(playerEntity, commandsContainer);
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

        private void ChangeRoleTest(PlayerEntity playerEntity, IAdaptiveContainer<IFsmInputCommand> commands)
        {
            if (playerEntity.hasGamePlay &&
                playerEntity.gamePlay.IsVariant() &&
                playerEntity.hasStateInterface)
            {
                var state = playerEntity.stateInterface.State.GetNextPostureState();
                if (state == PostureInConfig.Stand) {

                    var characterInfo = playerEntity.characterInfo.CharacterInfoProviderContext;
                    float targetHeight = characterInfo.GetStandCapsule().Height;

                    GameObject gameObject = playerEntity.RootGo();
                    Vector3 startPoint = gameObject.transform.position;
                    RaycastHit outHit;
                    startPoint.y += CastRadius;

                    if (PhysicsCastHelper.SphereCast(startPoint, CastRadius, Vector3.up, out outHit, targetHeight - 2 * CastRadius,UnityLayers.AllCollidableLayerMask, QueryTriggerInteraction.Ignore, LiftUp))
                    {
                        //var item = commands.GetAvailableItem(command => { return command.Type == FsmInput.None; });
                        //item.Type = FsmInput.Crouch;
                        playerEntity.stateInterface.State.SetPostureCrouch();
                    }
                }
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
            if (!((state == PostureInConfig.Land || state == PostureInConfig.Stand) && (moveState == MovementInConfig.Sprint || moveState == MovementInConfig.Run)))
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

        private static readonly float MinMoveJumpSpeed = 4f;
        
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
            IntersectionDetectTool.SetColliderDisable(gameObject, IntersectionDetectTool.ColliderEnableState);
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

                isHit = PhysicsCastHelper.SphereCast(position, CastRadius, Vector3.down, out outHit, dist,
                    UnityLayers.AllCollidableLayerMask, QueryTriggerInteraction.Ignore, 0.1f);
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
            }
            
            
            IntersectionDetectTool.RestoreCollider(gameObject, IntersectionDetectTool.ColliderEnableState);
            return isHit;
        }

        private static bool UseSphereCast(ICharacterControllerContext controller)
        {
            return CompareUtility.IsApproximatelyEqual(controller.direction, Vector3.up);
        }

        private void AnimatorChange(AbstractNetworkAnimator networkAnimator, IUserCmd cmd)
        {
            networkAnimator.NeedChangeServerTime = true;
            networkAnimator.BaseServerTime = -1;
        }

        private void FreeFallTest(PlayerEntity player, IAdaptiveContainer<IFsmInputCommand> commands)
        {
            var freeFallTest = (!player.playerMove.IsGround &&
                                !(player.stateInterface.State.GetActionState() == ActionInConfig.Gliding ||
                                  player.stateInterface.State.GetActionState() == ActionInConfig.Parachuting)) &&
                               player.playerMove.Velocity.y < -SpeedManager.Gravity;
            if (NeedFreeFallTest(player) && (freeFallTest || StandToFreefallTest(player)))
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
        
        private static bool NeedFreeFallTest(PlayerEntity player)
        {
            return !IsLadderMove(player) &&
                   player.stateInterface.State.GetCurrentPostureState() != PostureInConfig.Climb;
        }

        private static bool IsLadderMove(PlayerEntity player)
        {
            return player.stateInterface.State.GetCurrentMovementState() == MovementInConfig.Ladder ||
                   player.stateInterface.State.GetCurrentMovementState() == MovementInConfig.EnterLadder;
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
            int slowDown = NoLimit;
            int prevSlowDown = state.GetSteepSlowDown();

            if (IsLimitRun(state.GetSteepAngle(), prevSlowDown))
            {
                for (int i = 0; i < commands.Length; ++i)
                {
                    var v = commands[i];
                    if (v.Type == FsmInput.Sprint || v.Type == FsmInput.Run)
                    {
                        v.Type = FsmInput.Walk;
                        _logger.DebugFormat("sprint to Walk due to move in water or is steep slope!, angle:{0}",state.GetSteepAngle());
                        slowDown = LimitRun;
                    }
                }
            }
            else if (state.IsMoveInWater() || IsLimitSprint(state.GetSteepAngle(), prevSlowDown))
            {
                for (int i = 0; i < commands.Length; ++i)
                {
                    var v = commands[i];
                    if (v.Type == FsmInput.Sprint)
                    {
                        v.Type = FsmInput.Run;
                        _logger.DebugFormat("sprint to run due to move in water or is steep slope!, angle:{0}",state.GetSteepAngle());
                        slowDown = LimitSprint;
                    }
                }
            }
            state.SetSteepSlowDown(slowDown);
        }
        
        

        private bool IsLimitSprint(float tanAngle, int prevSlowDown)
        {
            bool ret = false;
            var sprintbegin =
                Mathf.Tan(Mathf.Deg2Rad * SingletonManager.Get<CharacterStateConfigManager>().SteepLimitSprintBegin);
            var sprintstop =
                Mathf.Tan(Mathf.Deg2Rad * SingletonManager.Get<CharacterStateConfigManager>().SteepLimitSprintStop);
            if (tanAngle >= sprintbegin || (tanAngle >= sprintstop && prevSlowDown >= LimitSprint))
            {
                ret = true;
            }
            return ret;
        }
        
        private bool IsLimitRun(float tanAngle, int prevSlowDown)
        {
            bool ret = false;
            var runBegin =
                Mathf.Tan(Mathf.Deg2Rad * SingletonManager.Get<CharacterStateConfigManager>().SteepLimitRunBegin);
            var runStop =
                Mathf.Tan(Mathf.Deg2Rad * SingletonManager.Get<CharacterStateConfigManager>().SteepLimitRunStop);
            if (tanAngle >= runBegin || (tanAngle >= runStop && prevSlowDown >= LimitRun))
            {
                ret = true;
            }
            return ret;
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
            var characterInfo = playerEntity.characterInfo.CharacterInfoProviderContext;
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
                int keyLength = testCondition.Count;
                for (int j = 0; j < keyLength; ++j)
                {
                    if (testCondition[j] == v.Type)
                    {
                        testCommand.Add(commandsContainer[i]);
                        _logger.DebugFormat("match type:{0}, state:{1}", v.Type, state);
                        break;
                    }
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
                targetHeight = characterInfo.GetStandCapsule().Height;   
                toStand = true;
            }
            // to crouch
            else
            {
                targetHeight = characterInfo.GetCrouchCapsule()
                    .Height;
                toStand = false;
            }

            var gameObject = playerEntity.RootGo();
            var prevLayer = gameObject.layer;
            IntersectionDetectTool.SetColliderDisable(gameObject, IntersectionDetectTool.ColliderEnableState);
            var startPoint = gameObject.transform.position;
            //UnityLayers.
            // a shift lift up
            startPoint.y += CastRadius;
            RaycastHit outHit;

//            DebugDraw.DebugWireSphere(startPoint, Color.red, CastRadius, 1f);
//            DebugDraw.DebugWireSphere(startPoint + new Vector3(0,targetHeight - CastRadius - LiftUp,0), Color.magenta, CastRadius, 1f);

            if (PhysicsCastHelper.SphereCast(startPoint, CastRadius, Vector3.up, out outHit, targetHeight - 2 
            * CastRadius,
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
                else
                {
                    playerEntity.tip.TipType = ETipType.CanNotToCrouch;
                }

                //Debug.DrawLine(outHit.point, outHit.normal, Color.red, 5000.0f);
            }

            IntersectionDetectTool.RestoreCollider(gameObject, IntersectionDetectTool.ColliderEnableState);
            testCommand.Clear();
            testCondition.Clear();
        }


        
        private void JumpTipTest(PlayerEntity playerEntity, IUserCmd cmd)
        {
            var state = playerEntity.stateInterface.State.GetNextPostureState();
            if (!(playerEntity.stateInterface.State.IsFreefallState() || state == PostureInConfig.Slide))
            {
                return;
            }

            if (cmd.IsCrouch)
            {
                playerEntity.tip.TipType = ETipType.CanNotToCrouch;
            }
            if (cmd.IsProne)
            {
                playerEntity.tip.TipType = ETipType.CanNotProne;
            }
        }
        
        private static readonly float ProneOffset = 0.1f;
        private static readonly float RadiusOffset = -0.05f;
        public static readonly int LimitSprint = 1;
        public static readonly int LimitRun = 2;
        public static readonly int NoLimit = 0;

        /// <summary>
        /// 距离过近不能趴下
        /// </summary>
        /// <param name="playerEntity"></param>
        /// <param name="commandsContainer"></param>
        private void ProneDisableTest(PlayerEntity playerEntity, IAdaptiveContainer<IFsmInputCommand> commandsContainer)
        {
            var state = playerEntity.stateInterface.State.GetNextPostureState();
            var characterInfo = playerEntity.characterInfo.CharacterInfoProviderContext;
            if (!(state == PostureInConfig.Stand || state == PostureInConfig.Crouch))
            {
                return;
            }

            // crouchDisable
            testCommand.Clear();
            for (int i = 0; i < commandsContainer.Length; i++)
            {
                var v = commandsContainer[i];
                if (v.Type == FsmInput.Prone)
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
            IntersectionDetectTool.SetColliderDisable(gameObject, IntersectionDetectTool.ColliderEnableState);

            var positionValue = playerEntity.position.Value;

            var crouchHeight =
                characterInfo.GetCrouchCapsule().Height;
            var radius =
                characterInfo.GetCrouchCapsule().Radius + RadiusOffset;
            var newCenter = new Vector3(positionValue.x, positionValue.y + crouchHeight - radius, positionValue.z);
            var distHemi =
                characterInfo.GetStandCapsule()
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

            IntersectionDetectTool.RestoreCollider(gameObject, IntersectionDetectTool.ColliderEnableState);
            testCommand.Clear();
        }

        private void PostureInterruptAction(PlayerEntity player, IUserCmd cmd)
        {
            // 打断当前动作
            if (cmd.IsProne)
            {
          //      player.ModeController().CallBeforeAction(player.WeaponController(),EPlayerActionType.Prone);
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
            if(!player.gamePlay.IsStandPosture)
                stateManager.SetPostureCrouch();
        }

        private void Dying(PlayerEntity player)
        {
            if (null == player) return;
            var stateManager = player.stateInterface.State;
            if (null == stateManager) return;
            stateManager.InterruptAction();
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
