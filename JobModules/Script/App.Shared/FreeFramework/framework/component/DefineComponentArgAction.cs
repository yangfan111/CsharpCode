using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.para;
using Core.Free;

namespace gameplay.gamerule.free.component
{
	[System.Serializable]
	public class DefineComponentArgAction : AbstractGameAction, IRule
    {
		private const long serialVersionUID = 5655675093726216330L;

		private IList<ParaValue> args;

		public override void DoAction(IEventArgs args)
		{
			if (this.args != null)
			{
				foreach (ParaValue pv in this.args)
				{
					args.GetDefault().GetParameters().AddPara(pv.GetPara(args));
				}
			}
		}

        public int GetRuleID()
        {
            return (int)ERuleIds.DefineComponentArgAction;
        }
    }
}
