using Sharpen;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.util;

namespace com.wd.free.action
{
	[System.Serializable]
	public class SaveVarAction : AbstractGameAction
	{
		private const long serialVersionUID = -3856004590316476013L;

		private string name;

		private string var;

		public override void DoAction(IEventArgs args)
		{
			IParable parable = args.GetUnit(FreeUtil.ReplaceVar(var, args));
			if (parable != null)
			{
				args.SetPara(FreeUtil.ReplaceVar(name, args), parable);
			}
		}
	}
}
