using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeUIImageValue : AbstractFreeUIValue
	{
		private const long serialVersionUID = -8816773404741426794L;

		private string url;

		public override int GetType()
		{
			return STRING;
		}

		public override object GetValue(IEventArgs args)
		{
			if (autoStatus > 0)
			{
				return FreeUtil.ReplaceNumber(url, args);
			}
			else
			{
				return FreeUtil.ReplaceVar(url, args);
			}
		}

		public virtual string GetUrl()
		{
			return url;
		}

		public virtual void SetUrl(string url)
		{
			this.url = url;
		}

		public override string ToString()
		{
			return "ui image value:" + url;
		}
	}
}
