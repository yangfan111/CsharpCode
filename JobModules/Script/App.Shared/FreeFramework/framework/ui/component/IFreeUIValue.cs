using Sharpen;
using com.wd.free.@event;

namespace gameplay.gamerule.free.ui.component
{
	public interface IFreeUIValue
	{
		int GetType();

		int GetSeq(IEventArgs args);

		int GetAutoStatus();

		int GetAutoIndex();

		object GetValue(IEventArgs args);
	}
}
