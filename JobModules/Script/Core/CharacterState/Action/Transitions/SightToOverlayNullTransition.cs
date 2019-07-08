using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Action.Transitions
{
    class SightToOverlayNullTransition : FsmTransition
    {
        public SightToOverlayNullTransition(short id, short target, int duration) : base(id, target, duration)
        {
            _simpleTransferCondition = (command, addOutput) =>
            {
                var ret = command.IsMatch(FsmInput.CancelSight);

                if (ret)
                {
                    FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.ADSLayer,
                                                   AnimatorParametersHash.Instance.ADSDisableValue,
                                                   CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);

                    FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.ADSLowerBodyLayerP1,
                                                   AnimatorParametersHash.Instance.ADSDisableValue,
                                                   CharacterView.FirstPerson);
                    addOutput(FsmOutput.Cache);
                    
                    FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.ADSUpperBodyLayerP1,
                                                   AnimatorParametersHash.Instance.ADSDisableValue,
                                                   CharacterView.FirstPerson);
                    addOutput(FsmOutput.Cache);

                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.EnableSightMoveHash,
                                             AnimatorParametersHash.Instance.EnableSightMoveName,
                                             AnimatorParametersHash.Instance.EnableSightMoveDisableValue,
                                             CharacterView.FirstPerson, true);
                    addOutput(FsmOutput.Cache);

                    _speedRatio = command.AdditioanlValue;
                    command.Handled = true;
                }

                return ret;
            };

            _interruptCondition = (command, action) =>
            {
                if (command.IsMatch(FsmInput.Sight))
                {
                    command.AdditioanlValue = NormalizedTime;
                    return FsmTransitionResponseType.ForceEnd;
                }

                return FsmTransitionResponseType.NoResponse;
            };
            
            _update = GetLerpFunc(FsmOutputType.FirstPersonSight, 1, 0);
        }

        public override void Init(IFsmInputCommand command)
        {
            base.Init(command);
            if (InitValue != 0)
            {
                NormalizedTime = 1 - InitValue;
            }
        }
    }
}
