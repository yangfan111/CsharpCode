using com.cpkf.yyjd.tools.util;
using Sharpen;
using com.wd.free.action;
using com.wd.free.unit;

namespace com.wd.free.skill
{
	[System.Serializable]
	public abstract class AbstractSkill : ISkill
	{
		private const long serialVersionUID = 98152716160189883L;

		protected internal string key;

		protected internal string desc;

		protected internal bool disable;

		protected internal IGameAction disableAction;

		protected internal IGameUnit unit;

		public virtual string GetDesc()
		{
			return desc;
		}

		public virtual void SetDesc(string desc)
		{
			this.desc = desc;
		}

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void SetKey(string key)
		{
			this.key = key;
		}

		public virtual IGameUnit GetUnit()
		{
			return unit;
		}

		public virtual void SetUnit(IGameUnit unit)
		{
			this.unit = unit;
		}

		public virtual void SetDisable(bool disable)
		{
			this.disable = disable;
		}

		public virtual ISkill Clone()
		{
		    return (ISkill)SerializeUtil.Clone(this);
		}

		public abstract void Frame(ISkillArgs arg1);
	}
}
