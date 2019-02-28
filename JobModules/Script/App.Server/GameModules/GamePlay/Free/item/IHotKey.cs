using Sharpen;
using com.wd.free.@event;
using com.wd.free.item;

namespace gameplay.gamerule.free.item
{
	public interface IHotKey
	{
		string GetHotKey(IEventArgs args, ItemPosition ip);
	}
}
