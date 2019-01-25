using System.Collections.Generic;
using Sharpen;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class RangeBufSkill : AbstractCoolDownSkill
	{
		private const long serialVersionUID = -3257043555978488725L;

		private int range;

		private IList<AuraSKill.GroupEffect> effects;

		public override void Effet(ISkillArgs args)
		{
			foreach (AuraSKill.GroupEffect ge in effects)
			{
				ge.Frame(args, this.unit, range);
			}
		}

		public override void Resume(ISkillArgs args)
		{
		}

		public override void Initial(ISkillArgs args)
		{
		}
	}
}
