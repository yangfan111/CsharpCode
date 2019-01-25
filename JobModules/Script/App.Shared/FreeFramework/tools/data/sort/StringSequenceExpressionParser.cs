using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	public class StringSequenceExpressionParser : IExpressionParser
	{
		public const string TYPE = "ss";

		public override IValueOrder Parse(string type)
		{
			string[] ss = type.Split(UNDERLINE);
			IList<string> values = new List<string>();
			for (int i = 1; i < ss.Length; i++)
			{
				values.Add(ss[i]);
			}
			return new SequenceValueOrder(Sharpen.Collections.ToArray(values, new string[] {  }));
		}

		public override bool CanParse(string value)
		{
			string[] ss = value.Split(UNDERLINE);
			if (ss.Length > 1)
			{
				return TYPE.Equals(ss[0].Trim().ToLower());
			}
			return false;
		}
	}
}
