using Core.Fsm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.CharacterState;

namespace Core.CharacterState.Action.States
{
    class DriveState : ActionState
    {
        public DriveState(ActionStateId id) : base(id)
        {
            #region VehiclesAnim to KeepNull

            AddTransition(
                (command, addOutput) =>
                {
                    if (command.IsMatch(FsmInput.DriveEnd))
                    {
                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.VehiclesAnimHash,
                                                 AnimatorParametersHash.Instance.VehiclesAnimName,
                                                 AnimatorParametersHash.Instance.VehiclesAnimDisableValue,
                                                 CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ForceEndProneHash,
                                                 AnimatorParametersHash.Instance.ForceEndProneName,
                                                 AnimatorParametersHash.Instance.ForceEndProneDisable,
                                                 CharacterView.ThirdPerson, false);
                        addOutput(FsmOutput.Cache);

                        command.Handled = true;

                        return true;
                    }

                    return false;
                },
                null, (int) ActionStateId.KeepNull, null, 0, new[] { FsmInput.DriveEnd });

            #endregion
        }

        public override FsmStateResponseType HandleInput(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            float seatId = command.AdditioanlValue % 10;
            if (command.IsMatch(FsmInput.DriveStart))
            {
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.VehiclesAnimStateHash,
                                         AnimatorParametersHash.Instance.VehiclesAnimStateName,
                                         seatId,
                                         CharacterView.ThirdPerson);
                addOutput(FsmOutput.Cache);

                command.Handled = true;
            }

            return base.HandleInput(command, addOutput);
        }
    }
}
