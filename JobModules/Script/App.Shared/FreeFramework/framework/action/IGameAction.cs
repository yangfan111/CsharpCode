using Sharpen;
using com.wd.free.@event;

namespace com.wd.free.action
{
	public interface IGameAction
	{
		void Act(IEventArgs args);

        void Reset(IEventArgs args);

		string ToMessage(IEventArgs args);
	}
}
