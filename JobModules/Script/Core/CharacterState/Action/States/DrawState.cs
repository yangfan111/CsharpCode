using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Configuration;
using Core.Fsm;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;

namespace Core.CharacterState.Action.States
{
    class DrawState : ActionState
    {
        public DrawState(ActionStateId id) : base(id)
        {
            #region Draw To AdditiveNull

            AddTransition(
                (command, addOutput) => FsmTransition.SimpleCommandHandler(command, FsmInput.SelectFinished),
                null, (int) ActionStateId.CommonNull, null, 0, new[] { FsmInput.SelectFinished });

            #endregion
            
            #region Interrupt

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.InterruptSwitchWeapon))
                    {
                        FsmOutput.Cache.SetValue(FsmOutputType.InterruptAction, 
                            (int)FsmInput.SelectFinished);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                null, (int)ActionStateId.CommonNull, null, 0, new[] { FsmInput.InterruptSwitchWeapon });

            #endregion
        }

        public override FsmStateResponseType HandleInput(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            if (command.IsMatch(FsmInput.SelectProgressP3))
            {
                LerpUpperBodyLayerWeight(command, addOutput, SingletonManager.Get<CharacterStateConfigManager>().LongLayerWeightTransitionTime);
            }
            return base.HandleInput(command, addOutput);
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SelectHash,
                AnimatorParametersHash.Instance.SelectName,
                AnimatorParametersHash.Instance.SelectDisableValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            TurnOffUpperBodyOverlay(addOutput);
        }
    }
}
