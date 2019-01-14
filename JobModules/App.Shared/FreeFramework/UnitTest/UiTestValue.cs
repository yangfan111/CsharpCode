using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public class UiTestValue : AbstractTestValue
    {
        public string ui;

        public override TestValue GetCaseValue(IEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
