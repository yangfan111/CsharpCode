using Sharpen;
using com.wd.free.@event;

namespace com.wd.free.unit
{
	public interface IUnitSelector
	{
		GameUnitSet Select(IEventArgs args, IGameUnit unit);
	}
}
