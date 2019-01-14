using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.exception;
using com.wd.free.para;

namespace com.wd.free.para.exp
{
    public class UnitPara
    {
        private const string DOT = ".";

        private string unit;

        private string para;

        private static Dictionary<int, Dictionary<string, string[]>> splitCache = new Dictionary<int, Dictionary<string, string[]>>();

        public UnitPara()
            : base()
        {
        }

        public virtual string GetUnit()
        {
            return unit;
        }

        public virtual void SetUnit(string unit)
        {
            this.unit = unit;
        }

        public virtual string GetPara()
        {
            return para;
        }

        public virtual void SetPara(string para)
        {
            this.para = para;
        }

        public virtual IPara GetPara(IEventArgs args)
        {
            IParable parable = args.GetUnit(unit);
            if (parable != null)
            {
                ParaList pl = args.GetUnit(unit).GetParameters();
                if (pl.HasPara(para))
                {
                    return pl.Get(para);
                }
                else
                {
                    string[] vs = Split(GetPara(), 1);
                    if (para.Contains("[") && para.Contains("]"))
                    {
                        vs = new string[] { para };
                    }
                    if (vs.Length != 4)
                    {
                        if (para.Contains("[@") && para.Contains("']/@"))
                        {
                            vs = Split(GetPara(), 2);
                        }
                    }
                    if (vs.Length == 4)
                    {
                        IPara p = pl.Get(vs[0].Trim());
                        if (p != null && p is ParaListSet)
                        {
                            ParaListSet pls = (ParaListSet)p;
                            ParaList sub = pls.GetParaList(vs[1].Trim(), vs[2].Trim(), args);
                            if (sub != null && sub.HasPara(vs[3].Trim()))
                            {
                                return sub.Get(vs[3].Trim());
                            }
                        }
                    }
                    else
                    {
                        if (para.Contains("[") && para.Contains("]"))
                        {
                            vs = Split(para, 3);
                            if (vs.Length == 3 && StringUtil.IsNullOrEmpty(vs[2]))
                            {
                                IPara p = pl.Get(vs[0].Trim());
                                if (p != null && (p is MapPara || p is StringPara))
                                {
                                    if (p is MapPara)
                                    {
                                        MapPara mp = (MapPara)p;
                                        if (vs[1].Trim().Equals("length"))
                                        {
                                            return new IntPara(p.GetName(), mp.Size());
                                        }
                                        return mp.GetValue(vs[1].Trim());
                                    }
                                    if (p is StringPara)
                                    {
                                        if ("length".Equals(vs[1].Trim()))
                                        {
                                            return new IntPara(p.GetName(), p.GetValue().ToString().Length);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return null;
            }
            else
            {
                throw new GameConfigExpception("unit " + unit + " is not existed.\n" + GetUnit() + "." + GetPara());
            }
        }

        private static string[] Split(string v, int type)
        {
            if (!splitCache.ContainsKey(type))
            {
                splitCache.Add(type, new Dictionary<string, string[]>());
            }

            if (!splitCache[type].ContainsKey(v))
            {
                switch (type)
                {
                    case 1:
                        splitCache[type].Add(v, StringUtil.Split(v, "_"));
                        break;
                    case 2:
                        splitCache[type].Add(v, StringUtil.Split(v, new string[] { "[@", "='", "']/@" }));
                        break;
                    case 3:
                        splitCache[type].Add(v, StringUtil.Split(v, new string[] { "[", "]" }));
                        break;
                    case 4:
                        splitCache[type].Add(v, StringUtil.Split(v, DOT));
                        break;
                    case 5:
                        splitCache[type].Add(v, StringUtil.Split(v, ","));
                        break;
                    default:
                        break;
                }
            }

            return splitCache[type][v];
        }

        public static com.wd.free.para.exp.UnitPara ParseOne(string exp)
        {
            string[] vs = Split(exp,4);
            com.wd.free.para.exp.UnitPara fp = new com.wd.free.para.exp.UnitPara();
            if (vs.Length == 2)
            {
                fp.unit = vs[0].Trim();
                fp.para = vs[1].Trim();
                if (fp.unit.Equals("0"))
                {
                    fp.unit = string.Empty;
                    fp.para = exp.Trim();
                }
            }
            else
            {
                if (vs.Length == 1)
                {
                    fp.unit = BaseEventArgs.DEFAULT;
                    fp.para = vs[0].Trim();
                }
                else
                {
                    if (vs.Length == 3)
                    {
                        fp.unit = vs[0].Trim();
                        fp.para = vs[1].Trim() + "." + vs[2].Trim();
                    }
                }
            }
            return fp;
        }

        public static com.wd.free.para.exp.UnitPara[] Parse(string fields)
        {
            List<com.wd.free.para.exp.UnitPara> list = new List<com.wd.free.para.exp.UnitPara>();
            if (!StringUtil.IsNullOrEmpty(fields))
            {
                foreach (string f in Split(fields,5))
                {
                    string temp = f.Trim();
                    string[] vs = Split(temp,4);
                    com.wd.free.para.exp.UnitPara fp = new com.wd.free.para.exp.UnitPara();
                    if (vs.Length == 2)
                    {
                        fp.unit = vs[0].Trim();
                        fp.para = vs[1].Trim();
                    }
                    else
                    {
                        if (vs.Length == 1)
                        {
                            fp.unit = BaseEventArgs.DEFAULT;
                            fp.para = vs[0].Trim();
                        }
                    }
                    list.Add(fp);
                }
            }
            return list.ToArray();
        }

        public override string ToString()
        {
            return unit + "." + para;
        }
    }
}
