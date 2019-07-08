using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Free;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoPointValue : AbstractAutoValue, IRule
    {
		private const long serialVersionUID = 2613028647019236871L;

		private string xyz;

		private string delta;

		public override string ToConfig(IEventArgs args)
		{
			if (!StringUtil.IsNullOrEmpty(id))
			{
				return "point|" + FreeUtil.ReplaceNumber(id, args) + "," + FreeUtil.ReplaceVar(xyz, args) + "|" + FreeUtil.ReplaceVar(delta, args);
			}
			else
			{
				return "point|" + FreeUtil.ReplaceVar(xyz, args) + "|" + FreeUtil.ReplaceVar(delta, args);
			}
		}

        public int GetRuleID()
        {
            return (int)ERuleIds.AutoPointValue;
        }
    }
}
