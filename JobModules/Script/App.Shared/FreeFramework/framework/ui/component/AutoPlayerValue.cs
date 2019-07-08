using Sharpen;
using com.wd.free.@event;
using Core.Free;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoPlayerValue : AbstractAutoValue, IRule
    {
		private const long serialVersionUID = 2613028647019236871L;

		private string key;

		public override string ToConfig(IEventArgs args)
		{
			return "player|" + GetID(args) + "|" + key + "|" + all;
		}

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void SetKey(string key)
		{
			this.key = key;
		}

        public int GetRuleID()
        {
            return (int)ERuleIds.AutoPlayerValue;
        }
    }
}
