using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.util;
using Free.framework;
using gameplay.gamerule.free.ui.component;
using Core.Free;
using System;

namespace gameplay.gamerule.free.ui
{
    [System.Serializable]
    public class FreeUiAddChildAction : SendMessageAction
    {
        public string key;

        public string parent;

        public string clear;

        public string eventKey;

        private List<OnePrefabValue> values;
        private List<OnePrefabEvent> events;

        public FreeUiAddChildAction()
            : base()
        {
            this.values = new List<OnePrefabValue>();
            this.events = new List<OnePrefabEvent>();
        }

        public List<OnePrefabValue> GetValues()
        {
            return values;
        }

        public virtual string GetKey()
        {
            return key;
        }

        public virtual void SetKey(string key)
        {
            this.key = key;
        }

        protected override void BuildMessage(IEventArgs args)
        {
            builder.Key = FreeMessageConstant.AddChild;

            builder.Bs.Add(FreeUtil.ReplaceBool(clear, args));

            builder.Ss.Add(FreeUtil.ReplaceVar(key, args));
            builder.Ss.Add(FreeUtil.ReplaceVar(parent, args));

            builder.Ss.Add(ValueString(args));
            builder.Ss.Add(EventString(args));
            if (string.IsNullOrEmpty(eventKey))
            {
                builder.Ss.Add("");
            }
            else
            {
                builder.Ss.Add(eventKey);
            }
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

        private static string GetAutoString(IFreeComponent fc, IEventArgs args)
        {
            IList<string> list = new List<string>();
            if (fc.GetAutos(args) != null)
            {
                foreach (IFreeUIAuto auto in fc.GetAutos(args))
                {
                    list.Add(auto.GetField() + "=" + auto.ToConfig(args));
                }
            }
            return StringUtil.GetStringFromStrings(list, "|||");
        }

        public override string ToString()
        {
            return this.GetType().Name + " " + key;
        }

        public override string GetMessageDesc()
        {
            string d = string.Empty;
            if (!StringUtil.IsNullOrEmpty(desc))
            {
                d = "(" + desc + ")";
            }
            return "子UI'" + key + d + "'" + "'\n" + builder.ToString();
        }

    }
}
