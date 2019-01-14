using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.skill;

namespace com.wd.free
{
    public class SimpleSkillArgs : BaseEventArgs, ISkillArgs
    {
        public TestPlayerInput input = new TestPlayerInput();

        public IPlayerInput GetInput()
        {
            return input;
        }
    }
}
