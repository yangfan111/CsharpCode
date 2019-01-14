using com.wd.free.@event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.UnitTest
{
    public interface ITestValue
    {
        string Name { get; }
        TestValue GetCaseValue(IEventArgs args);
    }
}
