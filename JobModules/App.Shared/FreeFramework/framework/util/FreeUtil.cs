using System;
using System.Collections.Generic;
using System.Text;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.@event;
using com.wd.free.exception;
using com.wd.free.para;
using com.wd.free.para.exp;

namespace com.wd.free.util
{
	public class FreeUtil
	{
		private static MyDictionary<string, double> fMap = new MyDictionary<string, double>();

		private static LinkedHashMap<string, string> ncache;

		private static LinkedHashMap<string, double> doubleCache;

        private static Dictionary<string, IList<FreeUtil.VarText>> splitCache;

		private static IList<IFreeReplacer> replacers;

		public const string VAR_START = "{";

		public const string VAR_END = "}";

		public static string EMPTY_STRING = string.Empty;

		private const string TRUE = "true";
        private const string OtherTrue = "True";

		static FreeUtil()
		{
			ncache = new LinkedHashMap<string, string>();
			doubleCache = new LinkedHashMap<string, double>();
			replacers = new List<IFreeReplacer>();
			replacers.Add(new FreeRandomReplacer());
            splitCache = new Dictionary<string, IList<FreeUtil.VarText>>();
		}

		public static void RegisterReplacer(IFreeReplacer replacer)
		{
			replacers.Add(replacer);
		}

		public static int ReplaceInt(string message, IEventArgs args)
		{
			return (int)GetDouble(message, args);
		}

		public static bool ReplaceBool(string message, IEventArgs args)
		{
            string v = ReplaceVar(message, args);
			return TRUE == v || OtherTrue == v;
		}

		public static float ReplaceFloat(string message, IEventArgs args)
		{
			return (float)GetDouble(message, args);
		}

		public static double ReplaceDouble(string message, IEventArgs args)
		{
			return GetDouble(message, args);
		}

        public static string ReplaceString(string message, IEventArgs args)
        {
            return ReplaceVar(message, args);
        }

        public static bool IsVar(string value)
        {
            return value != null && value.Contains(VAR_START) && value.Contains(VAR_END);
        }

		public static string ReplaceNumber(string message, IEventArgs args)
		{
			string nv = message;
			if (message != null && message.Contains(VAR_START) && message.Contains(VAR_END))
			{
				nv = ReplaceVar(message, args);
				if (nv == null)
				{
					return nv;
				}
			}
			// long s = FreeTimeDebug.recordStart("NumberUtil");
			string num = GetNumber(nv);
			// FreeTimeDebug.recordEnd("NumberUtil", s);
			return num;
		}

		private static double GetDouble(string nv, IEventArgs args)
		{
			nv = ReplaceVar(nv, args);
			if (nv != null && StringUtil.ContainsNumber(nv))
			{
				try
				{
					return double.Parse(nv);
				}
				catch (Exception)
				{
					if (!doubleCache.ContainsKey(nv))
					{
						double r = 0d;
						try
						{
							string v = FormulaUtil.GetValue(nv, fMap).ToString();
							r = double.Parse(v);
						}
						catch (Exception)
						{
						}
						doubleCache[nv] = r;
						if (doubleCache.Count > 10000)
						{
							doubleCache.Remove(doubleCache.Keys.Iterator().Next());
						}
					}
					return doubleCache[nv];
				}
			}
			else
			{
				return 0d;
			}
		}

		private static string GetNumber(string nv)
		{
			if (StringUtil.ContainsNumber(nv))
			{
				try
				{
					double d = double.Parse(nv);
					if (d == d)
					{
						return d.ToString();
					}
					else
					{
						return d.ToString();
					}
				}
				catch (Exception)
				{
					if (!ncache.ContainsKey(nv))
					{
						string r = null;
						try
						{
							string v = FormulaUtil.GetValue(nv, fMap).ToString();
							double d = double.Parse(v);
							if (d == d)
							{
								r = d.ToString();
							}
							else
							{
								r = d.ToString();
							}
						}
						catch (Exception)
						{
							r = nv;
						}
						ncache[nv] = r;
						if (ncache.Count > 1000)
						{
							ncache.Remove(ncache.Keys.Iterator().Next());
						}
					}
					return ncache[nv];
				}
			}
			else
			{
				return nv;
			}
		}

		public static string ReplaceVar(string message, IEventArgs args)
		{
			// long s = FreeTimeDebug.recordStart("replace");
			if (message == null || !message.Contains(VAR_START) || !message.Contains(VAR_END))
			{
				return message;
			}
			if (StringUtil.IsNullOrEmpty(message))
			{
				return message;
			}
			if ("null".Equals(message))
			{
				return string.Empty;
			}
			string var = FreeUtil.VarText.Replace(message, args);
			// FreeTimeDebug.recordEnd("replace", s);
			return var;
		}

		// 只有一层的变量,速度快
		private static string ReplaceOneLevelVar(string message, IEventArgs args)
		{
			string nv = message;
			string[] vs = GetVars(message);
			if (vs.Length > 0)
			{
				string temp = message;
				foreach (string v in vs)
				{
					IPara pa = new ParaExp(v).GetSourcePara(args);
					if (pa != null)
					{
						object va = pa.GetValue();
						if (va != null)
						{
							temp = temp.Replace(VAR_START + v + VAR_END, va.ToString());
						}
					}
				}
				nv = temp;
			}
			return nv;
		}

		private static string[] GetVars(string message)
		{
			if (StringUtil.IsNullOrEmpty(message))
			{
				return new string[0];
			}
			IList<string> vs = new List<string>();
			int[] ss = StringUtil.GetAllIndex(message, VAR_START);
			int[] es = StringUtil.GetAllIndex(message, VAR_END);
			if (ss.Length == es.Length)
			{
				for (int i = 0; i < ss.Length; i++)
				{
					if (ss[i] < es[i])
					{
						vs.Add(Sharpen.Runtime.Substring(message, ss[i] + 1, es[i]));
					}
				}
			}
			return Sharpen.Collections.ToArray(vs, new string[0]);
		}

		public class VarText
		{
			public string text;

			public bool var;

			public VarText(string text, bool var)
				: base()
			{
				this.text = text;
				this.var = var;
			}

			public static string Replace(string text, IEventArgs args)
			{
				IList<FreeUtil.VarText> list = ToVarText(text);
				StringBuilder sb = new StringBuilder();
				foreach (FreeUtil.VarText vt in list)
				{
					sb.Append(vt.Replace(args, false));
				}
				return sb.ToString();
			}

			private string Replace(IEventArgs args, bool once)
			{
				if (!var)
				{
					return text;
				}
				else
				{
					IList<FreeUtil.PosIndex> list = GetPos(text, '{', '}');
					if (list.Count == 0 || once)
					{
						IPara pa = new ParaExp(text).GetSourcePara(args);
						if (pa != null)
						{
							object va = pa.GetValue();
							if (va != null)
							{
								return va.ToString();
							}
						}
						string v = HandleSpecial(text, args);
						if (v != null)
						{
							return v;
						}
						StringBuilder sb = new StringBuilder();
						sb.Append(VAR_START);
						sb.Append(text);
						sb.Append(VAR_END);
						return sb.ToString();
					}
					else
					{
						IList<FreeUtil.VarText> subs = ToVarText(text);
						StringBuilder sb = new StringBuilder();
						foreach (FreeUtil.VarText vt in subs)
						{
							sb.Append(vt.Replace(args, false));
						}
						FreeUtil.VarText newVt = new FreeUtil.VarText(sb.ToString(), true);
						return newVt.Replace(args, true);
					}
				}
			}

			private string HandleSpecial(string text, IEventArgs args)
			{
				foreach (IFreeReplacer replacer in replacers)
				{
					if (replacer.CanHandle(text, args))
					{
						return replacer.Replace(text, args);
					}
				}
				return null;
			}

			private static IList<FreeUtil.VarText> ToVarText(string text)
			{
                if (!splitCache.ContainsKey(text))
                {
                    IList<FreeUtil.VarText> vars = new List<FreeUtil.VarText>();
                    IList<FreeUtil.PosIndex> list = GetPos(text, '{', '}');
                    if (list.Count == 0)
                    {
                        vars.Add(new FreeUtil.VarText(text, false));
                        return vars;
                    }
                    FreeUtil.PosIndex lastEnd = null;
                    for (int i = 0; i < list.Count - 1; i = i + 2)
                    {
                        FreeUtil.PosIndex spi = list[i];
                        FreeUtil.PosIndex epi = list[i + 1];
                        if (lastEnd != null)
                        {
                            vars.Add(new FreeUtil.VarText(Sharpen.Runtime.Substring(text, lastEnd.index + 1, spi.index), false));
                        }
                        else
                        {
                            if (i == 0 && spi.index > 0)
                            {
                                vars.Add(new FreeUtil.VarText(Sharpen.Runtime.Substring(text, 0, spi.index), false));
                            }
                        }
                        vars.Add(new FreeUtil.VarText(Sharpen.Runtime.Substring(text, spi.index + 1, epi.index), true));
                        if (i + 1 == list.Count - 1)
                        {
                            if (epi.index < text.Length - 1)
                            {
                                vars.Add(new FreeUtil.VarText(Sharpen.Runtime.Substring(text, epi.index + 1, text.Length), false));
                            }
                        }
                        lastEnd = epi;
                    }

                    splitCache.Add(text, vars);
                }
				
				return splitCache[text];
			}
		}

		public static IList<FreeUtil.PosIndex> GetPos(string text, char start, char end)
		{
			IList<FreeUtil.PosIndex> list = new List<FreeUtil.PosIndex>();
			char[] cs = text.ToCharArray();
			for (int i = 0; i < cs.Length; i++)
			{
				if (start == cs[i])
				{
					FreeUtil.PosIndex pi = new FreeUtil.PosIndex(i, 0, true);
					list.Add(pi);
				}
				else
				{
					if (end == cs[i])
					{
						FreeUtil.PosIndex pi = new FreeUtil.PosIndex(i, 0, false);
						list.Add(pi);
					}
				}
			}
			if (list.Count == 0)
			{
				return list;
			}
			int max = 0;
			int count = 0;
			foreach (FreeUtil.PosIndex pi_1 in list)
			{
				if (pi_1.start)
				{
					count++;
				}
				pi_1.deep = count;
				if (count > max)
				{
					max = count;
				}
				if (!pi_1.start)
				{
					count--;
				}
			}
			if (max == 0 && list.Count > 0)
			{
				throw new GameConfigExpception("错误格式‘" + text + "’");
			}
			int status = 0;
			IList<FreeUtil.PosIndex> deepList = new List<FreeUtil.PosIndex>();
			for (int j = 0; j < list.Count; j++)
			{
				FreeUtil.PosIndex pi = list[j];
				if (pi.deep == 1)
				{
					if (status == 0)
					{
						if (pi.start)
						{
							deepList.Add(pi);
							status = 1;
						}
						else
						{
							throw new GameConfigExpception("错误格式‘" + text + "’");
						}
					}
					else
					{
						if (status == 1)
						{
							if (!pi.start)
							{
								deepList.Add(pi);
								status = 0;
							}
							else
							{
								throw new GameConfigExpception("错误格式‘" + text + "’");
							}
						}
					}
				}
			}
			if (deepList.Count < 2 || deepList.Count % 2 != 0)
			{
				throw new GameConfigExpception("错误格式‘" + text + "’");
			}
			return deepList;
		}

		public class PosIndex
		{
			public int index;

			public int deep;

			public bool start;

			public PosIndex(int index, int deep, bool start)
				: base()
			{
				this.index = index;
				this.deep = deep;
				this.start = start;
			}

			public override string ToString()
			{
				return index + "," + deep + "," + start;
			}
		}
	}
}
