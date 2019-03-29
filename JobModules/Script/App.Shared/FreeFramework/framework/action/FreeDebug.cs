using System.Collections.Generic;
using Sharpen;

namespace com.wd.free.action
{
	public class FreeDebug
	{
		private static LinkedHashMap<string, long> time = new LinkedHashMap<string, long>();

		private static MyDictionary<string, int> count = new MyDictionary<string, int>();

		private static int index = 0;

		public static void Reset()
		{
			index = 0;
			time.Clear();
			count.Clear();
		}

		public static long Start(string key)
		{
			long s = Runtime.NanoTime();
			time[s + "_" + key] = 0L;
			count[s + "_" + key] = index;
			index++;
			return s;
		}

		public static void Stop(string key, long s)
		{
			time[s + "_" + key] = Runtime.NanoTime() - s;
			index--;
		}

		public static void Print(int min)
		{
			foreach (string key in time.Keys)
			{
				int i = count[key];
				if (time[key] > min)
				{
					System.Console.Error.WriteLine(GetBlank(i) + Sharpen.Runtime.Substring(key, key.IndexOf("_") + 1) + ":" + time[key] / 10000);
				}
			}
		}

		private static string GetBlank(int index)
		{
			string s = string.Empty;
			for (int i = 0; i < index; i++)
			{
				s = s + "    ";
			}
			return s;
		}
	}
}
