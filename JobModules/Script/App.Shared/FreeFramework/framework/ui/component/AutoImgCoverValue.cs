using Sharpen;
using com.wd.free.@event;
using Core.Free;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoImgCoverValue : AbstractAutoValue, IRule
    {
		private const long serialVersionUID = 2613028647019236871L;

		private IFreeUIAuto coverU;

		private IFreeUIAuto coverV;

        public int GetRuleID()
        {
            return (int)ERuleIds.AutoImgCoverValue;
        }

        public override string ToConfig(IEventArgs args)
		{
			return "cover|" + coverU.ToConfig(args) + "_$$$_" + coverV.ToConfig(args);
		}
	}
}
