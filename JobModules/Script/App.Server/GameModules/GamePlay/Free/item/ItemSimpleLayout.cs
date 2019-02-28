using Sharpen;
using com.wd.free.item;

namespace gameplay.gamerule.free.item
{
	public class ItemSimpleLayout : IItemLayout
	{
		private int x;

		private int y;

		private int width;

		private int height;

		private int column;

		private int row;

		public virtual int GetX(ItemPosition ip)
		{
			return x + width / column * ip.GetX();
		}

		public virtual int GetY(ItemPosition ip)
		{
			return y + height / row * ip.GetY();
		}
	}
}
