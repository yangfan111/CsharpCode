using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using Free.framework;
using System.Collections.Generic;
using com.cpkf.yyjd.tools.util;
using Core.Free;

namespace gameplay.gamerule.free.ui.component
{
    [System.Serializable]
    public class FreePrefabComponet : AbstractFreeComponent
    {
        private string name;

        private List<OnePrefabValue> values;
        private List<OnePrefabEvent> events;

        public override int GetKey(IEventArgs args)
        {
            return Prefab;
        }

        public override string GetStyle(IEventArgs args)
        {
            return FreeUtil.ReplaceVar(name, args) + FreeMessageConstant.SpliterStyle 
                + ValueString(args) + FreeMessageConstant.SpliterStyle + EventString(args);
        }

        private string EventString(IEventArgs args)
        {
            List<string> list = new List<string>();

            if (events != null)
            {
                foreach (OnePrefabEvent v in events)
                {
                    list.Add(FreeUtil.ReplaceVar(v.field, args) + FreeMessageConstant.SpilterField 
                        + FreeUtil.ReplaceVar(v.eventKey, args) + FreeMessageConstant.SpilterField + v.eventType);
                }
            }

            return StringUtil.GetStringFromStrings(list, FreeMessageConstant.SpliterRecord);
        }

        private string ValueString(IEventArgs args)
        {
            List<string> list = new List<string>();

            if (values != null)
            {
                foreach (OnePrefabValue v in values)
                {
                    list.Add(FreeUtil.ReplaceVar(v.field, args) + FreeMessageConstant.SpilterField + FreeUtil.ReplaceVar(v.value, args));
                }
            }

            return StringUtil.GetStringFromStrings(list, FreeMessageConstant.SpliterRecord);
        }

        public override IFreeUIValue GetValue()
        {
            return new FreePrefabValue();
        }

        public override SimpleProto CreateChildren(IEventArgs args)
        {
            SimpleProto b = FreePool.Allocate();
            b.Key = 1;

            return b;
        }
    }
}
