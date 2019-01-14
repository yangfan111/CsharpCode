using System;
using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.util.math
{
	public class NumberUtil
	{
		private const int NUM_MIN_ASC = 48;

		private const int NUM_MAX_ASC = 57;

		private const int DOT = 46;

		public static string GetLenedDouble(double d, int len)
		{
			if (double.IsNaN(d) || double.IsInfinity(d))
			{
				return "N/A";
			}

		    return Math.Round(d, len).ToString();
		}

		public static int GetMax(int[] ints)
		{
			int max = int.MinValue;
			foreach (int @in in ints)
			{
				if (@in > max)
				{
					max = @in;
				}
			}
			return max;
		}

		public static int GetMin(int[] ints)
		{
			int min = int.MaxValue;
			foreach (int @in in ints)
			{
				if (@in < min)
				{
					min = @in;
				}
			}
			return min;
		}

		public static double[] GetAllDoubles(string value)
		{
			List<double> set = new List<double>();
			int start = -1;
			int end = -1;
			value = value.Replace("。", ".");
			char[] chars = value.ToCharArray();
			for (int i = 0; i < chars.Length; i++)
			{
				if ((chars[i] >= NUM_MIN_ASC && chars[i] <= NUM_MAX_ASC) || chars[i] == '.')
				{
					if (start == -1)
					{
						start = i;
					}
					if (i == chars.Length - 1)
					{
						end = i + 1;
					}
				}
				else
				{
					if ((end == -1 && start != -1))
					{
						end = i;
					}
				}
				if (start != -1 && end != -1)
				{
					set.Add(GetDouble(Sharpen.Runtime.Substring(value, start, end)));
					start = -1;
					end = -1;
				}
			}
			return Sharpen.Collections.ToArray(set, new double[] {  });
		}

		public static int GetInt(string @string)
		{
			try
			{
				return System.Convert.ToInt32(@string);
			}
			catch (Exception)
			{
				double d = GetDouble(@string);
				return (int)d;
			}
		}

		public static double GetDouble(string @string)
		{
			try
			{
				return double.Parse(@string);
			}
			catch (Exception)
			{
				try
				{
					if (@string != null)
					{
						char[] chars = @string.ToCharArray();
						bool start = false;
						int len = 0;
						for (int i = 0; i < chars.Length; i++)
						{
							if (chars[i] == '。')
							{
								chars[i] = '.';
							}
							if ((chars[i] < NUM_MIN_ASC || chars[i] > NUM_MAX_ASC) && chars[i] != DOT && chars[i] != ',' && chars[i] != '，')
							{
								chars[i] = ' ';
								if (start)
								{
									len = i;
									break;
								}
							}
							else
							{
								start = true;
							}
						}
						@string = new string(chars);
						if (len != 0)
						{
							@string = Sharpen.Runtime.Substring(@string, 0, len);
						}
						@string = @string.Replace(" ", string.Empty);
						@string = @string.Replace(",", string.Empty);
						@string = @string.Replace("，", string.Empty);
						if (@string.Equals(string.Empty))
						{
							return 0;
						}
						return double.Parse(@string);
					}
					else
					{
						return 0;
					}
				}
				catch (Exception)
				{
					return 0;
				}
			}
		}
	}
}
