using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;

namespace com.wd.free.action
{
	[System.Serializable]
	public class AddParaFromOtherAction : AbstractGameAction
	{
		private const long serialVersionUID = -2416299676814019992L;

		private string key;

		private bool @override;

		private string from;

		private string fields;

		public override void DoAction(IEventArgs args)
		{
			IParable pa = args.GetUnit(key);
			IParable other = args.GetUnit(from);
			if (pa != null && other != null)
			{
				if (StringUtil.IsNullOrEmpty(fields))
				{
					AddTo(other.GetParameters().GetFields(), other, pa.GetParameters());
				}
				else
				{
					AddTo(StringUtil.Split(fields, ","), other, pa.GetParameters());
				}
			}
		}

		private void AddTo(string[] fields, IParable target, ParaList to)
		{
			foreach (string field in fields)
			{
				FieldPair fp = FieldPair.Parse(field)[0];
				ParaList tp = target.GetParameters();
				if (tp.HasPara(fp.GetFrom()))
				{
					IPara para = tp.Get(fp.GetFrom());
					if (para != null)
					{
						IPara clone = (IPara)para.Copy();
						clone.SetName(fp.GetTo());
						if (@override || !to.HasPara(fp.GetTo()))
						{
							to.AddPara(clone);
						}
						else
						{
							to.AddPara(clone);
						}
					}
				}
			}
		}
	}
}
