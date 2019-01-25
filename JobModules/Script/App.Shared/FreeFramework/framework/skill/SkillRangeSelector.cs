using Sharpen;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.unit;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class SkillRangeSelector : IUnitSelector
	{
		private const long serialVersionUID = -7944686343153478732L;

		private int range;

		private IGameUnit trigger;

		public SkillRangeSelector(IGameUnit trigger, int range)
		{
			this.trigger = trigger;
			this.range = range;
		}

		public virtual GameUnitSet Select(IEventArgs args, IGameUnit unit)
		{
			GameUnitSet resutl = new GameUnitSet();
			foreach (IGameUnit gu in args.GetGameUnits())
			{
				if (((XYZPara.XYZ)gu.GetXYZ().GetValue()).Distance(((XYZPara.XYZ)trigger.GetXYZ().GetValue())) <= range)
				{
					resutl.AddGameUnit(gu);
				}
			}
			return resutl;
		}
	}
}
