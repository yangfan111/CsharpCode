using com.wd.free.action.function;
using com.wd.free.@event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.framework.util
{
    public class FuntionUtil
    {
        public static void Call(IEventArgs args, string func, params string[] paras)
        {
            GetFunc(func, paras).Act(args);
        }

        public static UseCommonAction GetFunc(string func, params string[] paras)
        {
            UseCommonAction use = new UseCommonAction();
            use.key = func;
            use.values = new List<ArgValue>();
            for (int i = 0; i < paras.Length; i = i + 2)
            {
                use.values.Add(new ArgValue(paras[i].Trim(), paras[i + 1].Trim()));
            }

            return use;
        }
    }
}
