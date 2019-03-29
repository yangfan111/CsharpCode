using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeUITextValue : AbstractFreeUIValue
	{
		private const long serialVersionUID = 5852438718279157187L;

		private string text;

		public override int GetType()
		{
			return STRING;
		}

		public override object GetValue(IEventArgs args)
		{
			if (autoStatus > 0)
			{
				return FreeUtil.ReplaceNumber(text, args);
			}
			else
			{
				return FreeUtil.ReplaceVar(text, args);
			}
		}

		public virtual string GetText()
		{
			return text;
		}

		public virtual void SetText(string text)
		{
			this.text = text;
		}

		public override string ToString()
		{
			return  "ui text value:" + text;
		}
	}
}
