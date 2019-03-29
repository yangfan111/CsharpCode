using System;
using System.Collections.Generic;
using com.cpkf.yyjd.tools.util;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Free;

namespace gameplay.gamerule.free.ui.component
{
    [System.Serializable]
    public class FreePrefabValue : AbstractFreeUIValue
    {
        private List<OnePrefabValue> values;

        public FreePrefabValue()
        {
            values = new List<OnePrefabValue>();
        }

        public void Clear()
        {
            this.values.Clear();
        }

        public void AddOneValue(OnePrefabValue value)
        {
            this.values.Add(value);
        }

        public override int GetType()
        {
            return STRING;
        }

        private List<string> list;

        public override object GetValue(IEventArgs args)
        {
            if (list == null)
            {
                list = new List<string>();
            }
            list.Clear();

            foreach (OnePrefabValue value in values)
            {
                list.Add(value.field + FreeMessageConstant.SpilterField + FreeUtil.ReplaceVar(value.value, args));
            }

            return StringUtil.GetStringFromStrings(list, FreeMessageConstant.SpliterRecord);
        }

        public override string ToString()
        {
            return "ui prefab value:" + values;
        }
    }

    [Serializable]
    public class OnePrefabValue
    {
        public string field;
        public string value;

        public OnePrefabValue()
        {

        }

        public OnePrefabValue(string field, string value)
        {
            this.field = field;
            this.value = value;
        }
    }

    [Serializable]
    public class OnePrefabEvent
    {
        public string field;
        public string eventKey;
        public int eventType;
    }
}
