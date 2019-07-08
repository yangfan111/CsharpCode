using System.Collections.Generic;
using Sharpen;
using com.wd.free.para;
using System;
using Core.Free;

namespace gameplay.gamerule.free.action
{
	[System.Serializable]
	public class CommonActions : Iterable<CommonGameAction>, IRule
    {
		private IList<CommonGameAction> actions;

		public CommonActions()
		{
			this.actions = new List<CommonGameAction>();
		}

		public virtual void AddCommonAction(CommonGameAction action)
		{
			this.actions.Add(action);
		}

	    public void Clear()
	    {
	        this.actions.Clear();
	    }

        public int GetRuleID()
        {
            return (int)ERuleIds.CommonActions;
        }

        public override Sharpen.Iterator<CommonGameAction> Iterator()
		{
			return actions.Iterator();
		}

		public virtual void Merge(gameplay.gamerule.free.action.CommonActions actions)
		{
			foreach (CommonGameAction ca in actions.actions)
			{
				this.actions.Add(ca);
			}
		}
	}
}
