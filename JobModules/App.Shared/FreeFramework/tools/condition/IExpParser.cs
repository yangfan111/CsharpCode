using Sharpen;

namespace com.cpkf.yyjd.tools.condition
{
	public interface IExpParser<T>
		where T : IClausable
	{
		T Parse(string exp);
	}
}
