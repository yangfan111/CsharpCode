using Sharpen;
using com.wd.free.@event;
using Core.Free;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoPercentValue : AbstractAutoValue, IRule
    {
		private const long serialVersionUID = 2613028647019236871L;

		private IFreeUIAuto current;

		private IFreeUIAuto max;

        public int GetRuleID()
        {
            return (int)ERuleIds.AutoPercentValue;
        }

        public override string ToConfig(IEventArgs args)
		{
			return "percent|" + current.ToConfig(args) + "->" + max.ToConfig(args);
		}
	}
}
