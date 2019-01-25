using Sharpen;

namespace com.wd.free.para
{
	public interface IFields
	{
		IPara Get(string field);

		bool HasField(string field);

		string[] GetFields();
	}
}
