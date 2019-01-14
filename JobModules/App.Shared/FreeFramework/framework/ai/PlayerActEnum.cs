using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.wd.free.ai
{
    public class PlayerActEnum
    {
        public enum CmdType
        {
            Idle = 0, Walk = 1, PressKey = 2, InterceptKey = 3, Attack = 4, ClearKeys = 5
        }
    }
}
