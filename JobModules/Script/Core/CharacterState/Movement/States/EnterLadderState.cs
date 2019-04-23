using System;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Movement.States
{
    internal class EnterLadderState : MovementState
    {
        //private static LoggerAdapter _logger = new LoggerAdapter(typeof(LadderState));

        public EnterLadderState(MovementStateId id) : base(id)
        {
            #region enterladder to Ladder

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.EnterLadderFinished))
                    {
                        return true;
                    }
                    return false;
                },
                (command, addOutput) => FsmTransitionResponseType.NoResponse,
                (int) MovementStateId.Ladder, null, 0, new[] {FsmInput.EnterLadderFinished});
            
            #endregion
            
            #region enterladder to exitladder
            
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
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            base.DoBeforeEntering(command, addOutput);

            int a = 0;
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);

            int a = 0;
        }
    }
}
