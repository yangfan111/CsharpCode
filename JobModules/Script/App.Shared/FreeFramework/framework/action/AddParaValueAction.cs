using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;

namespace com.wd.free.action
{
	[System.Serializable]
	public class AddParaValueAction : AbstractGameAction
	{
		private const long serialVersionUID = -1756304819902634228L;

		private string key;

		private bool @override;

		private IList<ParaValue> paras;

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
			if (p != null && paras != null)
			{
				foreach (ParaValue para in paras)
				{
					if (!p.GetParameters().HasPara(para.GetName()) || @override)
					{
						p.GetParameters().AddPara(para.GetPara(args));
					}
				}
			}
		}
	}
}
