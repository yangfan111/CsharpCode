using Sharpen;
using com.wd.free.@event;
using com.wd.free.unit;
using gameplay.gamerule.free.ui;

namespace com.wd.free.map.position
{
	public interface IMapRegion
	{
		bool In(IEventArgs args, UnitPosition pos);

		bool InRectange(FreeUIUtil.Rectangle rec, IEventArgs args);

		UnitPosition GetCenter(IEventArgs args);

		bool IsDynamic();
	}
}
