using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;

namespace gameplay.gamerule.free.component
{
	[System.Serializable]
	public class ComponentAction : AbstractGameAction, IComponentable
	{
		private const long serialVersionUID = -3096668947524226601L;

		private string name;

		private string title;

		private IGameAction defaultAction;

		public override void DoAction(IEventArgs args)
		{
			IGameAction action = args.ComponentMap.GetAction(name);
			if (action != null)
			{
				action.Act(args);
			}
			else
			{
				if (defaultAction != null)
				{
					defaultAction.Act(args);
				}
			}
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual string GetTitle()
		{
			return title;
		}

		public virtual void SetTitle(string title)
		{
			this.title = title;
		}

		public virtual IGameAction GetDefaultAction()
		{
			return defaultAction;
		}
	}
}
