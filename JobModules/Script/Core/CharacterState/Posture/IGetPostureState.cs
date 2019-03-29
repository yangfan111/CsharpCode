using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlConfig;

namespace Core.CharacterState.Posture
{
    public interface IGetPostureState
    {
        PostureInConfig GetCurrentPostureState();
        PostureInConfig GetNextPostureState();
        LeanInConfig GetCurrentLeanState();
        LeanInConfig GetNextLeanState();
    }
}
