using System.Collections.Generic;
using Sharpen;

namespace com.wd.free.para
{
	public class ParaUtil
	{
		[System.NonSerialized]
		private static MyDictionary<string, IPara> map;

		static ParaUtil()
		{
			map = new MyDictionary<string, IPara>();
			map["string"] = new StringPara();
			map["int"] = new IntPara();
			map["long"] = new LongPara();
			map["bool"] = new BoolPara();
			map["float"] = new FloatPara();
			map["double"] = new DoublePara();
		}

		public static IPara GetPara(string type)
		{
            if (map.ContainsKey(type))
            {
                return map[type];
            }

            return null;
		}

		public static bool IsBasicPara(IPara p)
		{
			return p is IntPara || p is StringPara || p is BoolPara || p is LongPara || p is FloatPara || p is DoublePara;
		}
	}
}
