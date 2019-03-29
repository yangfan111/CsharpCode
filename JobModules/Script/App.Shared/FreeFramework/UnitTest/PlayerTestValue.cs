using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.para;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public class PlayerTestValue : AbstractTestValue
    {
        private string field;

        public override TestValue GetCaseValue(IEventArgs args)
        {
            TestValue tv = new TestValue();
            FreeData fd = (FreeData)args.GetUnit(UnitTestConstant.Tester);
            if (fd != null)
            {
                IPara para = fd.GetParameters().Get(field);
                if (para != null)
                {
                    tv.AddField(field, para.GetValue().ToString());
                }
            }

            return tv;
        }
    }
}
