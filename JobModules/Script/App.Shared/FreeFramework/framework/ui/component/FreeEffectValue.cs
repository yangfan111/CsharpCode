using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Free;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeEffectValue : AbstractFreeUIValue, IRule
    {
		private const long serialVersionUID = -8816773404741426794L;

		private string value;

        public int GetRuleID()
        {
            return (int)ERuleIds.FreeEffectValue;
        }

        public override int GetType()
		{
			return STRING;
		}

		public override object GetValue(IEventArgs args)
		{
            if (value.Contains(","))
            {
                return FreeUtil.ReplaceVar(value, args);
            }
            else
            {
                return GetValue(args, value);
            }
			
		}

		public override string ToString()
		{
			return this.GetType() + ":" + value;
		}
	}
}
