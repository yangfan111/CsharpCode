using Sharpen;
using com.wd.free.@event;

namespace com.wd.free.skill
{
	public interface ISkillArgs : IEventArgs
	{
		IPlayerInput GetInput();
	}
}
