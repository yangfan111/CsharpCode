using System.Collections.Generic;
using Sharpen;
using com.wd.free.para;
using com.wd.free.trigger;
using com.wd.free.unit;

namespace com.wd.free.config
{
	public class ConfigGameUnit
	{
		private string key;

		private string parent;

		private string paraParent;

		private string triggerParent;

		private IList<IPara> paras;

		private IList<GameTrigger> trriggers;

		public ConfigGameUnit()
			: base()
		{
			this.paras = new List<IPara>();
			this.trriggers = new List<GameTrigger>();
		}

		public ConfigGameUnit(string key)
			: base()
		{
			this.key = key;
			this.paras = new List<IPara>();
			this.trriggers = new List<GameTrigger>();
		}

		public virtual string GetParent()
		{
			return parent;
		}

		public virtual string GetParaParent()
		{
			return paraParent;
		}

		public virtual string GetTriggerParent()
		{
			return triggerParent;
		}

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void AddPara(IPara para)
		{
			this.paras.Add(para);
		}

		public virtual void AddTrigger(GameTrigger delegate_)
		{
			this.trriggers.Add(delegate_);
		}

		public virtual IGameUnit ToGameUnit()
		{
			BaseGameUnit unit = new BaseGameUnit();
			unit.SetKey(key);
			foreach (IPara para in paras)
			{
				unit.GetParameters().AddPara(para);
			}
			// for (GameTrigger para : trriggers) {
			// unit.getTriggers().addTrigger(para);
			// }
			return unit;
		}
	}
}
