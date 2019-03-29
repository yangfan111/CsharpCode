using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.CharacterState.Posture
{
    /// <summary>
    /// PostureStateId不要超过short的范围
    /// </summary>
    enum PostureStateId
    {
        JumpStart,                  // 0
        // 长时间掉落的循环动作
        Freefall,                   // 1
        JumpEnd,                    // 2

        Stand,                      // 3
        Crouch,                     // 4
        Prone,                      // 5
        ProneTransit,               // 6
        ProneToStand,               // 7
        ProneToCrouch,              // 8

        Swim,                       // 9
        Dive,                       // 10
        Dying,                      // 11

        PeekLeft,                   // 12
        PeekRight,                  // 13
        NoPeek,                     // 14

        Climb,
        Slide,

        EnumEnd
    }

}
