using System;
using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.util
{
	public class SortUtil
	{
		public static LinkedHashMap<object, object> GetTopsByValue<T>(MyDictionary<T,int> dic, int top)
		{
			int[] objs = Sharpen.Collections.ToArray(dic.Values);
			Arrays.Sort(objs);
			HashSet<object> addedSet = new HashSet<object>();
			LinkedHashMap<object, object> map = new LinkedHashMap<object, object>();
			for (int i = 0; i < MyMath.Min(objs.Length, top); i++)
			{
				foreach (T key in dic.Keys)
				{
					if (dic[key].Equals(objs[objs.Length - i - 1]) && !addedSet.Contains(key))
					{
						map[key] = dic[key];
						addedSet.Add(key);
					}
				}
			}
			return map;
		}
	}
}
