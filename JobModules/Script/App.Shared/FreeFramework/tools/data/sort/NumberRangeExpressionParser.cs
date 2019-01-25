using Sharpen;
using com.cpkf.yyjd.tools.util.math;

namespace com.cpkf.yyjd.tools.data.sort
{
	public class NumberRangeExpressionParser : IExpressionParser
	{
		public const string TYPE = "nr";

		public override IValueOrder Parse(string type)
		{
			string[] ss = type.Split(UNDERLINE);
			RangeNumberValueOrder orderValue = new RangeNumberValueOrder();
			orderValue.SetStart(NumberUtil.GetInt(ss[1]));
			orderValue.SetEnd(NumberUtil.GetInt(ss[2]));
			orderValue.SetContainsStart(true);
			orderValue.SetContainsEnd(true);
			return orderValue;
		}

		public override bool CanParse(string value)
		{
			string[] ss = value.Split(UNDERLINE);
			if (ss.Length == 3)
			{
				return TYPE.Equals(ss[0].Trim().ToLower());
			}
			return false;
		}
	}
}
