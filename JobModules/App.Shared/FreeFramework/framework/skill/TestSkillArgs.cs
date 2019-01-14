using Sharpen;
using com.wd.free;
using com.wd.free.@event;

namespace com.wd.free.skill
{
	public class TestSkillArgs : BaseEventArgs, ISkillArgs
	{
		private IPlayerInput input;

		public TestSkillArgs()
		{
			
		}

		public virtual IPlayerInput GetInput()
		{
			return input;
		}
	}
}
