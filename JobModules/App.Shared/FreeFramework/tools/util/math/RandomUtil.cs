using System;
using System.Collections.Generic;
using Sharpen;
using Core.Utils;

namespace com.cpkf.yyjd.tools.util.math
{
	public class RandomUtil
	{
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RandomUtil));

        private static Random random = new Random();

        public static void SetSeed(int seed)
        {
            random = new Random(seed);
            _logger.InfoFormat("random seed: {0}", seed);
        }

		public static int Random(int min, int max)
		{
			return min + random.Next(max - min + 1);
		}

		public static int RandomWithPro(int min, int[] priority)
		{
			int[] ii = RandomWithPro(min, priority, 1);
			if (ii.Length > 0)
			{
				return ii[0];
			}
			return min;
		}

		public static int[] RandomWithPro(int min, int[] priority, int count)
		{
			int rCount = 0;
			int totalPro = 0;
			int trueCount = 0;
			for (int i = 0; i < priority.Length; i++)
			{
				totalPro = totalPro + priority[i];
				if (priority[i] > 0)
				{
					trueCount++;
				}
			}
			int[] result = new int[Math.Min(count, trueCount)];
			int[] clone = new int[priority.Length];
			System.Array.Copy(priority, 0, clone, 0, priority.Length);
			while (rCount < result.Length && totalPro > 0)
			{
				result[rCount] = RandomWithProRepeat(min, clone, 1)[0];
				totalPro = totalPro - clone[result[rCount] - min];
				clone[result[rCount] - min] = 0;
				rCount = rCount + 1;
			}
			return result;
		}

		/// <summary>例如：min=1, priority=new int[]{1,2,3}, count=2; 则数字1,2,3按照 1/6,2/6,3/6的概率出现</summary>
		/// <param name="min"/>
		/// <param name="count"/>
		/// <param name="priority"/>
		/// <returns/>
		public static int[] RandomWithProRepeat(int min, int[] priority, int count)
		{
			int total = 0;
			for (int i = 0; i < priority.Length; i++)
			{
				total = total + priority[i];
			}
			int[] allInt = new int[total];
			total = 0;
			for (int i_1 = 0; i_1 < priority.Length; i_1++)
			{
				for (int j = total; j < total + priority[i_1]; j++)
				{
					allInt[j] = i_1;
				}
				total = total + priority[i_1];
			}
			int[] result = new int[count];
			for (int i_2 = 0; i_2 < count; i_2++)
			{
				result[i_2] = allInt[Random(0, total - 1)] + min;
			}
			return result;
		}

		public static int[] RandomRepeat(int min, int max, int count)
		{
			int[] result = new int[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = Random(min, max);
			}
			return result;
		}

		public static int[] Random(int min, int max, int count)
		{
			int[] result = new int[Math.Min(count, max - min + 1)];
			int index = 0;
			LinkedHashSet<int> set = new LinkedHashSet<int>();
			for (int i = min; i <= max; i++)
			{
				set.AddItem(i);
			}
			while (index < result.Length)
			{
				int r = Random(0, set.Count - 1);
				if (r < set.Count)
				{
					result[index] = GetInt(set, r);
					set.Remove(result[index]);
				}
				else
				{
					break;
				}
				index++;
			}
			return result;
		}

		private static int GetInt(ICollection<int> set, int index)
		{
			int i = 0;
			foreach (int @in in set)
			{
				if (i == index)
				{
					return @in;
				}
				i++;
			}
			return 0;
		}
	}
}
