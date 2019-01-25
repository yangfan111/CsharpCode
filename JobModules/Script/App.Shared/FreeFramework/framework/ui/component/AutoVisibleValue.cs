using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoVisibleValue : AbstractAutoValue
	{
		private const long serialVersionUID = 2613028647019236871L;

		private string reverse;

		private string xyz;

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
