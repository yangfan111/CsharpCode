using Sharpen;
using com.wd.free.@event;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeUIAutoValue : AbstractFreeUIValue
	{
		private const long serialVersionUID = -8816773404741426794L;

		private string action;

		public FreeUIAutoValue()
		{
		}

		public FreeUIAutoValue(int seq, string action)
		{
			this.seq = seq.ToString();
			this.action = action;
		}

		public override int GetType()
		{
			return STRING;
		}

		public override object GetValue(IEventArgs args)
		{
			return GetAction();
		}

		public virtual string GetAction()
		{
			return action;
		}

		public virtual void SetAction(string action)
		{
			this.action = action;
		}

		public override string ToString()
		{
			return "ui auto value:" + action;
		}
	}
}
