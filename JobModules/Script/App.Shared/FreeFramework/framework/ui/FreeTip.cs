using Sharpen;

namespace gameplay.gamerule.free.ui
{
	public class FreeTip
	{
		private string lang;

		private string tip;

		private string type;

		public virtual string GetType()
		{
			return type;
		}

		public virtual void SetType(string type)
		{
			this.type = type;
		}

		public virtual string GetLang()
		{
			return lang;
		}

		public virtual void SetLang(string lang)
		{
			this.lang = lang;
		}

		public virtual string GetTip()
		{
			return tip;
		}

		public virtual void SetTip(string tip)
		{
			this.tip = tip;
		}
	}
}
