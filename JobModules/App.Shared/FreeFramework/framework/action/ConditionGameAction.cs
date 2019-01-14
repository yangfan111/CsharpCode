using Sharpen;
using com.wd.free.@event;
using com.wd.free.para.exp;

namespace com.wd.free.action
{
	[System.Serializable]
	public class ConditionGameAction : AbstractGameAction
	{
		private const long serialVersionUID = -2931122771587588898L;

		private IParaCondition condition;

		private IGameAction trueAction;

		private IGameAction falseAction;

		public ConditionGameAction()
		{
		}

		public override void DoAction(IEventArgs args)
		{
			// if (condition != null) {
			// System.out.println(condition + " is " + condition.meet(args));
			// }
			if (condition == null || condition.Meet(args))
			{
				if (trueAction != null)
				{
					trueAction.Act(args);
				}
			}
			else
			{
				if (falseAction != null)
				{
					falseAction.Act(args);
				}
			}
		}

		public override string GetDesc()
		{
			return desc;
		}

		public override void SetDesc(string desc)
		{
			this.desc = desc;
		}

		public virtual IParaCondition GetCondition()
		{
			return condition;
		}

		public virtual void SetCondition(IParaCondition condition)
		{
			this.condition = condition;
		}

		public override string ToString()
		{
			string s = condition.ToString() + " ? ";
			if (trueAction != null)
			{
				s = s + " true->" + trueAction.ToString();
			}
			if (falseAction != null)
			{
				s = s + " false->" + falseAction.ToString();
			}
			return s;
		}
	}
}
