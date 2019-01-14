using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoTimeStringValue : AbstractAutoValue
	{
		private const long serialVersionUID = 2613028647019236871L;

		private string time;

		private string values;

		public override string ToConfig(IEventArgs args)
		{
			return "string" + SPLITER + FreeUtil.ReplaceVar(time, args) + SPLITER + StringUtil.GetStringFromStrings(StringUtil.Split(FreeUtil.ReplaceVar(values, args), ","), SPLITER);
		}
	}
}
