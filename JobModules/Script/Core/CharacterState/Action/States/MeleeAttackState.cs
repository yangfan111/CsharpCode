using Core.Fsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Configuration;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;
using FsmInput = Core.Fsm.FsmInput;

namespace Core.CharacterState.Action.States
{
    class MeleeAttackState : ActionState
    {
        public MeleeAttackState(ActionStateId id) : base(id)
        {
            #region MeleeAttack to CommonNull

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.MeleeAttackFinished))
                    {
                        command.Handled = true;
                        
                        TurnOffUpperBodyOverlay(addOutput);

                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.CommonNull, null, 0, new[] { FsmInput.MeleeAttackFinished});

            #endregion
        }

        public override FsmStateResponseType HandleInput(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            if (command.IsMatch(FsmInput.MeleeAttackProgressP3))
            {
                LerpUpperBodyLayerWeight(command, addOutput, SingletonManager.Get<CharacterStateConfigManager>().LongLayerWeightTransitionTime);
            }
            return base.HandleInput(command, addOutput);
        }

        public override void Update(int frameInterval, Action<FsmOutput> addOutput)
        {
            if (UpdateForTheFirstTime)
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeAttackHash,
                                         AnimatorParametersHash.Instance.MeleeAttackName,
                                         AnimatorParametersHash.Instance.MeleeAttackEnd,
                                         CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                addOutput(FsmOutput.Cache);
            }
            
            base.Update(frameInterval, addOutput);
        }
    }
}
