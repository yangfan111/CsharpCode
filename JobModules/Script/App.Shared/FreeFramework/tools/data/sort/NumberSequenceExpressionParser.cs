using Sharpen;
using com.cpkf.yyjd.tools.util.math;

namespace com.cpkf.yyjd.tools.data.sort
{
	public class NumberSequenceExpressionParser : IExpressionParser
	{
		public const string TYPE = "ns";

		public override IValueOrder Parse(string type)
		{
			string[] ss = type.Split(UNDERLINE);
			NumberSequenceValueOrder orderValue = new NumberSequenceValueOrder();
			int i1 = NumberUtil.GetInt(ss[1]);
			int i2 = NumberUtil.GetInt(ss[2]);
			if (i1 > i2)
			{
				orderValue.SetMin(i2);
				orderValue.SetMax(i1);
				orderValue.SetDesc(true);
			}
			else
			{
				orderValue.SetMin(i1);
				orderValue.SetMax(i2);
				orderValue.SetDesc(false);
			}
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
