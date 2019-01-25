using System.Collections.Generic;
using Sharpen;
using com.wd.free.skill;

namespace com.wd.free.unit
{
	public class GameUnitSet : Iterable<IGameUnit>
	{
		private IList<IGameUnit> units;

		public GameUnitSet()
		{
			this.units = new List<IGameUnit>();
		}

		public virtual void AddGameUnit(IGameUnit unit)
		{
			this.units.Add(unit);
		}

		public virtual void RemoveGameaUnit(IGameUnit unit)
		{
			this.units.Remove(unit);
		}

		public virtual void AddEffect(ISkillEffect effect)
		{
			foreach (IGameUnit unit in units)
			{
				unit.GetUnitSkill().AddSkillEffect(effect);
			}
		}

		public virtual void RemoveEffect(ISkillEffect effect)
		{
			foreach (IGameUnit unit in units)
			{
				unit.GetUnitSkill().RemoveSkillEffect(effect);
			}
		}

		public override Sharpen.Iterator<IGameUnit> Iterator()
		{
			return units.Iterator();
		}
	}
}
