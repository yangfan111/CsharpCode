using System;
using System.Collections.Generic;
using Sharpen;

namespace com.cpkf.yyjd.tools.util
{
	public class DateUtil
	{
		public enum TimeUnit
		{
			day,
			month,
			week,
			hour,
			minute,
			second
		}

		public static int GetIntyyyyMMdd(DateTime date)
		{
			SimpleDateFormat formatter = new SimpleDateFormat("yyyyMMdd");
			return System.Convert.ToInt32(formatter.Format(date));
		}

		public static string GetDateString(DateTime date)
		{
			SimpleDateFormat formatter = new SimpleDateFormat("yyyy-MM-dd");
			return formatter.Format(date);
		}

		public static string GetTodayDate()
		{
			return GetDateAndTimeString(new DateTime(), "yyyy-MM-dd");
		}

		public static string GetTodayDateTime()
		{
			return GetDateAndTimeString(new DateTime(), "yyyy-MM-dd HH:mm:ss");
		}

		public static string GetTimeString(DateTime date)
		{
			SimpleDateFormat formatter = new SimpleDateFormat("HH:mm:ss");
			return formatter.Format(date);
		}

		public static string GetDateAndTimeString(DateTime date)
		{
			SimpleDateFormat formatter = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
			return formatter.Format(date);
		}

		public static string GetDateAndTimeString(DateTime date, string format)
		{
			SimpleDateFormat formatter = new SimpleDateFormat(format);
			return formatter.Format(date);
		}

		public static DateTime GetDateFromString(string date)
		{
			SimpleDateFormat formatter = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
			try
			{
				return new DateTime(formatter.Parse(date).Millisecond);
			}
			catch (ParseException)
			{
			}

			return DateTime.Now;
		}

		public static DateTime GetDateFromString(string date, string format)
		{
			SimpleDateFormat formatter = new SimpleDateFormat(format);
			try
			{
				return new DateTime(formatter.Parse(date).Millisecond);
			}
			catch (Exception)
			{
			}
			return DateTime.Now;
		}

		public static double GetTimeByMinute(DateTime date1, DateTime date2)
		{
			return MyMath.Abs((date1.Millisecond - date2.Millisecond)) / 60000;
		}

		public static double GetTimeBySecond(DateTime date1, DateTime date2)
		{
			return MyMath.Abs((date1.Millisecond - date2.Millisecond)) / 1000;
		}

		public static double GetTimeByHour(DateTime date1, DateTime date2)
		{
			return MyMath.Abs((date1.Millisecond - date2.Millisecond)) / 3600000;
		}

		public static double GetTimeByDay(DateTime date1, DateTime date2)
		{
			return MyMath.Abs((date1.Millisecond - date2.Millisecond)) / 86400000;
		}

		public static double GetTimeByWeek(DateTime date1, DateTime date2)
		{
			return MyMath.Abs((date1.Millisecond - date2.Millisecond)) / 604800000;
		}

		public static double GetTimeByMonth(DateTime date1, DateTime date2)
		{
			return MyMath.Abs((date1.Millisecond - date2.Millisecond)) / 2592000000;
		}

		public static double GetTimeValue(DateUtil.TimeUnit tu, DateTime date1, DateTime date2)
		{
			switch (tu)
			{
				case DateUtil.TimeUnit.day:
				{
					return GetTimeByDay(date1, date2);
				}

				case DateUtil.TimeUnit.month:
				{
					return GetTimeByMonth(date1, date2);
				}

				case DateUtil.TimeUnit.week:
				{
					return GetTimeByWeek(date1, date2);
				}

				case DateUtil.TimeUnit.hour:
				{
					return GetTimeByHour(date1, date2);
				}

				case DateUtil.TimeUnit.minute:
				{
					return GetTimeByMinute(date1, date2);
				}

				case DateUtil.TimeUnit.second:
				{
					return GetTimeBySecond(date1, date2);
				}

				default:
				{
					return 0;
				}
			}
		}

		public static long GetMiniseconds(DateUtil.TimeUnit tu, double value)
		{
			double d;
			switch (tu)
			{
				case DateUtil.TimeUnit.day:
				{
					d = 86400000 * value;
					break;
				}

				case DateUtil.TimeUnit.month:
				{
					d = 2592000000 * value;
					break;
				}

				case DateUtil.TimeUnit.week:
				{
					d = 604800000 * value;
					break;
				}

				case DateUtil.TimeUnit.hour:
				{
					d = 3600000 * value;
					break;
				}

				case DateUtil.TimeUnit.minute:
				{
					d = 60000 * value;
					break;
				}

				case DateUtil.TimeUnit.second:
				{
					d = 1000 * value;
					break;
				}

				default:
				{
					d = 0;
					break;
				}
			}
			return (long)d;
		}

		public static DateTime GetBeforeDay(DateUtil.TimeUnit tu, double value)
		{
			double time = 1;
			switch (tu)
			{
				case DateUtil.TimeUnit.day:
				{
					time = 86400000;
					break;
				}

				case DateUtil.TimeUnit.month:
				{
					time = value * 2592000000;
					break;
				}

				case DateUtil.TimeUnit.week:
				{
					time = value * 604800000;
					break;
				}

				case DateUtil.TimeUnit.hour:
				{
					time = value * 3600000;
					break;
				}

				case DateUtil.TimeUnit.minute:
				{
					time = value * 60000;
					break;
				}

				case DateUtil.TimeUnit.second:
				{
					time = value * 1000;
					break;
				}
			}
			return new DateTime(Runtime.CurrentTimeMillis() - (long)time);
		}

		public static int GetDateCount(DateUtil.TimeUnit tu, int value)
		{
			switch (tu)
			{
				case DateUtil.TimeUnit.day:
				{
					return value * 1;
				}

				case DateUtil.TimeUnit.month:
				{
					return value * 30;
				}

				case DateUtil.TimeUnit.week:
				{
					return value * 7;
				}

				default:
				{
					break;
				}
			}
			return 1;
		}

		public static DateTime GetClosestDate(DateTime date, IList<DateTime> list)
		{
			long min = long.MaxValue;
		    DateTime result = DateTime.MinValue;
			foreach (DateTime d in list)
			{
				long value = Math.Abs(date.Millisecond - d.Millisecond);
				if (min > value)
				{
					min = value;
					result = d;
				}
			}
			if (list.Count > 0 && result.Millisecond < list[0].Millisecond)
			{
				result = DateTime.MinValue;
			}
			return result;
		}

		public static IList<DateTime> GetDateList(DateUtil.TimeUnit tu, int value, int displayCount, DateTime earliestDay)
		{
			double count = MyMath.Min(GetDateCount(tu, value), GetTimeValue(DateUtil.TimeUnit.day, new DateTime(), earliestDay));
			double fre = count / displayCount;
			if (fre < 1)
			{
				displayCount = (int)count;
				fre = 1;
			}
			IList<DateTime> list = new List<DateTime>();
			for (int i = 1; i < displayCount + 1; i++)
			{
				list.Add(GetBeforeDay(DateUtil.TimeUnit.day,(displayCount - i) * fre));
			}
			return list;
		}

		public static bool IsBetweenTime(string start, string end)
		{
			string now = GetTimeString(new DateTime());
			return string.CompareOrdinal(now, start) > 0 && string.CompareOrdinal(now, end) < 0;
		}

		/// <exception cref="java.text.ParseException"/>
		public static bool IsYeaterday(DateTime oldTime)
		{
			DateTime newTime = new DateTime();
			// 将下面的 理解成 yyyy-MM-dd 00：00：00 更好理解点
			SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd");
			string todayStr = format.Format(newTime);
			DateTime today = format.Parse(todayStr);
			// 昨天 86400000=24*60*60*1000 一天
			if ((today.Millisecond - oldTime.Millisecond) > 0 && (today.Millisecond - oldTime.Millisecond) <= 86400000)
			{
				return true;
			}
			return false;
		}

		/// <exception cref="java.text.ParseException"/>
		public static bool IsDaybeforeYeaterday(DateTime oldTime)
		{
			DateTime newTime = new DateTime();
			// 将下面的 理解成 yyyy-MM-dd 00：00：00 更好理解点
			SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd");
			string todayStr = format.Format(newTime);
			DateTime today = format.Parse(todayStr);
			// 昨天 86400000=24*60*60*1000 一天
			if ((today.Millisecond - oldTime.Millisecond) > 86400000 && (today.Millisecond - oldTime.Millisecond) <= 86400000 * 2)
			{
				return true;
			}
			return false;
		}

		/// <exception cref="java.text.ParseException"/>
		public static bool IsToady(DateTime oldTime)
		{
			DateTime newTime = new DateTime();
			// 将下面的 理解成 yyyy-MM-dd 00：00：00 更好理解点
			SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd");
			string todayStr = format.Format(newTime);
			DateTime today = format.Parse(todayStr);
			// 昨天 86400000=24*60*60*1000 一天
			if ((today.Millisecond - oldTime.Millisecond) <= 0)
			{
				// 至少是今天
				return true;
			}
			return false;
		}

		/// <exception cref="java.text.ParseException"/>
		public static int GetWeeksAgo(DateTime oldTime)
		{
			DateTime newTime = new DateTime();
			SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd");
			string todayStr = format.Format(newTime);
			DateTime today = format.Parse(todayStr);
			// 86400000=24*60*60*1000 一天
			int n = (int)((today.Millisecond - oldTime.Millisecond) / (86400000 * 7));
			return n;
		}

		/// <exception cref="java.text.ParseException"/>
		public static int GetDaysAgo(DateTime oldTime)
		{
			DateTime newTime = new DateTime();
			SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd");
			string todayStr = format.Format(newTime);
			DateTime today = format.Parse(todayStr);
			// 86400000=24*60*60*1000 一天
			int n = (int)((today.Millisecond - oldTime.Millisecond) / 86400000);
			return n;
		}
	}
}
