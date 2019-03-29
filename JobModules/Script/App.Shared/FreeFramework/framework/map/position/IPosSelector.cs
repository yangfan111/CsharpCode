using Sharpen;
using com.wd.free.@event;
using com.wd.free.unit;

namespace com.wd.free.map.position
{
	public interface IPosSelector
	{
		UnitPosition Select(IEventArgs args);

		UnitPosition[] Select(IEventArgs args, int count);
	}
}
