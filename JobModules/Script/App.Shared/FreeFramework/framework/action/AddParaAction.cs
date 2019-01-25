using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.util;

namespace com.wd.free.action
{
	[System.Serializable]
	public class AddParaAction : AbstractGameAction
	{
		private const long serialVersionUID = -1756304819902634228L;

		private string key;

		private bool @override;

		private IList<IPara> paras;

		public AddParaAction()
		{
			this.paras = new List<IPara>();
		}

		public virtual void AddPara(IPara para)
		{
			this.paras.Add(para);
		}

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
				foreach (IPara para in paras)
				{
					if (!p.GetParameters().HasPara(para) || @override)
					{
						IPara clone = (IPara)para.Copy();
						clone.SetName(FreeUtil.ReplaceVar(clone.GetName(), args));
						p.GetParameters().AddPara(clone);
					}
				}
			}
		}

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void SetKey(string key)
		{
			this.key = key;
		}
	}
}
