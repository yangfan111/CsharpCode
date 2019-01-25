using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.para;

namespace com.wd.free.action
{
	[System.Serializable]
	public class DefineParaAction : AbstractGameAction
	{
		private const long serialVersionUID = 4962914672129547843L;

		private IList<ParaValue> paras;

		private IGameAction action;

		public DefineParaAction()
		{
			this.paras = new List<ParaValue>();
		}

		public virtual void AddParaValue(ParaValue pv)
		{
			this.paras.Add(pv);
		}

		public override void DoAction(IEventArgs args)
		{
			if (paras == null)
			{
				paras = new List<ParaValue>();
			}
			foreach (ParaValue pv in paras)
			{
				args.GetDefault().GetParameters().TempUse(pv.GetPara(args));
			}
			action.Act(args);
			foreach (ParaValue pv_1 in paras)
			{
				args.GetDefault().GetParameters().Resume(pv_1.GetName());
			}
		}

		public virtual IGameAction GetAction()
		{
			return action;
		}

		public virtual void SetAction(IGameAction action)
		{
			this.action = action;
		}
	}
}
