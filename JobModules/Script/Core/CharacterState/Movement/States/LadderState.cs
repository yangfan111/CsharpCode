using System;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Movement.States
{
    internal class LadderState : MovementState
    {
        //private static LoggerAdapter _logger = new LoggerAdapter(typeof(LadderState));

        public LadderState(MovementStateId id) : base(id)
        {
            #region ladder to exitladder

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.ExitLadder))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.LadderHash,
                            AnimatorParametersHash.Instance.LadderName,
                            AnimatorParametersHash.Instance.LadderDisableValue,
                            CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.LadderExitStateHash,
                            AnimatorParametersHash.Instance.LadderExitStateName,
                            (int)command.AdditioanlValue,
                            CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FreeFallHash,
                            AnimatorParametersHash.Instance.FreeFallName,
                            AnimatorParametersHash.Instance.FreeFallDisable,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                        return true;
                    }

                    return false;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int) MovementStateId.ExitLadder, null, 0, new[] {FsmInput.ExitLadder});

            #endregion
            
            #region interruptLadder

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.InterruptLadder))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.LadderHash,
                            AnimatorParametersHash.Instance.LadderName,
                            AnimatorParametersHash.Instance.LadderDisableValue,
                            CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.LadderExitStateHash,
                            AnimatorParametersHash.Instance.LadderExitStateName,
                            AnimatorParametersHash.Instance.LadderExitStateMiddle,
                            CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                        
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MotionHash,
                            AnimatorParametersHash.Instance.MotionName,
                            AnimatorParametersHash.Instance.MotionlessValue,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                        
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FreeFallHash,
                            AnimatorParametersHash.Instance.FreeFallName,
                            AnimatorParametersHash.Instance.FreeFallDisable,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                        return true;
                    }

                    return false;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int) MovementStateId.Idle, null, 0, new[] {FsmInput.InterruptLadder});

            #endregion
        }

        public override FsmStateResponseType HandleInput(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            if (command.IsMatch(FsmInput.LadderSpeed))
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.LadderSpeedHash,
                    AnimatorParametersHash.Instance.LadderSpeedName,
                    command.AdditioanlValue,
                    CharacterView.ThirdPerson);
                addOutput(FsmOutput.Cache);

                command.Handled = true;
            }
            
            return base.HandleInput(command, addOutput);
        }
    }
}
