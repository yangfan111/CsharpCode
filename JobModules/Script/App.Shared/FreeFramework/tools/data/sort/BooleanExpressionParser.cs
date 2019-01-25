using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	public class BooleanExpressionParser : IExpressionParser
	{
		private const string ONLY_TRUE = "t";

		private const string ONLY_FLASE = "f";

		private const string TRUE_FALSE = "tf";

		private const string FALSE_TRUE = "ft";

		public override IValueOrder Parse(string type)
		{
			type = type.Trim().ToLower();
			if (ONLY_TRUE.Equals(type))
			{
				return new SequenceValueOrder(new string[] { "true" });
			}
			else
			{
				if (ONLY_FLASE.Equals(type))
				{
					return new SequenceValueOrder(new string[] { "false", "null" });
				}
				else
				{
					if (TRUE_FALSE.Equals(type))
					{
						return new SequenceValueOrder(new string[] { "true", "false", "null" });
					}
					else
					{
						if (FALSE_TRUE.Equals(type))
						{
							return new SequenceValueOrder(new string[] { "false", "null", "true" });
						}
					}
				}
			}
			return null;
		}

		public override bool CanParse(string value)
		{
			value = value.ToLower().Trim();
			return TRUE_FALSE.Equals(value) || FALSE_TRUE.Equals(value) || ONLY_FLASE.Equals(value) || ONLY_TRUE.Equals(value);
		}
	}
}
