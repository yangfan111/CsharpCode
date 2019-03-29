using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	public class CharOrderExpressionParser : IExpressionParser
	{
		public const string CHAR = "char";

		public override IValueOrder Parse(string type)
		{
			type = type.Trim().ToLower();
			if (CHAR.Equals(type))
			{
				return new CharValueOrder();
			}
			return null;
		}

		public override bool CanParse(string value)
		{
			return CHAR.Equals(value);
		}
	}
}
