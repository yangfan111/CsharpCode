using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	public abstract class IExpressionParser
	{
		public const string UNDERLINE = "_";

		public abstract IValueOrder Parse(string value);

		public abstract bool CanParse(string value);
	}

	public static class IExpressionParserConstants
	{
	}
}
