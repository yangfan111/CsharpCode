using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.CharacterState.Movement
{
    enum MovementStateId
    {
        Idle,               // 0
        Walk,               // 1
        Run,                // 2
        Sprint,             // 3
        Swim,               // 4
        Dive,               // 5
        DiveMove,               // 6
        Injured,            // 7

        EnumEnd
    }
}
