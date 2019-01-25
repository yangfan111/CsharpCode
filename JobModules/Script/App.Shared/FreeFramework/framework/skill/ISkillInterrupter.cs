using Sharpen;

namespace com.wd.free.skill
{
	public interface ISkillInterrupter
	{
		bool IsInterrupted(ISkillArgs args);
	}
}
