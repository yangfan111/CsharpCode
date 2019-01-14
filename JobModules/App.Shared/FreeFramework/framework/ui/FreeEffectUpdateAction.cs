using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using gameplay.gamerule.free.ui.component;

namespace gameplay.gamerule.free.ui
{
	[System.Serializable]
	public class FreeEffectUpdateAction : SendMessageAction
	{
		private const long serialVersionUID = -1188091456904593121L;

		private string key;

		private IList<IFreeUIValue> values;

		public FreeEffectUpdateAction()
		{
			this.values = new List<IFreeUIValue>();
		}

		public virtual void AddValue(IFreeUIValue value)
		{
			this.values.Add(value);
		}

		protected override  void BuildMessage(IEventArgs args)
		{
		    builder.Key = 62;
		    builder.Ss.Add(FreeUtil.ReplaceVar(key, args));
			if (values != null)
			{
				foreach (IFreeUIValue com in values)
				{
				    builder.Ks.Add(com.GetSeq(args));
				    builder.Ins.Add(com.GetAutoStatus() * 100 + com.GetAutoIndex());
				}
				foreach (IFreeUIValue com_1 in values)
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
				}
			}
		}

		public override string GetMessageDesc()
		{
			return "更新特效" + key + "'\n" + builder.ToString();
		}
	}
}
