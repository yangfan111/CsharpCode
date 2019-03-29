using Sharpen;
using com.wd.free.@event;
using com.wd.free.para;

namespace com.wd.free.para.exp
{
	public interface IExpReplacer
	{
		bool CanHandle(string exp, IEventArgs args);

		IPara Replace(string exp, IEventArgs args);
	}
}
