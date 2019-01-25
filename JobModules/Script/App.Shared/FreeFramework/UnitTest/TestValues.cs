using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.UnitTest
{
    public class TestValues
    {
        private List<TestValue> values;

        public TestValues()
        {
            this.values = new List<TestValue>();
        }

        public void AddTestValue(TestValue tv)
        {
            this.values.Add(tv);
        }



    }
}
