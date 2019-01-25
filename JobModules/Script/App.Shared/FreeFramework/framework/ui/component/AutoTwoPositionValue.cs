using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoTwoPositionValue : AbstractAutoValue
	{
		private const long serialVersionUID = 2613028647019236871L;

		private string source;

		private string target;

		private string distance;

		private string height;

		private bool toSource;

		public override string ToConfig(IEventArgs args)
		{
			if (distance == null)
			{
				distance = "0";
			}
			if (height == null)
			{
				height = "0";
			}
			return "tposition|" + FreeUtil.ReplaceVar(source, args) + "|" + FreeUtil.ReplaceVar(target, args) + "|" + FreeUtil.ReplaceNumber(distance, args) + "|" + FreeUtil.ReplaceNumber(height, args) + "|" + toSource;
		}
	}
}
