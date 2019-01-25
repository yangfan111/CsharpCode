using System.Collections.Generic;
using Sharpen;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class SkillMultiInterrupter : ISkillInterrupter
	{
		private const long serialVersionUID = 4470828830102400954L;

		public IList<ISkillInterrupter> inters;

		public virtual bool IsInterrupted(ISkillArgs args)
		{
			if (inters != null)
			{
				foreach (ISkillInterrupter inter in inters)
				{
					if (inter.IsInterrupted(args))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
