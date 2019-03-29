using Sharpen;
using com.wd.free.item;

namespace gameplay.gamerule.free.item
{
	public interface IItemLayout
	{
		int GetX(ItemPosition ip);

		int GetY(ItemPosition ip);
	}
}
