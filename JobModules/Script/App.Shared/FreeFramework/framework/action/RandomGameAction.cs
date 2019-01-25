using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util.collection;
using com.cpkf.yyjd.tools.util.math;
using com.wd.free.@event;
using com.wd.free.para.exp;

namespace com.wd.free.action
{
	[System.Serializable]
	public class RandomGameAction : AbstractGameAction
	{
		private IList<IGameAction> actions;

		public override void DoAction(IEventArgs args)
		{
			if (actions != null)
			{
				IList<IGameAction> list = new List<IGameAction>();
			    int[] pros = new int[actions.Count];
				for (int i = 0; i < actions.Count; i++)
				{
					IGameAction ga = actions[i];
					if (ga is RandomGameAction.ConAction)
					{
						RandomGameAction.ConAction ca = (RandomGameAction.ConAction)ga;
						if (ca.GetCondition() == null || ca.GetCondition().Meet(args))
						{
							list.Add(ca.GetAction());
						    pros[list.Count - 1] = ca.GetProbability();
                        }
					}
					else
					{
						list.Add(ga);
					    pros[i] = 1;
					}
				}

			    if (list.Count > 0)
			    {
			        int index = RandomUtil.RandomWithProRepeat(0, pros, 1)[0];
			        if (index < list.Count)
			        {
                        list[index].Act(args);
			        }
			    }
			}
		}

		public virtual void AddGameAction(IGameAction action)
		{
			if (actions == null)
			{
				actions = new List<IGameAction>();
			}
			this.actions.Add(action);
		}

		[System.Serializable]
		public class ConAction : AbstractGameAction
		{
			private const long serialVersionUID = 5436170514812431313L;

			private IParaCondition condition;

			private int probability;

			private IGameAction action;

			public ConAction()
				: base()
			{
			}

			public ConAction(IParaCondition condition, IGameAction action)
				: base()
			{
				this.condition = condition;
				this.action = action;
			}

			public override void DoAction(IEventArgs args)
			{
				this.action.Act(args);
			}

			public virtual IParaCondition GetCondition()
			{
				return condition;
			}

			public virtual void SetCondition(IParaCondition condition)
			{
				this.condition = condition;
			}

			public virtual IGameAction GetAction()
			{
				return action;
			}

			public virtual void SetAction(IGameAction action)
			{
				this.action = action;
			}

			public virtual int GetProbability()
			{
				return probability;
			}

			public virtual void SetProbability(int probability)
			{
				this.probability = probability;
			}
		}
	}
}
