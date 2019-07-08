using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Free;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoVisibleValue : AbstractAutoValue, IRule
    {
		private const long serialVersionUID = 2613028647019236871L;

		private string reverse;

		private string xyz;

        public int GetRuleID()
        {
            return (int)ERuleIds.AutoVisibleValue;
        }

        public override string ToConfig(IEventArgs args)
		{
			if (!StringUtil.IsNullOrEmpty(id))
			{
				return "visible|" + reverse + "|" + FreeUtil.ReplaceNumber(id, args) + "," + FreeUtil.ReplaceVar(xyz, args);
			}
			else
			{
				return "visible|" + reverse + "|" + FreeUtil.ReplaceVar(xyz, args);
			}
		}
	}
}
