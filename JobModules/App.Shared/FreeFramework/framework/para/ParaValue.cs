using System;
using Sharpen;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.@event;
using com.wd.free.util;

namespace com.wd.free.para
{
    [System.Serializable]
    public class ParaValue
    {
        private const long serialVersionUID = 5260989621718192497L;

        private string name;

        private string type;

        private bool isPublic;

        private string desc;

        private string value;

        public virtual string GetName()
        {
            return name;
        }

        public virtual void SetName(string name)
        {
            this.name = name;
        }

        public virtual string GetType()
        {
            return type;
        }

        public virtual void SetType(string type)
        {
            this.type = type;
        }

        public virtual string GetValue()
        {
            return value;
        }

        public virtual void SetValue(string value)
        {
            this.value = value;
        }

        public virtual string GetDesc()
        {
            return desc;
        }

        public virtual void SetDesc(string desc)
        {
            this.desc = desc;
        }

        public virtual IPara GetPara(IEventArgs args)
        {
            IPara para = ParaUtil.GetPara(type);
            if (para != null)
            {
                para = (IPara)para.Copy();
                para.SetName(name);
                if (value == null)
                {
                    value = string.Empty;
                }

                if (para is StringPara)
                {
                    ((StringPara)para).SetValue(FreeUtil.ReplaceVar(value, args));
                }
                else if (para is BoolPara)
                {
                    ((BoolPara)para).SetValue(FreeUtil.ReplaceBool(value, args));
                }
                else
                {
                    double v = FreeUtil.ReplaceDouble(value, args);

                    if (para is IntPara)
                    {
                        ((IntPara)para).SetValue((int)v);
                    }
                    else
                    {
                        if (para is FloatPara)
                        {
                            ((FloatPara)para).SetValue((float)v);
                        }
                        else
                        {
                            if (para is DoublePara)
                            {
                                ((DoublePara)para).SetValue(v);
                            }
                        }
                    }
                }
                para.SetPublic(isPublic);
                para.SetName(FreeUtil.ReplaceVar(para.GetName(), args));
            }

            return para;
        }
    }
}
