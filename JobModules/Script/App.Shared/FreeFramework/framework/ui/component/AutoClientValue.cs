using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using Core.Free;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoClientValue : AbstractAutoValue, IRule
    {
		private const long serialVersionUID = 2613028647019236871L;

		private string key;

		private string type;

		public override string ToConfig(IEventArgs args)
		{
			return "client|" + key + "|" + FreeUtil.ReplaceVar(type, args);
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
            return (int)ERuleIds.AutoClientValue;
        }
    }
}
