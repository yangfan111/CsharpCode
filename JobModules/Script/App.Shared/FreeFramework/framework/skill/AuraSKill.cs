using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.para.exp;
using com.wd.free.unit;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class AuraSKill : AbstractSkill
	{
		private const long serialVersionUID = -2075872402350578799L;

		private int range;

		private IList<AuraSKill.GroupEffect> effects;

		public virtual int GetRange()
		{
			return range;
		}

		public virtual void SetRange(int range)
		{
			this.range = range;
		}

		public override void Frame(ISkillArgs args)
		{
			foreach (AuraSKill.GroupEffect ge in effects)
			{
				ge.Frame(args, this.unit, range);
			}
		}

		[System.Serializable]
		public class GroupEffect
		{
			private const long serialVersionUID = -3348967790172052193L;

			private string key;

			private string condition;

			private ISkillEffect effect;

			[System.NonSerialized]
			private ExpParaCondition con;

			public virtual string GetKey()
			{
				return key;
			}

			public virtual void SetKey(string key)
			{
				this.key = key;
			}

			public virtual ISkillEffect GetEffect()
			{
				return effect;
			}

			public virtual void SetEffect(ISkillEffect effect)
			{
				this.effect = effect;
			}

			public virtual void Frame(ISkillArgs args, IGameUnit unit, int range)
			{
				if (con == null && !StringUtil.IsNullOrEmpty(condition))
				{
					con = new ExpParaCondition(condition);
				}
				BaseEventArgs bea = (BaseEventArgs)args;
				bea.TempUse("give", unit);
				foreach (IGameUnit gu in args.GetGameUnits())
				{
					if (((XYZPara.XYZ)gu.GetXYZ().GetValue()).Distance(((XYZPara.XYZ)unit.GetXYZ().GetValue())) <= range)
					{
						bea.TempUse("get", gu);
						if (con == null || con.Meet(args))
						{
							effect.SetKey(this.key);
							effect.SetSource(unit.GetID());
							if (effect.GetTime() < 200)
							{
								effect.SetTime(200);
							}
							gu.GetUnitSkill().AddSkillEffect(effect);
						}
						bea.Resume("get");
					}
				}
				bea.Resume("give");
			}
		}
	}
}
