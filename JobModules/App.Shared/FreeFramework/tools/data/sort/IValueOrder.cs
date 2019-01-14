using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	[System.Serializable]
	public abstract class IValueOrder
	{
		public const string NULL = "null";

		public const string NOT_NULL = "notnull";

		public const int REMOVE = -1;

		public abstract int[] Sort(IList<MarkedData> datas, string feature);
	}

	public static class IValueOrderConstants
	{
	}
}
