using App.Shared.Components.Player;
using App.Shared.GameModules.Camera.Utils;
using Core.Compare;
using Core.Fsm;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Appearance;
using Utils.Utils;
using XmlConfig;

namespace App.Shared.GameModules.Player.CharacterState
{
    public class FsmInputCreator
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FsmInputCreator));

        private static readonly Dictionary<FsmInput, List<PostureInConfig>> FilterFsmInputByStateDict = new Dictionary<FsmInput, List<PostureInConfig>>(FsmInputEqualityComparer.Instance)
        {
            {FsmInput.Up, new List<PostureInConfig>{ PostureInConfig.Dive} },
            {FsmInput.Down, new List<PostureInConfig>{PostureInConfig.Dive, PostureInConfig.Swim} },
            {FsmInput.DiveMove, new List<PostureInConfig>{PostureInConfig.Dive} },
        };

        private static readonly int InitCommandLen = 5;
        private static readonly float EPS = 0.0001f;

        private List<IFsmInputFilter> _filters = new List<IFsmInputFilter>
        {
            new ProneStateFilter(), new DiveStateFilter(), /*new JumpStateFilter(),*/ new ClimbStateFilter(), new ProneToCrouchStateFilter(),new ProneToStandStateFilter(),new ProneTransitStateFilter()
        };

        private FsmInputContainer _commandsContainer = new FsmInputContainer(InitCommandLen);

        public FsmInputCreator()
        {

        }

        public IAdaptiveContainer<IFsmInputCommand> CommandsContainer { get { return _commandsContainer; } }

        public void CreateCommands(IUserCmd cmd, FilterState state, PlayerEntity player, Contexts contexts) //,int curLeanState,int leanTimeCount)
        {
            FromUserCmdToFsmInput(cmd, player, contexts);
            foreach (var v in _filters)
            {
                TryFilter(v, state);
            }

            BlockFilter(state);
        }

        /// <summary>
        /// 一些命令只有在特定的状态下使用
        /// </summary>
        /// <param name="state"></param>
        private void BlockFilter(FilterState state)
        {
            for (int i = 0; i < CommandsContainer.Length; ++i)
            {
                if (FilterFsmInputByStateDict.ContainsKey(CommandsContainer[i].Type) && !FilterFsmInputByStateDict[CommandsContainer[i].Type].Contains(state.Posture, CommonIntEnumEqualityComparer<PostureInConfig>.Instance))
                {
                    CommandsContainer[i].Reset();
                }
            }
        }

        public void Reset()
        {
            _commandsContainer.Reset();
        }

        private void FromUserCmdToFsmInput(IUserCmd cmd, PlayerEntity player, Contexts contexts)
        {
            if (player.hasGamePlay && player.gamePlay.IsObserving()) return;
            //Logger.InfoFormat("horzontal val:{0}, vertical val:{1}", cmd.MoveHorizontal, cmd.MoveVertical);
            // 根据WSAD生成FsmInput
            bool IsSprint = false;
            bool IsVariant = IsVariantFilter(player);
            if (CompareUtility.IsApproximatelyEqual(cmd.MoveHorizontal, 0, EPS) 
                && CompareUtility.IsApproximatelyEqual(cmd.MoveVertical, 0, EPS)
                && !player.playerMove.IsAutoRun)
            {
                // WSAD均未按下
                SetCommand(FsmInput.Idle);
            }
            else
            {
                if (!CompareUtility.IsApproximatelyEqual(cmd.MoveHorizontal, 0, EPS))
                {
                    SetCommand(cmd.MoveHorizontal > 0 ? FsmInput.Right : FsmInput.Left, cmd.MoveHorizontal);
                }
                if (!CompareUtility.IsApproximatelyEqual(cmd.MoveVertical, 0, EPS))
                {
                    SetCommand(cmd.MoveVertical > 0 ? FsmInput.Forth : FsmInput.Back, cmd.MoveVertical);
                }
                if (PlayerStateUtil.HasPlayerState(EPlayerGameState.UseItem, player.gamePlay) || player.IsAiming()
                    || player.StateInteractController().GetCurrStates().Contains(EPlayerState.RunDebuff))
                {
                    SetCommand(FsmInput.Walk);
                }
                else
                {
                    if(player.playerMove.IsAutoRun)
                    {
                        if (IsVariant && player.hasStateInterface &&
                            player.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Crouch)
                        {
                            SetCommand(FsmInput.Forth, InputValueLimit.MaxAxisValue);
                            SetCommand(FsmInput.Run);
                        }
                        else
                        {
                            SetCommand(FsmInput.Forth, InputValueLimit.MaxAxisValue);
                            SetCommand(FsmInput.Sprint);
                        }
                    }
                    // 冲刺
                    IsSprint = (cmd.MoveVertical > 0 && cmd.MoveVertical >= Math.Abs(cmd.MoveHorizontal));
                    if (cmd.IsRun && IsCanSprint(cmd, player))
                    {
                        // 冲刺只有前向90度
                        if (IsSprint)
                        {
                            if (!(player.hasStateInterface && player.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Crouch && IsVariantFilter(player))) {
                                SetCommand(FsmInput.Sprint);
                            }
                        }
                        else
                        {
                            SetCommand(FsmInput.Run);
                        }
                    }
                    // 静走 静走不被限制
                    else if (cmd.FilteredInput.IsInput(EPlayerInput.IsSlightWalk))
                    {
                        if (IsVariant)
                        {
                            SetCommand(FsmInput.Run);
                        }
                        else
                        {
                            SetCommand(FsmInput.Walk);
                        }
                    }
                    else if (IsCanRun(cmd))
                    {
                        SetCommand(FsmInput.Run);
                    }
                    //不能冲刺跑步，就切换为静走
                    else if(cmd.FilteredInput.IsInput(EPlayerInput.IsSlightWalk))
                    {
                        SetCommand(FsmInput.Walk);
                    }
                }
            }

            if (!CompareUtility.IsApproximatelyEqual(cmd.MoveUpDown, 0, EPS))
            {
                SetCommand(cmd.MoveUpDown > 0 ? FsmInput.Up : FsmInput.Down, cmd.MoveUpDown);
            }

            if (CompareUtility.IsApproximatelyEqual(cmd.MoveHorizontal, 0, EPS) &&
                CompareUtility.IsApproximatelyEqual(cmd.MoveVertical, 0, EPS) &&
                CompareUtility.IsApproximatelyEqual(cmd.MoveUpDown, 0, EPS))
            {
                SetCommand(FsmInput.DiveIdle);
            }
            else
            {
                SetCommand(FsmInput.DiveMove);

            }

            if (cmd.IsPeekLeft && !cmd.IsPeekRight && cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsPeekLeft))
            {
                CheckConditionAndSetCommand(cmd, XmlConfig.EPlayerInput.IsPeekLeft, FsmInput.PeekLeft);
            }
            else if (cmd.IsPeekRight && !cmd.IsPeekLeft && cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsPeekRight))
            {
                CheckConditionAndSetCommand(cmd, XmlConfig.EPlayerInput.IsPeekRight, FsmInput.PeekRight);
            }
            else
            {
                SetCommand(FsmInput.NoPeek);
            }

            if (cmd.IsJump)
            {
				CheckConditionAndSetCommand(cmd, XmlConfig.EPlayerInput.IsJump, FsmInput.Jump);
            }

            bool runInterrupt =  (cmd.IsRun && 
                IsSprint && 
                IsVariant && 
                player.hasStateInterface && 
                player.stateInterface.State.GetCurrentPostureState() == PostureInConfig.Crouch);
            UserCmd.SetInterrupt(UserCmdEnum.IsRun, runInterrupt ? UserCmdHandle.Interrupt : UserCmdHandle.None);

            if (cmd.IsCrouch)
            {
                bool autoRunSynchronous = (player.playerMove.IsAutoRun && IsVariant);
                UserCmd.SetInterrupt(UserCmdEnum.IsRun, autoRunSynchronous ? UserCmdHandle.Synchronous : UserCmdHandle.None);
                UserCmd.SetInterrupt(UserCmdEnum.IsSwitchAutoRun, autoRunSynchronous ? UserCmdHandle.Synchronous : UserCmdHandle.None);

                CheckConditionAndSetCommand(cmd, XmlConfig.EPlayerInput.IsCrouch, FsmInput.Crouch);
            }

            if (cmd.IsCDown)
            {
                SetCommand(FsmInput.BigJump);
            }
            
            if (cmd.IsProne && !IsVariant)
            {
                CheckConditionAndSetCommand(cmd, XmlConfig.EPlayerInput.IsProne, FsmInput.Prone);
            }
        }

        private bool IsVariantFilter(PlayerEntity player)
        {
            return (player.hasGamePlay &&
                (player.gamePlay.JobAttribute == (int)EJobAttribute.EJob_Variant ||
                player.gamePlay.JobAttribute == (int)EJobAttribute.EJob_Matrix));
        }

        private bool IsCanSprint(IUserCmd cmd, PlayerEntity playerEntity)
        {
            var stateBlock = !cmd.FilteredInput.IsInput(EPlayerInput.IsSprint);
            var configAssy = playerEntity.WeaponController().HeldWeaponAgent.WeaponConfigAssy;
            var weaponBlock = configAssy != null && configAssy.S_CantRun;
            return !stateBlock && !weaponBlock;
        }

        private bool IsCanRun(IUserCmd cmd)
        {
            return cmd.FilteredInput.IsInput(EPlayerInput.IsRun);
        }

        private void CheckConditionAndSetCommand(IUserCmd cmd, XmlConfig.EPlayerInput mappedInput, FsmInput fsmInput)
        {
            if (null != cmd.FilteredInput && cmd.FilteredInput.IsInput(mappedInput))
            {
                SetCommand(fsmInput);
            }
            else
            {
                if(null == cmd.FilteredInput)
                {
                    Logger.Error("FilteredInput in cmd should never be null !");
                }
            }
        }

        private void SetCommand(FsmInput type, float value = 0f)
        {
            var command = CommandsContainer.GetAvailableItem();
            command.Type = type;
            command.AdditioanlValue = value;
        }

        private void TryFilter(IFsmInputFilter filter, FilterState state)
        {
            filter.SetCurrentState(state);
            if (filter.Active)
            {
                filter.Filter(CommandsContainer);
            }
        }
    }
}
