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
    class PickUpState : ActionState
    {
        public PickUpState(ActionStateId id) : base(id)
        {
            #region PickUp to CommonNull

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.PickUpEnd))
                    {
                        command.Handled = true;
                        
                        TurnOffUpperBodyOverlay(addOutput);

                        return true;
                    }

                    return false;
                },
                null, (int)ActionStateId.CommonNull, null, 0, new[] { FsmInput.PickUpEnd });

            #endregion
        }

        public override FsmStateResponseType HandleInput(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            if (command.IsMatch(FsmInput.PickUpProgressP3))
            {
                LerpUpperBodyLayerWeight(command, addOutput, SingletonManager.Get<CharacterStateConfigManager>().LongLayerWeightTransitionTime);
            }
            return base.HandleInput(command, addOutput);
        }

        public override void Update(int frameInterval, Action<FsmOutput> addOutput)
        {
            if (UpdateForTheFirstTime)
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PickUpHash,
                                         AnimatorParametersHash.Instance.PickUpName,
                                         AnimatorParametersHash.Instance.PickUpDisable,
                                         CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
                addOutput(FsmOutput.Cache);
            }

            base.Update(frameInterval, addOutput);
        }
    }
}
