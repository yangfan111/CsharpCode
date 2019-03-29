using System;
using Sharpen;

namespace com.cpkf.yyjd.tools.condition
{
	[System.Serializable]
	public class ConditionParseException : Exception
	{
		private const long serialVersionUID = -6679347737687740692L;

		public ConditionParseException()
		{
		}

		public ConditionParseException(string message)
			: base(message)
		{
		}

		public ConditionParseException(Exception e)
			: base(e.Message)
		{
		}
	}
}
