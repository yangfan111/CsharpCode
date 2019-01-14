using Sharpen;
using com.wd.free.@event;
using Free.framework;

namespace gameplay.gamerule.free.ui.component
{
	public interface IFreeUIShow
	{
		int GetKey();

		void Handle(IEventArgs args, SimpleProto b);
	}
}
