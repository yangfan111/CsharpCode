using com.wd.free.para.exp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace App.Server.GameModules.GamePlay.Free.condition
{
    public class UseCodeCondition : IParaCondition
    {
        private string code;
        private string paras;

        public bool Meet(IEventArgs args)
        {
            if("CanAddToBag" == code)
            {

                return true;
            }

            return false;
        }
    }
}
