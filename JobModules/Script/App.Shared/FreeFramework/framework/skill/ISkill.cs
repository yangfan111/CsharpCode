using Sharpen;
using com.wd.free.unit;

namespace com.wd.free.skill
{
	public interface ISkill : MyCloneable
	{
		IGameUnit GetUnit();

		void SetUnit(IGameUnit unit);

		string GetKey();

		string GetDesc();

		void SetDisable(bool valid);

		void Frame(ISkillArgs args);

		ISkill Clone();
	}
}
