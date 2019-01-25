using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using XmlConfig;

namespace Core.CharacterState.Posture
{
    public interface ICharacterPosture: IGetPostureState, IGetActionState
    {
        bool IsNeedJumpSpeed();
        /// <summary>
        /// 只有对同步使用
        /// </summary>
        bool IsNeedJumpForSync { get; set; }
    }

    public interface ICharacterPostureInConfig
    {
        bool InTransition();
        float TransitionRemainTime();
        float TransitionTime();
        PostureInConfig CurrentPosture();
        PostureInConfig NextPosture();
    }
}
