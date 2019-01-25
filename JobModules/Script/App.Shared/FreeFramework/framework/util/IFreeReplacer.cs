using Sharpen;
using com.wd.free.@event;

namespace com.wd.free.util
{
	public interface IFreeReplacer
	{
		bool CanHandle(string exp, IEventArgs args);

		string Replace(string exp, IEventArgs args);
	}
}
