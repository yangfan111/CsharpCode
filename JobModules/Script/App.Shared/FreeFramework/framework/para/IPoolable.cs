using Sharpen;

namespace com.wd.free.para
{
	public interface IPoolable
	{
		IPoolable Copy();

		IPoolable Borrow();

		void Recycle();

		bool IsTemp();
	}
}
