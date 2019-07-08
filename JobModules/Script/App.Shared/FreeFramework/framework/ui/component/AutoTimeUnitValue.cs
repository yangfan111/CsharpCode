using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Free;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoTimeUnitValue : AbstractAutoValue, IRule
    {
		private const long serialVersionUID = 2613028647019236871L;

		private string scale;

		private int unit;

		private bool desc;

		public override string ToConfig(IEventArgs args)
		{
			return "time-auto|" + unit + "|" + FreeUtil.ReplaceNumber(scale, args) + "|" + desc;
		}

        public int GetRuleID()
        {
            return (int)ERuleIds.AutoTimeUnitValue;
        }
    }
}
