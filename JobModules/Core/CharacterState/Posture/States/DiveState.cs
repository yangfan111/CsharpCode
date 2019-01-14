using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Appearance;
using Core.Fsm;
using Utils.CharacterState;

namespace Core.CharacterState.Posture.States
{
    class DiveState:PostureState
    {
        public DiveState(PostureStateId id) : base(id)
        {
        }

        public override void DoBeforeEntering(IFsmInputCommand command, Action<FsmOutput> addOutput)
        {
            base.DoBeforeEntering(command, addOutput);
            FsmOutput.Cache.SetValue(FsmOutputType.ChangeDiveSensitivity,
                InputSchemeConst.Dive);
            addOutput(FsmOutput.Cache);
        }

        public override void DoBeforeLeaving(Action<FsmOutput> addOutput)
        {
            base.DoBeforeLeaving(addOutput);
            FsmOutput.Cache.SetValue(FsmOutputType.ChangeDiveSensitivity,
                InputSchemeConst.Default);
            addOutput(FsmOutput.Cache);
        }
    }
}
