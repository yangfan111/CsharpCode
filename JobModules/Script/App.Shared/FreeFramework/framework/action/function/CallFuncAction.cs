using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;

namespace com.wd.free.action.function
{
	[System.Serializable]
	public class CallFuncAction : AbstractGameAction
	{
		private const long serialVersionUID = 2157777497711974144L;

		private IList<ArgValue> args;

		private IFuncSelector selector;

		public CallFuncAction()
		{
			this.args = new List<ArgValue>();
		}

		public virtual void AddArgValue(ArgValue arg)
		{
			this.args.Add(arg);
		}

		public virtual IFuncSelector GetSelector()
		{
			return selector;
		}

		public virtual void SetSelector(IFuncSelector selector)
		{
			this.selector = selector;
		}

		public override void DoAction(IEventArgs args)
		{
			GameFunc func = selector.Select(args);
			if (func != null)
			{
				func.Action(this.args, args);
			}
		}
	}
}
