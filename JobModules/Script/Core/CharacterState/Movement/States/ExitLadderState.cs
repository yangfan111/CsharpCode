using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Movement.States
{
    internal class ExitLadderState : MovementState
    {
        //private static LoggerAdapter _logger = new LoggerAdapter(typeof(LadderState));

        public ExitLadderState(MovementStateId id) : base(id)
        {
            #region ladder to exitladder

            AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.ExitLadderFinished),
                null, (int) MovementStateId.Idle, null, 0, new[] {FsmInput.ExitLadderFinished});

            #endregion
        }

        public override void DoBeforeLeaving(System.Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.LadderHash,
                AnimatorParametersHash.Instance.LadderName,
                AnimatorParametersHash.Instance.LadderDisableValue,
                CharacterView.ThirdPerson, false);
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

            base.DoBeforeLeaving(addOutput);
        }
    }
}
