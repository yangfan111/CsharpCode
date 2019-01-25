using System;
using System.Reflection;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.exception;
using com.wd.free.para;

namespace com.wd.free.action
{
	/// <summary>将source中指定的字段值，以IPara的形式加入到target中</summary>
	/// <author>Dave Yang</author>
	[System.Serializable]
	public abstract class AbstractReflectAddAction : AbstractGameAction
	{
		private const long serialVersionUID = 2800918075257274831L;

		protected internal string fields;

		public override void DoAction(IEventArgs args)
		{
			ParaList target = GetTarget(args);
			object source = GetSource(args);
			if (fields != null && source != null && target != null)
			{
				foreach (FieldPair fN in FieldPair.Parse(fields))
				{
					string[] ffs = new string[2];
					ffs[0] = fN.GetFrom();
					ffs[1] = fN.GetTo();
					try
					{
						FieldInfo f = ReflectionCache.GetField(source, ffs[0].Trim());
						AbstractPara para = null;
						string type = f.GetType().Name.ToLower();
						if ("long".Equals(type))
						{
							para = new LongPara(ffs[1].Trim());
						}
						if ("int".Equals(type))
						{
							para = new IntPara(ffs[1].Trim());
						}
						if ("float".Equals(type))
						{
							para = new FloatPara(ffs[1].Trim());
						}
						if ("double".Equals(type))
						{
							para = new DoublePara(ffs[1].Trim());
						}
						if ("string".Equals(type))
						{
							para = new StringPara(ffs[1].Trim());
						}
						if ("boolean".Equals(type))
						{
							para = new BoolPara(ffs[1].Trim());
						}
						if (para == null)
						{
							throw new GameConfigExpception(ffs[1].Trim() + "'s type '" + type + "' is not supported.");
						}
						
						para.SetValue(f.GetValue(source));
						
						target.AddPara(para);
					}
					catch (Exception e)
					{
						throw new GameConfigExpception(fN + " is not a valid field.\n" + ExceptionUtil.GetExceptionContent(e));
					}
				}
			}
		}

		public abstract ParaList GetTarget(IEventArgs args);

		public abstract object GetSource(IEventArgs args);
	}
}
