using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.skill;
using Core.Prediction.UserPrediction.Cmd;

namespace com.wd.free
{
    public class TestPlayerInput : IPlayerInput
    {
        public bool pressed;

        public bool IsPressed(int key)
        {
            return pressed;
        }

        public void SetUserCmd(IUserCmd cmd)
        {
            
        }
    }
}
