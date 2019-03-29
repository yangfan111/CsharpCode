using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.util;
using Free.framework;
using gameplay.gamerule.free.ui.component;
using Core.Free;

namespace gameplay.gamerule.free.ui
{
    [System.Serializable]
    public class FreeUICreateAction : SendMessageAction
    {
        private const long serialVersionUID = -1188091456904593121L;

        private string key;

        private bool show;

        private int layer;

        private bool atBottom;

        private IList<IFreeComponent> components;

        public FreeUICreateAction()
            : base()
        {
            this.components = new List<IFreeComponent>();
        }

        public virtual bool IsShow()
        {
            return show;
        }

        public virtual void SetShow(bool show)
        {
            this.show = show;
        }

        public virtual string GetKey()
        {
            return key;
        }

        public virtual void SetKey(string key)
        {
            this.key = key;
        }

        public virtual IList<IFreeComponent> GetComponents()
        {
            return components;
        }

        public static void Build(SimpleProto b, IEventArgs args, string key, string desc, bool show, bool atBottom, Iterable<IFreeComponent> components, int layer = 0)
        {
            b.Key = 50;

            b.Bs.Add(show);
            b.Bs.Add(atBottom);
            b.Ks.Add(layer);

            if (desc == null)
            {
                desc = "";
            }

            b.Ss.Add(FreeUtil.ReplaceVar(key, args));
            b.Ss.Add(FreeUtil.ReplaceVar(desc, args));
            if (components != null)
            {
                foreach (IFreeComponent com in components)
                {
                    b.Ks.Add(com.GetKey(args));
                    b.Ins.Add(FreeUtil.ReplaceInt(com.GetWidth(args), args));
                    b.Ins.Add(FreeUtil.ReplaceInt(com.GetHeight(args), args));
                    b.Ins.Add(FreeUtil.ReplaceInt(com.GetRelative(args), args));
                    b.Ins.Add(FreeUtil.ReplaceInt(com.GetParent(args), args));
                    b.Fs.Add(FreeUtil.ReplaceFloat(com.GetX(args), args));
                    b.Fs.Add(FreeUtil.ReplaceFloat(com.GetY(args), args));
                    if (com.GetStyle(args) == null)
                    {
                        b.Ss.Add(string.Empty);
                    }
                    else
                    {
                        b.Ss.Add(com.GetStyle(args));
                    }
                    b.Ss.Add(GetAutoString(com, args));
                    if (com.GetEvent(args) != null)
                    {
                        b.Ss.Add(com.GetEvent(args));
                    }
                    else
                    {
                        b.Ss.Add(string.Empty);
                    }

                    SimpleProto child = com.CreateChildren(args);
                    b.Ps.Add(child);
                }
            }
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

        protected override void BuildMessage(IEventArgs args)
        {
            Build(builder, args, key, desc, show, atBottom, components.AsIterable(), layer);
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
            return "创建UI'" + key + d + "'" + "'\n" + builder.ToString();
        }
    }
}
