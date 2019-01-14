using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.para;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class SimpleInstantSkill : AbstractSkill
	{
		private const long serialVersionUID = 6967171122753916574L;

		private IList<IPara> paras;

		private IList<IGameAction> actions;

		[System.NonSerialized]
		private bool initialed;

		public override void Frame(ISkillArgs args)
		{
			if (args is BaseEventArgs)
			{
				BaseEventArgs ea = (BaseEventArgs)args;
				Ini(ea);
				foreach (IGameAction action in actions)
				{
					action.Act(ea);
				}
			}
		}

		private void Ini(BaseEventArgs args)
		{
			if (!initialed)
			{
				IParable parable = new _IParable_33(paras);
				args.SetPara(key.ToString(), parable);
				initialed = true;
			}
		}

		private class _IParable_33 : IParable
		{
			public _IParable_33(IList<IPara> paras)
			{
				{
					this.pl = new ParaList();
					if (paras != null)
					{
						foreach (IPara para in paras)
						{
							this.pl.AddPara(para);
						}
					}
				}
			}

			private ParaList pl;

			public ParaList GetParameters()
			{
				return this.pl;
			}
		}
	}
}
