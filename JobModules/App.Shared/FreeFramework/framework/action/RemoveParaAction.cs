using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;

namespace com.wd.free.action
{
	[System.Serializable]
	public class RemoveParaAction : AbstractGameAction
	{
		private const long serialVersionUID = 5695293732519970848L;

		private string key;

		private string para;

		public override void DoAction(IEventArgs args)
		{
			IParable p = null;
			if (StringUtil.IsNullOrEmpty(key))
			{
				p = args.GetDefault();
			}
			else
			{
				p = args.GetUnit(key);
			}
			if (p != null && para != null)
			{
				foreach (string pa in StringUtil.Split(para, ","))
				{
					p.GetParameters().RemovePara(pa.Trim());
				}
			}
		}
	}
}
