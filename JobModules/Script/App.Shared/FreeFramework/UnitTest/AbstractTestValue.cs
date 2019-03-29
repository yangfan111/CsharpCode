using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;

namespace App.Shared.FreeFramework.UnitTest
{
    [Serializable]
    public abstract class AbstractTestValue : ITestValue
    {
        private string name;

        public string Name { get { return name; } }

        public abstract TestValue GetCaseValue(IEventArgs args);
    }
}
