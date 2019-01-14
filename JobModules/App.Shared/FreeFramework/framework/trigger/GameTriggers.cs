using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;

namespace com.wd.free.trigger
{
	public class GameTriggers
	{
		private MyDictionary<int, IList<GameTrigger>> triggers;

		public GameTriggers()
		{
			this.triggers = new MyDictionary<int, IList<GameTrigger>>();
		}

        public List<GameTrigger> GetTriggers()
        {
            List<GameTrigger> list = new List<GameTrigger>();
            foreach (IList<GameTrigger> tlist in triggers.Values)
            {
                list.AddRange(tlist);
            }

            return list;
        }

		public virtual void AddTrigger(GameTrigger trigger)
		{
			if (!triggers.ContainsKey(trigger.GetKey()))
			{
				triggers[trigger.GetKey()] = new List<GameTrigger>();
			}
			IList<GameTrigger> tList = triggers[trigger.GetKey()];
			tList.Add(trigger);
		}

		public virtual void DisableTrigger(int key, string name)
		{
			if (triggers.ContainsKey(key))
			{
				IList<GameTrigger> tList = triggers[key];
				foreach (GameTrigger gt in tList)
				{
					if (name.Equals(gt.GetName()))
					{
						gt.Disable();
					}
				}
			}
		}

		public virtual void EnableTrigger(int key, string name)
		{
			if (triggers.ContainsKey(key))
			{
				IList<GameTrigger> tList = triggers[key];
				foreach (GameTrigger gt in tList)
				{
					if (name.Equals(gt.GetName()))
					{
						gt.Enable();
					}
				}
			}
		}

		public virtual void Merge(TriggerList triggers)
		{
			foreach (GameTrigger gt in triggers)
			{
				AddTrigger(gt);
			}
		}

		public virtual bool IsEmpty(int @event)
		{
			IList<GameTrigger> tList = this.triggers[@event];
			if (tList != null && !tList.IsEmpty())
			{
				foreach (GameTrigger gt in tList)
				{
					if (gt.GetActions().Count > 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		public virtual void Trigger(int @event, IEventArgs args)
		{
			IList<GameTrigger> tList = this.triggers[@event];
			if (tList != null)
			{
				foreach (GameTrigger trigger in tList)
				{
					trigger.Trigger(args);
				}
			}
		}
	}
}
