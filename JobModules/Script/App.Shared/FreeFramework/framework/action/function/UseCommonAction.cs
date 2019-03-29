using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.action.function;
using com.wd.free.@event;
using com.wd.free.exception;
using com.wd.free.util;
using gameplay.gamerule.free.action;
using UnityEngine;
using com.wd.free.debug;

namespace com.wd.free.action.function
{
    [Serializable]
    public class UseCommonAction : AbstractGameAction
    {
        public String key;
        public List<ArgValue> values;

        public override void DoAction(IEventArgs args)
        {
            if (values == null)
            {
                values = new List<ArgValue>();
            }

            foreach (CommonGameAction function in args.Functions)
            {
                if (function.GetKey() == FreeUtil.ReplaceVar(key, args))
                {
                    GameFunc func = function.ToGameFunc();
                    checkArg(func);

                    try
                    {
                        //long mem = GC.GetTotalMemory(false);
                        func.Action(values, args);
                        //Debug.LogFormat("call func {0}, args:{1}, memory:{2}", key, ToArgString(), GC.GetTotalMemory(false) - mem);
                        if (FreeLog.IsEnable())
                        {
                            FreeLog.CallFunc(string.Format("call func:{0}, args:{1}", key, ToArgString()));
                        }
                    }
                    catch (Exception e)
                    {
                        FreeLog.Error("call func " + key + " failed.\n"
                                     + ExceptionUtil.GetExceptionContent(e), this);
                    }

                    break;
                }
            }
        }

        private string ToArgString()
        {
            List<string> list = new List<string>();
            foreach (ArgValue av in values)
            {
                list.Add(av.ToString());
            }
            return string.Join(",", list.ToArray());
        }

        private void checkArg(GameFunc func)
        {
            if (values != null)
            {
                foreach (ArgValue av in values)
                {
                    bool has = false;
                    foreach (FuncArg fa in func.GetArgs())
                    {
                        if (av.GetName() == fa.GetName())
                        {
                            has = true;
                            break;
                        }
                    }
                    if (!has)
                    {
                        throw new GameConfigExpception("func '" + func.GetKey()
                                                                + "(" + func.GetName() + ")' does not have arg '"
                                                                + av.GetName() + "'");
                    }
                }
            }
        }

        public override String ToString()
        {
            if (values != null)
            {
                return "call " + key + ":" + values.ToString();
            }
            else
            {
                return "call " + key;
            }
        }
    }
}
