using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.util;
using Free.framework;
using gameplay.gamerule.free.ui.component;

namespace gameplay.gamerule.free.ui
{
	[System.Serializable]
	public class FreeUIUpdateAction : SendMessageAction
	{
		private const long serialVersionUID = -1188091456904593121L;

		private string key;

		private IList<IFreeUIValue> values;

		public FreeUIUpdateAction()
		{
			this.values = new List<IFreeUIValue>();
		}

		public virtual void AddValue(IFreeUIValue value)
		{
			this.values.Add(value);
		}

        public void ClearValue()
        {
            this.values.Clear();
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
		    builder.Key = 52;
		    builder.Ss.Add(FreeUtil.ReplaceNumber(key, args));
			builder.Ks.Add(0);
			if (values != null)
			{
				foreach (IFreeUIValue com in values)
				{
					builder.Ks.Add(com.GetSeq(args));
					builder.Ins.Add(com.GetAutoStatus() * 100 + com.GetAutoIndex());
				}
				foreach (IFreeUIValue com_1 in values)
				{
					switch (com_1.GetType())
					{
						case AbstractFreeUIValue.STRING:
						{
							object obj = com_1.GetValue(args);
							if (obj == null)
							{
								builder.Ss.Add("null");
							}
							else
							{
								builder.Ss.Add(obj.ToString());
							}
							break;
						}

						case AbstractFreeUIValue.INT:
						{
							builder.Ins.Add(int.Parse(com_1.GetValue(args).ToString()));
							break;
						}

						case AbstractFreeUIValue.BOOL:
						{
						    builder.Bs.Add(bool.Parse(com_1.GetValue(args).ToString()));
							break;
						}

						case AbstractFreeUIValue.FLOAT:
						{
							builder.Fs.Add(float.Parse(com_1.GetValue(args).ToString()));
							break;
						}

						case AbstractFreeUIValue.COMPLEX:
						{
							SimpleProto[] list = (SimpleProto[])com_1.GetValue(args);
							IList<string> orders = new List<string>();
							for (int i = builder.Ps.Count; i < builder.Ps.Count + list.Length; i++)
							{
								orders.Add(i.ToString());
							}
							builder.Ss.Add(StringUtil.GetStringFromStrings(orders, "_"));
							foreach (SimpleProto sp in list)
							{
								builder.Ps.Add(sp);
							}
							break;
						}

						default:
						{
							break;
						}
					}
				}
			}
		}

		public override string GetMessageDesc()
		{
			string d = string.Empty;
			if (!StringUtil.IsNullOrEmpty(desc))
			{
				d = "(" + desc + ")";
			}
			return "更新UI'" + key + d + "'\n" + builder.ToString();
		}
	}
}
