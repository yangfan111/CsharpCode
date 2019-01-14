using Sharpen;
using com.wd.free.@event;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoImgCoverValue : AbstractAutoValue
	{
		private const long serialVersionUID = 2613028647019236871L;

		private IFreeUIAuto coverU;

		private IFreeUIAuto coverV;

		public override string ToConfig(IEventArgs args)
		{
			return "cover|" + coverU.ToConfig(args) + "_$$$_" + coverV.ToConfig(args);
		}
	}
}
