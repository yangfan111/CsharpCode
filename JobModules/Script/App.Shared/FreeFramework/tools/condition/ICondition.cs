using Sharpen;

namespace com.cpkf.yyjd.tools.condition
{
	public interface ICondition<T> : IExpParser<ICondition<T>>, IClausable
	{
		bool Meet(T t);

		ICondition<T> Parse(string expression);
	}
}
