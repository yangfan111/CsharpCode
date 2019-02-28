using Sharpen;
using com.wd.free.action;
using com.wd.free.item;
using com.wd.free.skill;
using com.wd.free.unit;
using gameplay.gamerule.free.rule;
using gameplay.gamerule.free.ui;

namespace gameplay.gamerule.free.item
{
	[System.Serializable]
	public class FreeGameItem : FreeItem
	{
		private const long serialVersionUID = -6970843131876269900L;

		private const string INI_RADIUS = "300";

		private string cat;

		private string time;

		private FreeEffectCreateAction effect;

		private ISkillTrigger trigger;

		private IGameAction enterAction;

		private IGameAction leaveAction;

		public override void Drop(ISkillArgs args, UnitPosition pos)
		{
		}

		public virtual string GetTime()
		{
			return time;
		}

		public virtual void SetTime(string time)
		{
			this.time = time;
		}

		public virtual string GetCat()
		{
			return cat;
		}

		public virtual void SetCat(string cat)
		{
			this.cat = cat;
		}
	}
}
