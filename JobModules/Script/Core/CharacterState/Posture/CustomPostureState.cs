using System;
using Core.Fsm;

namespace Core.CharacterState.Posture
{
    internal class CustomPostureState:PostureState
    {
        private float _height;
        private float _forward;
        private float _controllerHeight;
        private float _controllerRadius;
        
        public CustomPostureState(PostureStateId id, float height, float forward, float controllerHeight, float controllerRadius) : base(id)
        {
            _height = height;
            _forward = forward;
            _controllerHeight = controllerHeight;
            _controllerRadius = controllerRadius;
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

            FsmOutput.Cache.SetValue(FsmOutputType.CharacterControllerHeight, _controllerHeight);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(FsmOutputType.CharacterControllerRadius, _controllerRadius);
            addOutput(FsmOutput.Cache);
        }
    }
}