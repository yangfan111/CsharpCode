using System;
using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.util;

namespace com.wd.free.action.function
{
	[System.Serializable]
	public class GameFunc
	{
		private const long serialVersionUID = -8123300665766113596L;

		private string key;

		private string name;

		private string desc;

		private IList<FuncArg> args;

		private IGameAction action;

		public GameFunc()
		{
			this.args = new List<FuncArg>();
		}

		private ArgValue GetArgValue(FuncArg arg, IList<ArgValue> values, IEventArgs args)
		{
			if (values != null)
			{
				foreach (ArgValue av in values)
				{
					if (arg.GetName().Equals(av.GetName()))
					{
						return av;
					}
				}
			}
			return new ArgValue(arg.GetName(), FreeUtil.ReplaceVar(arg.GetDefaultValue(), args));
		}

		public virtual void Action(IList<ArgValue> funcArgs, IEventArgs args)
		{
			long s = FreeTimeDebug.RecordStart("call " + name + "(" + key + ")");
			FreeLog.CallFunc("call " + name + "(" + key + "):");
			if (funcArgs != null)
			{
				foreach (FuncArg arg in this.args)
				{
					SetArg(arg, funcArgs, args);
				}
			}
			if (action != null)
			{
				action.Act(args);
			}
			if (funcArgs != null)
			{
				foreach (FuncArg arg in this.args)
				{
					RemoveArg(arg, args);
				}
			}
			FreeTimeDebug.RecordEnd("call " + name + "(" + key + ")", s);
		}

		private void SetArg(FuncArg arg, IList<ArgValue> funcArgs, IEventArgs args)
		{
			ArgValue fa = GetArgValue(arg, funcArgs, args);
			if (fa != null)
			{
				IPara para = ParaUtil.GetPara(arg.GetType());
				if (para != null)
				{
					try
					{
						IPara old = new ParaExp(fa.GetValue()).GetSourcePara(args);
						if (old != null)
						{
							if (old.GetValue() != null)
							{
								para = para.Initial("=", old.GetValue().ToString());
							}
							else
							{
								para = para.Initial("=", string.Empty);
							}
						}
						else
						{
							para = para.Initial("=", FreeUtil.ReplaceNumber(fa.GetValue(), args));
						}
					}
					catch (Exception)
					{
						para = para.Initial("=", FreeUtil.ReplaceNumber(fa.GetValue(), args));
					}
					para.SetName("arg_" + arg.GetName());
					args.GetDefault().GetParameters().TempUse(para);
					IPara p = (IPara)para.Borrow();
					p.SetName(arg.GetName());
					if (para.GetValue() != null)
					{
						p.SetValue("=", para);
					}
					args.GetDefault().GetParameters().TempUse(p);
					FreeLog.FuncArg(p.ToString());
				}
				else
				{
					// 非简单变量
					((BaseEventArgs)args).TempUse(arg.GetName(), args.GetUnit(fa.GetValue()));
				}
			}
		}

		private void RemoveArg(FuncArg fa, IEventArgs args)
		{
			if (fa != null)
			{
				string t = fa.GetType();
				if ("string".Equals(t) || "int".Equals(t) || "bool".Equals(t) || "float".Equals(t) || "long".Equals(t) || "double".Equals(t))
				{
					IPara p = args.GetDefault().GetParameters().Get("arg_" + fa.GetName());
					p.Recycle();
					p = args.GetDefault().GetParameters().Get(fa.GetName());
					p.Recycle();
					args.GetDefault().GetParameters().Resume("arg_" + fa.GetName());
					args.GetDefault().GetParameters().Resume(fa.GetName());
				}
				else
				{
					((BaseEventArgs)args).Resume(fa.GetName());
				}
			}
		}

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void SetKey(string key)
		{
			this.key = key;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual string GetDesc()
		{
			return desc;
		}

		public virtual void SetDesc(string desc)
		{
			this.desc = desc;
		}

		public virtual IList<FuncArg> GetArgs()
		{
			return args;
		}

		public virtual void SetArgs(IList<FuncArg> args)
		{
			this.args = args;
		}

		public virtual IList<FuncArg> GetParas()
		{
			return args;
		}

		public virtual IGameAction GetAction()
		{
			return action;
		}

		public virtual void SetAction(IGameAction action)
		{
			this.action = action;
		}
	}
}
