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
                LerpUpperBodyLayerWeight(command, addOutput, SingletonManager.Get<CharacterStateConfigManager>().MeleeLayerWeightTransitionTime);
            }
            
            if (command.IsMatch(FsmInput.LightMeleeAttackOne))
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeStateHash,
                    AnimatorParametersHash.Instance.MeleeAttackName,
                    AnimatorParametersHash.Instance.LightMeleeOne,
                    CharacterView.FirstPerson | CharacterView.ThirdPerson);
                addOutput(FsmOutput.Cache);
                TurnOnUpperBodyOverlay(addOutput);
            }
            else if (command.IsMatch(FsmInput.LightMeleeAttackTwo))
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeStateHash,
                    AnimatorParametersHash.Instance.MeleeAttackName,
                    AnimatorParametersHash.Instance.LightMeleeTwo,
                    CharacterView.FirstPerson | CharacterView.ThirdPerson);
                addOutput(FsmOutput.Cache);
                TurnOnUpperBodyOverlay(addOutput);
            }
            else if (command.IsMatch(FsmInput.MeleeSpecialAttack))
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeStateHash,
                    AnimatorParametersHash.Instance.MeleeAttackName,
                    AnimatorParametersHash.Instance.ForceMelee,
                    CharacterView.FirstPerson | CharacterView.ThirdPerson);
                addOutput(FsmOutput.Cache);
                TurnOnUpperBodyOverlay(addOutput);
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

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeStateHash,
                AnimatorParametersHash.Instance.MeleeStateName,
                AnimatorParametersHash.Instance.NullMelee,
                CharacterView.FirstPerson | CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);

            base.DoBeforeLeaving(addOutput);
        }
    }
}
