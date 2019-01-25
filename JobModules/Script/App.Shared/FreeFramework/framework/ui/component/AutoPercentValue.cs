using Sharpen;
using com.wd.free.@event;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoPercentValue : AbstractAutoValue
	{
		private const long serialVersionUID = 2613028647019236871L;

		private IFreeUIAuto current;

		private IFreeUIAuto max;

		public override string ToConfig(IEventArgs args)
		{
			return "percent|" + current.ToConfig(args) + "->" + max.ToConfig(args);
		}
	}
}
