using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoConstValue : AbstractAutoValue
	{
		private const long serialVersionUID = 2613028647019236871L;

		private string value;

		public override string ToConfig(IEventArgs args)
		{
			return "const|" + FreeUtil.ReplaceNumber(value, args);
		}
	}
}
