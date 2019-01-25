using System;
using System.Reflection;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.exception;
using com.wd.free.para;

namespace com.wd.free.action
{
	/// <summary>对指定的Object，以Para的op方式进行赋值</summary>
	/// <author>Dave Yang</author>
	[System.Serializable]
	public abstract class AbstractReflectSetAction : AbstractGameAction
	{
		private const long serialVersionUID = -8752394837539763152L;

		private string fields;

		private string values;

		public override void DoAction(IEventArgs args)
		{
			object target = GetTarget(args);
			if (fields != null && values != null)
			{
				string[] fs = StringUtil.Split(fields, ",");
				string[] vs = StringUtil.Split(values, ",");
				if (fs.Length == vs.Length)
				{
					for (int i = 0; i < fs.Length; i++)
					{
						try
						{
							string fName = fs[i].Trim();
							FieldInfo f = ReflectionCache.GetField(target, fName);
							string type = f.GetType().Name.ToLower();
							IPara p = null;
							string[] ss = StringUtil.Split(vs[i].Trim(), ".");
							if (ss.Length == 2)
							{
								if (args.GetUnit(ss[0].Trim()) != null)
								{
									p = args.GetUnit(ss[0].Trim()).GetParameters().Get(ss[1].Trim());
								}
							}
							else
							{
								if (ss.Length == 1)
								{
									p = args.GetDefault().GetParameters().Get(ss[0].Trim());
								}
							}
							object v = null;
							if (p != null)
							{
								v = p.GetValue();
							}
							if ("long".Equals(type))
							{
								if (v == null)
								{
									v = long.Parse(vs[i].Trim());
								}
							}
							else
							{
								if ("int".Equals(type))
								{
									if (v == null)
									{
										v = int.Parse(vs[i].Trim());
									}
								}
								else
								{
									if ("float".Equals(type))
									{
										if (v == null)
										{
											v = float.Parse(vs[i].Trim());
										}
									}
									else
									{
										if ("double".Equals(type))
										{
											if (v == null)
											{
												v = double.Parse(vs[i].Trim());
											}
										}
										else
										{
											if ("string".Equals(type))
											{
												if (v == null)
												{
													v = vs[i].Trim().ToString();
												}
											}
											else
											{
												if ("boolean".Equals(type))
												{
													if (v == null)
													{
														v = bool.Parse(vs[i].Trim());
													}
												}
												else
												{
													throw new GameConfigExpception(fName + "'s type '" + type + "' is not supported.");
												}
											}
										}
									}
								}
							}
							SetValue(target, f, v);
							System.Console.Out.WriteLine(target.GetType().FullName + "'s " + fName + " -> " + v);
						}
						catch (Exception e)
						{
							throw new GameConfigExpception("set " + fields + " to " + values + " failed at " + target.GetType().FullName + "\n" + ExceptionUtil.GetExceptionContent(e));
						}
					}
				}
			}
		}

		public virtual string GetFields()
		{
			return fields;
		}

		public virtual void SetFields(string fields)
		{
			this.fields = fields;
		}

		public virtual string GetValues()
		{
			return values;
		}

		public virtual void SetValues(string values)
		{
			this.values = values;
		}

		/// <exception cref="System.ArgumentException"/>
		/// <exception cref="System.MemberAccessException"/>
		private void SetValue(object target, FieldInfo f, object v)
		{
			if (!f.IsPrivate)
			{
				f.SetValue(target, v);
			}
			else
			{
				f.SetValue(target, v);
			}
		}

		public abstract object GetTarget(IEventArgs args);
	}
}
