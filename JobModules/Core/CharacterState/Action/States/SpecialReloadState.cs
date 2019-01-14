using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Action.States
{
    class SpecialReloadState : ActionState
    {
        private int _reloadCount;

        public SpecialReloadState(ActionStateId id) : base(id)
        {
            #region SpecialReload To CommonNull 1

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.ReloadFinished))
                    {
                        command.Handled = true;
                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.CommonNull, null, 0, new[] { FsmInput.ReloadFinished });

            #endregion

            #region SpecialReload To CommonNull 2

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.ForceBreakSpecialReload))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ForceSpecialReloadEndHash,
                                                 AnimatorParametersHash.Instance.ForceSpecialReloadEndName,
                                                 AnimatorParametersHash.Instance.ForceSpecialReloadEndEnableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SpecialReloadHash,
                                                 AnimatorParametersHash.Instance.SpecialReloadName,
                                                 AnimatorParametersHash.Instance.SpecialReloadDisableValue,
                                                 CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.CommonNull, null, 0, new[] { FsmInput.ForceBreakSpecialReload });

            #endregion
            
            #region Interrupt

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.InterruptAction))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.InterruptHash,
                            AnimatorParametersHash.Instance.InterruptName,
                            AnimatorParametersHash.Instance.InterruptEnable,
                            CharacterView.FirstPerson | CharacterView.ThirdPerson, true);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;
                        return FsmStateResponseType.Reenter;
                    }

                    return FsmStateResponseType.Pass;
                },
                null, (int)ActionStateId.CommonNull, null, 0, new[] { FsmInput.InterruptAction });

            #endregion
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            base.DoBeforeEntering(command, addOutput);

            _reloadCount = (int)command.AdditioanlValue;
            
            // handled here is better than DoBeforeLeaving, consider reset
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ForceSpecialReloadEndHash,
                                     AnimatorParametersHash.Instance.ForceSpecialReloadEndName,
                                     AnimatorParametersHash.Instance.ForceSpecialReloadEndDisableValue,
                                     CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
        }
        
        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);
            
            TurnOffUpperBodyOverlay(addOutput);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.InterruptHash,
                AnimatorParametersHash.Instance.InterruptName,
                AnimatorParametersHash.Instance.InterruptDisable,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SpecialReloadHash,
                AnimatorParametersHash.Instance.SpecialReloadName,
                AnimatorParametersHash.Instance.SpecialReloadDisableValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
        }

        public override FsmStateResponseType HandleInput(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            if (command.IsMatch(FsmInput.SpecialReloadTrigger))
            {
                --_reloadCount;
            }

            if (_reloadCount <= 0 || command.IsMatch(FsmInput.BreakSpecialReload))
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ForceSpecialReloadEndHash,
                                         AnimatorParametersHash.Instance.ForceSpecialReloadEndName,
                                         AnimatorParametersHash.Instance.ForceSpecialReloadEndDisableValue,
                                         CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                addOutput(FsmOutput.Cache);
                
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SpecialReloadHash,
                                         AnimatorParametersHash.Instance.SpecialReloadName,
                                         AnimatorParametersHash.Instance.SpecialReloadDisableValue,
                                         CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                addOutput(FsmOutput.Cache);
                // 不单独区分FsmInput.BreakSpecialReload
                command.Handled = true;

                _reloadCount = int.MaxValue;
            }
            
            return base.HandleInput(command, addOutput);
        }
    }
}
