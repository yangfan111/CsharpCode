using System.Collections.Generic;
using Sharpen;

namespace com.wd.free.trigger
{
	[System.Serializable]
	public class TriggerList : Iterable<GameTrigger>
	{
		private const long serialVersionUID = -6223440681146463521L;

		private IList<GameTrigger> triggers;

		public TriggerList()
		{
			this.triggers = new List<GameTrigger>();
		}

		public virtual void AddTrigger(GameTrigger trigger)
		{
			this.triggers.Add(trigger);
		}

		public virtual void MergeTriggerList(com.wd.free.trigger.TriggerList tl)
		{
			for (int i = tl.triggers.Count - 1; i >= 0; i--)
			{
				this.triggers.Add(0, tl.triggers[i]);
			}
		}

		public virtual IList<GameTrigger> GetTriggers()
		{
			return triggers;
		}

		public virtual void SetTriggers(IList<GameTrigger> triggers)
		{
			this.triggers = triggers;
		}

		public override Sharpen.Iterator<GameTrigger> Iterator()
		{
			return triggers.Iterator();
		}
	}
}
