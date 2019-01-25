using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Fsm
{
    public enum FsmTransitionResponseType
    {
        Enter,
        // 强制Transition结束，并Reenter
        ForceEnd,
        // 取消Transition，并重新Transfer
        ChangeRoad,
        // 外部条件导致的Transition结束
        ExternalEnd,
        NoResponse
    }

    public enum FsmStateResponseType
    {
        Pass,
        Transfer,
        Reenter
    }

    public enum FsmTransitionMode
    {
        Pop = 1,
        Push,
        PopPush
    }

    [Flags]
    public enum FsmUpdateType
    {
        // 对输入，人物落地等事件有响应，概念上实际为上一帧处理完之后到这一帧处理前的所有事件
        ResponseToInput            = 0x1,
        // 对动画播放事件有响应
        ResponseToAnimation        = 0x2
    }
}
