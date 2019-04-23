using System;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Posture
{
    internal class CustomPostureState:PostureState
    {
        private float _height;
        private float _forward;
        private Func<float> _controllerHeight;
        private Func<float> _controllerRadius;
        private PostureStateId _stateId;
        
        public CustomPostureState(PostureStateId id, float height, float forward) : base(id)
        {
            _stateId = id;
            _height = height;
            _forward = forward;
            if (PostureStateId.Prone == id)
            {
                _controllerHeight = () => _characterInfo.GetProneCapsule().Height;
                _controllerRadius = () => _characterInfo.GetProneCapsule().Radius;
            }
            else if (PostureStateId.Crouch == id)
            {
                _controllerHeight = () => _characterInfo.GetCrouchCapsule().Height;
                _controllerRadius = () => _characterInfo.GetCrouchCapsule().Radius;
            }
            else
            {
                _controllerHeight = () => _characterInfo.GetStandCapsule().Height;
                _controllerRadius = () => _characterInfo.GetStandCapsule().Radius;
            }
            
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            base.DoBeforeEntering(command, addOutput);
            FsmOutput.Cache.SetValue(FsmOutputType.FirstPersonHeight,
                _height);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(FsmOutputType.FirstPersonForwardOffset,
                _forward);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(FsmOutputType.CharacterControllerHeight, _controllerHeight.Invoke());
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(FsmOutputType.CharacterControllerRadius, _controllerRadius.Invoke());
            addOutput(FsmOutput.Cache);

            switch (_stateId)
            {
                case PostureStateId.Prone:
                {
                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                        AnimatorParametersHash.Instance.PostureName,
                        AnimatorParametersHash.Instance.ProneValue,
                        CharacterView.FirstPerson | CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);
                    break;
                }
                case PostureStateId.Crouch:
                {
                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                        AnimatorParametersHash.Instance.PostureName,
                        AnimatorParametersHash.Instance.CrouchValue,
                        CharacterView.FirstPerson | CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);
                    break;
                }
                case PostureStateId.Stand:
                {
                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                        AnimatorParametersHash.Instance.PostureName,
                        AnimatorParametersHash.Instance.StandValue,
                        CharacterView.FirstPerson | CharacterView.ThirdPerson);
                    addOutput(FsmOutput.Cache);
                    break;
                }  
            }
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);
            
            switch (_stateId)
            {
                case PostureStateId.Prone:
                {
                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ForceToProneHash,
                        AnimatorParametersHash.Instance.ForceToProneName,
                        AnimatorParametersHash.Instance.ForceToProneDisable,
                        CharacterView.ThirdPerson, false);
                    addOutput(FsmOutput.Cache);
                    break;
                }
                case PostureStateId.Crouch:
                case PostureStateId.Stand:
                {
                    FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ForceEndProneHash,
                        AnimatorParametersHash.Instance.ForceEndProneName,
                        AnimatorParametersHash.Instance.ForceEndProneDisable,
                        CharacterView.ThirdPerson, false);
                    addOutput(FsmOutput.Cache);
                    break;
                }
            }
        }
    }
}