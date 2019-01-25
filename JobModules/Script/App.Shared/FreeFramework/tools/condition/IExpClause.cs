using Sharpen;

namespace com.cpkf.yyjd.tools.condition
{
	public interface IExpClause : IClausable
	{
		void AddAnd(IClausable t, bool negative);

		void AddOr(IClausable t, bool negative);

		void AddApend(IClausable t, bool negative);

		IExpClause Clone();
	}
}
