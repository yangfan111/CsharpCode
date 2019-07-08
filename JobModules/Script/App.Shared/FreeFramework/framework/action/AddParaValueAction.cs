using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;
using Core.Free;
using Core.Utils;
using com.wd.free.exception;

namespace com.wd.free.action
{
	[System.Serializable]
	public class AddParaValueAction : AbstractGameAction, IRule
    {
		private const long serialVersionUID = -1756304819902634228L;

		private string key;

		private bool @override;

		private IList<ParaValue> paras;

        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AddParaValueAction));

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
                    if (null == para) {
                        throw new GameConfigExpception("para is not a null field at " + key);
                    }
					if (!p.GetParameters().HasPara(para.GetName()) || @override)
					{
						p.GetParameters().AddPara(para.GetPara(args));
					}
				}
			}
            if (paras == null || paras.Count == 0) {
                _logger.Info("AddParaValueAction paras is null or count is 0 !");
            }
		}

        public int GetRuleID()
        {
            return (int)ERuleIds.AddParaValueAction;
        }
    }
}
