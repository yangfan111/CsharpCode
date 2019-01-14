using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;

namespace com.wd.free.action
{
	[System.Serializable]
	public class MultiGameAction : AbstractGameAction
	{
		private const long serialVersionUID = -714645490977369377L;

		private IList<IGameAction> actions;

		public override void DoAction(IEventArgs args)
		{
			if (actions != null)
			{
				foreach (IGameAction action in actions)
				{
					action.Act(args);
				}
			}
		}

		public virtual IList<IGameAction> GetActions()
		{
			if (actions == null)
			{
				actions = new List<IGameAction>();
			}
			return actions;
		}

        public override void Reset(IEventArgs args)
        {
            for(int i = 0; i < actions.Count; i++)
            {
                actions[i].Reset(args);
            }
        }

        public override string ToString()
		{
			if (actions != null)
			{
				return "multi " + actions.Count;
			}
			else
			{
				return "multi 0";
			}
		}
	}
}
