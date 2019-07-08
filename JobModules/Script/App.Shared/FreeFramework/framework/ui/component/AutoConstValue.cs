using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Free;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoConstValue : AbstractAutoValue, IRule
    {
		private const long serialVersionUID = 2613028647019236871L;

		private string value;

        public int GetRuleID()
        {
            return (int)ERuleIds.AutoConstValue;
        }

        public override string ToConfig(IEventArgs args)
		{
			return "const|" + FreeUtil.ReplaceNumber(value, args);
		}
	}
}
