using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoTimeValue : AbstractAutoValue
	{
		private const long serialVersionUID = 2613028647019236871L;

		private string time;

		private string from;

		private string to;

		private string reverse;

		public override string ToConfig(IEventArgs args)
		{
			string r = "time|" + FreeUtil.ReplaceInt(time, args) + "|" + FreeUtil.ReplaceInt(from, args) + "|" + FreeUtil.ReplaceInt(to, args) + "|" + "true".Equals(FreeUtil.ReplaceVar(reverse, args));
			return r;
		}
	}
}
