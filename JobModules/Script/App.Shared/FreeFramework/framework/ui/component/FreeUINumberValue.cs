using Sharpen;
using com.wd.free.@event;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeUINumberValue : AbstractFreeUIValue
	{
		private const long serialVersionUID = -3843507028022831041L;

		private string number;

		public override int GetType()
		{
			return STRING;
		}

		public override object GetValue(IEventArgs args)
		{
			return GetValue(args, number);
		}

		public virtual string GetNumber()
		{
			return number;
		}

		public virtual void SetNumber(string number)
		{
			this.number = number;
		}

		public override string ToString()
		{
			return "ui number value:" + number;
		}
	}
}
