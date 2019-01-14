using Core.Fsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Configuration;
using Utils.CharacterState;
using Utils.Configuration;
using Utils.Singleton;

namespace Core.CharacterState.Action.States
{
    class GrenadeState : ActionState
    {
        public GrenadeState(ActionStateId id) : base(id)
        {
            #region Grenade to CommonNull

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.GrenadeEndFinish))
                    {
                        TurnOffUpperBodyOverlay(addOutput);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.CommonNull, null, 0, new[] { FsmInput.GrenadeEndFinish });

            #endregion

            #region Grenade To CommonNull

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.ForceFinishGrenade))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ForceFinishThrowHash,
                                                 AnimatorParametersHash.Instance.ForceFinishThrowName,
                                                 AnimatorParametersHash.Instance.ForceFinishThrowEnable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, true);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.StartThrowHash,
                                                 AnimatorParametersHash.Instance.StartThrowName,
                                                 AnimatorParametersHash.Instance.StartThrowDisable,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, true);
                        addOutput(FsmOutput.Cache);
                        
                        TurnOffUpperBodyOverlay(addOutput);

                        command.Handled = true;
                        return FsmStateResponseType.Reenter;
                    }
                    return FsmStateResponseType.Pass;
                },
                null, (int) ActionStateId.CommonNull, null, 0, new[] { FsmInput.ForceFinishGrenade });

            #endregion
        }

        public override FsmStateResponseType HandleInput(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            if (command.IsMatch(FsmInput.ChangeGrenadeDistance))
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.NearThrowHash,
                                         AnimatorParametersHash.Instance.NearThrowName,
                                         command.AdditioanlValue,
                                         CharacterView.FirstPerson | CharacterView.ThirdPerson);
                addOutput(FsmOutput.Cache);
                
                command.Handled = true;
            }
            else if (command.IsMatch(FsmInput.FinishGrenade))
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ForceFinishThrowHash,
                                         AnimatorParametersHash.Instance.ForceFinishThrowName,
                                         AnimatorParametersHash.Instance.ForceFinishThrowDisable,
                                         CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                addOutput(FsmOutput.Cache);
                
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.StartThrowHash,
                                         AnimatorParametersHash.Instance.StartThrowName,
                                         AnimatorParametersHash.Instance.StartThrowDisable,
                                         CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                addOutput(FsmOutput.Cache);

                command.Handled = true;
            }
            else if (command.IsMatch(FsmInput.ThrowEndProgressP3))
            {
                LerpUpperBodyLayerWeight(command, addOutput, SingletonManager.Get<CharacterStateConfigManager>().LongLayerWeightTransitionTime);
            }

            return base.HandleInput(command, addOutput);
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            base.DoBeforeEntering(command, addOutput);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ForceFinishThrowHash,
                                     AnimatorParametersHash.Instance.ForceFinishThrowName,
                                     AnimatorParametersHash.Instance.ForceFinishThrowDisable,
                                     CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
        }
    }
}
