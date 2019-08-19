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
using Core.Utils;

namespace Core.CharacterState.Action.States.Melee
{
    class MeleeAttackSpecialState : ActionState
    {
        public MeleeAttackSpecialState(ActionStateId id) : base(id)
        {
            #region MeleeAttack to CommonNull

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.MeleeAttackFinished))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeStateHash,
                            AnimatorParametersHash.Instance.MeleeStateName,
                            AnimatorParametersHash.Instance.NullMelee,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                        
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
                LerpUpperBodyLayerWeight(command, addOutput, SingletonManager.Get<CharacterStateConfigManager>().MeleeLayerWeightTransitionTime);
            }
            
            return base.HandleInput(command, addOutput);
        }
    }
}
