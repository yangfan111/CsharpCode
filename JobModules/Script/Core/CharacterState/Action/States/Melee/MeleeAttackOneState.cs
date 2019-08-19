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
    class MeleeAttackOneState : ActionState
    {
        public MeleeAttackOneState(ActionStateId id) : base(id)
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

            #region Melee to CommonNull(due to interrupt)
            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.LightMeleeAttackOne))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeStateHash,
                            AnimatorParametersHash.Instance.MeleeStateName,
                            AnimatorParametersHash.Instance.NullMelee,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                        TurnOnUpperBodyOverlay(addOutput);

                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                null, (int) ActionStateId.CommonNull, null, 0, new[] {FsmInput.LightMeleeAttackOne});
            #endregion
            
            #region Melee to AttackTwo
            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.LightMeleeAttackTwo))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeStateHash,
                            AnimatorParametersHash.Instance.MeleeStateName,
                            AnimatorParametersHash.Instance.LightMeleeTwo,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                        TurnOnUpperBodyOverlay(addOutput);

                        command.Handled = true;
                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.MeleeAttackTwo, null, 0, new[] {FsmInput.LightMeleeAttackTwo});
            #endregion
            
            #region Melee to AttackSpecial
            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.MeleeSpecialAttack))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeStateHash,
                            AnimatorParametersHash.Instance.MeleeStateName,
                            AnimatorParametersHash.Instance.ForceMelee,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson);
                        addOutput(FsmOutput.Cache);
                        TurnOnUpperBodyOverlay(addOutput);

                        command.Handled = true;
                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.MeleeAttackSpecial, null, 0, new[] {FsmInput.MeleeSpecialAttack});
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
