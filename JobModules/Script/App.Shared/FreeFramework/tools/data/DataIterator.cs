using Sharpen;

namespace com.cpkf.yyjd.tools.data
{
	public interface DataIterator<T>
	{
		bool HasNext();

		T Next();

		void Close();

		void Reset();
	}
}
