using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpen;

namespace com.cpkf.yyjd.tools.util
{
	/// <summary>字符串工具类</summary>
	/// <author>Dave</author>
	public class StringUtil
	{
		public const string SPLITER_RECORD = "원";

		public const string SPLITER_FIELD = "빈";

		public const string UNIQUE_STRING = "어";

		private static readonly Pattern IP_PATTERN = Pattern.Compile("\\d+\\.\\d+\\.\\d+\\.\\d+");

		private static Pattern chinesePattern;

		static StringUtil()
		{
			chinesePattern = Pattern.Compile("[\u4e00-\u9fa5]");
		}

		public static string GetStandardString(string @string)
		{
			if (IsNullOrEmpty(@string))
			{
				return string.Empty;
			}
			return QBchange(@string.ToLower()).Trim();
		}

		public static string Unicode10ToWord(string unicode)
		{
			try
			{
				if (unicode.Contains("&#"))
				{
					string[] ss = unicode.Split("&#");
					string newString = string.Empty;
					foreach (string s in ss)
					{
						if (ContainsNumber(s) && s.Contains(";"))
						{
							int index = s.IndexOf(";");
							string number = Sharpen.Runtime.Substring(s, 0, index);
							string s1 = string.Empty;
							int a = System.Convert.ToInt32(number, 10);
							s1 = s1 + (char)a;
							newString = newString + s1 + Sharpen.Runtime.Substring(s, index + 1);
						}
						else
						{
							newString = newString + s;
						}
					}
					return newString;
				}
				else
				{
					return unicode;
				}
			}
			catch (Exception)
			{
				return unicode;
			}
		}

		public static bool StandardEquals(string s1, string s2)
		{
			return GetStandardString(s1).Equals(GetStandardString(s2));
		}

		public static bool ContainsNumber(string source)
		{
			if (source == null)
			{
				return false;
			}
			foreach (char c in source.ToCharArray())
			{
				if (c >= '0' && c <= '9')
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>返回Pascal化的字符串，即将字符串的首字母大写</summary>
		/// <param name="string">字符串</param>
		/// <returns>首字母大写的字符串</returns>
		public static string GetPascalString(string @string)
		{
			return Sharpen.Runtime.Substring(@string, 0, 1).ToUpper() + Sharpen.Runtime.Substring(@string, 1);
		}

		/// <summary>保留分隔符的划分方法</summary>
		/// <param name="string"/>
		/// <param name="rexs"/>
		/// <returns/>
		public static string[] SplitRemainRex(string @string, string[] rexs)
		{
			Arrays.Sort(rexs, GetLenComparator());
			IList<string> list = new List<string>();
			HashSet<string> set = new HashSet<string>();
			foreach (string rex in rexs)
			{
				set.Add(rex);
			}
			list.Add(@string);
			for (int i = 0; i < rexs.Length; i++)
			{
				IList<string> newList = new List<string>();
				foreach (string s in list)
				{
					if (set.Contains(s))
					{
						newList.Add(s);
					}
					else
					{
						Sharpen.Collections.AddAll(newList, Arrays.AsList(SplitRemainRex(s, rexs[i])));
					}
				}
				list = newList;
			}
			return Sharpen.Collections.ToArray(list, new string[] {  });
		}

		private static IComparer<string> GetLenComparator()
		{
			return new _Comparator_133();
		}

		private sealed class _Comparator_133 : IComparer<string>
		{
			public _Comparator_133()
			{
			}

			public int Compare(string o1, string o2)
			{
				return o2.Length - o1.Length;
			}
		}

		/// <summary>保留分隔符的划分方法</summary>
		/// <param name="source"/>
		/// <param name="rex"/>
		/// <returns/>
		public static string[] SplitRemainRex(string source, string rex)
		{
			string[] ss = Split(source, rex);
			IList<string> list = new List<string>();
			for (int i = 0; i < ss.Length; i++)
			{
				if (ss[i].Length > 0)
				{
					list.Add(ss[i].Trim());
				}
				if (i < ss.Length - 1)
				{
					list.Add(rex);
				}
			}
			return Sharpen.Collections.ToArray(list, new string[] {  });
		}

		/// <summary>
		/// String.split的行为：以字符어，分割어어时，返回的值为String[0]
		/// 返回标识符的个数+1的String数组
		/// </summary>
		/// <param name="string"/>
		/// <param name="rex"/>
		/// <returns/>
		public static string[] Split(string @string, string rex)
		{
			int[] indexs = GetAllIndex(@string, rex);
			IList<string> list = new List<string>();
			for (int i = 0; i < indexs.Length; i++)
			{
				if (i == 0)
				{
					list.Add(Sharpen.Runtime.Substring(@string, 0, indexs[i]));
				}
				if (i < indexs.Length - 1)
				{
					list.Add(Sharpen.Runtime.Substring(@string, indexs[i] + rex.Length, indexs[i + 1]));
				}
				else
				{
					if (i == indexs.Length - 1)
					{
						list.Add(Sharpen.Runtime.Substring(@string, indexs[i] + rex.Length));
					}
				}
			}
			if (indexs.Length == 0)
			{
				list.Add(@string);
			}
			return Sharpen.Collections.ToArray(list, new string[] {  });
		}

		public static string[] SplitWithoutBlank(string @string, string rex)
		{
			int[] indexs = GetAllIndex(@string, rex);
			IList<string> list = new List<string>();
			for (int i = 0; i < indexs.Length; i++)
			{
				if (i == 0)
				{
					if (!StringUtil.IsNullOrEmpty(Sharpen.Runtime.Substring(@string, 0, indexs[i])))
					{
						list.Add(Sharpen.Runtime.Substring(@string, 0, indexs[i]));
					}
				}
				if (i < indexs.Length - 1)
				{
					if (!StringUtil.IsNullOrEmpty(Sharpen.Runtime.Substring(@string, indexs[i] + rex.Length, indexs[i + 1])))
					{
						list.Add(Sharpen.Runtime.Substring(@string, indexs[i] + rex.Length, indexs[i + 1]));
					}
				}
				else
				{
					if (i == indexs.Length - 1)
					{
						if (!StringUtil.IsNullOrEmpty(Sharpen.Runtime.Substring(@string, indexs[i] + rex.Length)))
						{
							list.Add(Sharpen.Runtime.Substring(@string, indexs[i] + rex.Length));
						}
					}
				}
			}
			if (indexs.Length == 0)
			{
				if (!StringUtil.IsNullOrEmpty(@string))
				{
					list.Add(@string);
				}
			}
			return Sharpen.Collections.ToArray(list, new string[] {  });
		}

		public static string[] Split(string @string, string[] rexs)
		{
			Arrays.Sort(rexs, GetLenComparator());
			IList<string> list = new List<string>();
			list.Add(@string);
			for (int i = 0; i < rexs.Length; i++)
			{
				IList<string> newList = new List<string>();
				foreach (string s in list)
				{
					Sharpen.Collections.AddAll(newList, Arrays.AsList(Split(s, rexs[i])));
				}
				list = newList;
			}
			return Sharpen.Collections.ToArray(list, new string[] {  });
		}

		/// <summary>判断字符是否为中文字符</summary>
		/// <param name="ch">字符</param>
		/// <returns>true则是中文字符，false不是中文字符</returns>
		public static bool IsChineseCharacter(char ch)
		{
			string @string = ch.ToString();
			return chinesePattern.Matcher(@string).Find();
		}

		/// <summary>把pascal化的字符的首字母变小写</summary>
		/// <param name="string">字符串</param>
		/// <returns>去pascal化的字符串</returns>
		public static string GetStringFromPascal(string @string)
		{
			return Sharpen.Runtime.Substring(@string, 0, 1).ToLower() + Sharpen.Runtime.Substring(@string, 1);
		}

		/// <summary>把字符串列用空格连接成一个字符串</summary>
		/// <param name="strings">字符串列</param>
		/// <returns>字符串</returns>
		public static string GetStringFromStrings(string[] strings)
		{
			StringBuilder buf = new StringBuilder();
			foreach (string @string in strings)
			{
				buf.Append(@string);
				buf.Append(" ");
			}
			return buf.ToString().Trim();
		}

		public static string GetStringFromStringsWithUnique(string[] strings)
		{
			StringBuilder buf = new StringBuilder();
			if (strings.Length > 0)
			{
				for (int i = 0; i < strings.Length - 1; i++)
				{
					buf.Append(strings[i]);
					buf.Append(UNIQUE_STRING);
				}
				buf.Append(strings[strings.Length - 1]);
			}
			return buf.ToString().Trim();
		}

		public static string GetStringFromStrings(IList<string> list, string spliter)
		{
			return GetStringFromStrings(list.ToArray(), spliter);
		}

		/// <summary>把字符串列用指定的方式连接成一个字符串</summary>
		/// <param name="strings">字符串列</param>
		/// <returns>字符串</returns>
		public static string GetStringFromStrings(string[] strings, string spliter)
		{
			if (strings == null || strings.Length == 0)
			{
				return string.Empty;
			}
			else
			{
				if (spliter == null)
				{
					spliter = string.Empty;
				}
				StringBuilder buf = new StringBuilder();
				foreach (string @string in strings)
				{
					buf.Append(@string);
					buf.Append(spliter);
				}
				return Sharpen.Runtime.Substring(buf.ToString(), 0, buf.ToString().Length - spliter.Length);
			}
		}

		public static string[] GetStringsFromString(string stirng, string spliter)
		{
			return stirng.Split(spliter);
		}

		public static bool IsNullOrEmpty(string @string)
		{
			return @string == null || @string.Trim().Length == 0;
		}

		/// <summary>去掉字符串中的空格和tab</summary>
		/// <param name="string">字符串</param>
		/// <returns>去掉后的值</returns>
		public static string RemoveWhiteSpace(string @string)
		{
			if (IsNullOrEmpty(@string))
			{
				return string.Empty;
			}
			else
			{
				@string = @string.Replace(" ", string.Empty);
				@string = @string.Replace("\t", string.Empty);
				return @string;
			}
		}

		/// <summary>判断是否为英文或者数字字符串</summary>
		/// <param name="string">字符串</param>
		/// <returns>true则是，false则否</returns>
		public static bool IsCharOrNumberString(string @string)
		{
			char[] cs = @string.ToCharArray();
			foreach (char c in cs)
			{
				if (!char.IsDigit(c) && !IsEnglishCharacter(c))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>判断是否为数字字符串</summary>
		/// <param name="string">字符串</param>
		/// <returns>true则是，false则否</returns>
		public static bool IsNumberString(string @string)
		{
			if (StringUtil.IsNullOrEmpty(@string))
			{
				return false;
			}
			char[] cs = @string.ToCharArray();
			foreach (char c in cs)
			{
				if (!char.IsDigit(c) && c != '.' && c != '-')
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsIpAddress(string str)
		{
			Matcher matcher = IP_PATTERN.Matcher(str);
			if (matcher.Matches())
			{
				return true;
			}
			return false;
		}

		/// <summary>判断是否为英文字符</summary>
		/// <param name="ch">字符</param>
		/// <returns>true为英文字符，false则不是</returns>
		public static bool IsEnglishCharacter(char ch)
		{
			return (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z');
		}

		/// <summary>判断是否为英文字符</summary>
		/// <param name="ch">字符</param>
		/// <returns>true为英文字符，false则不是</returns>
		public static bool IsEnglishString(string @string)
		{
			@string = GetStandardString(@string);
			char[] cs = @string.ToCharArray();
			foreach (char c in cs)
			{
				if (c < 'a' || c > 'z')
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsEnglishOrNumberCharacter(char ch)
		{
			return IsEnglishCharacter(ch) || char.IsDigit(ch);
		}

		public static bool IsNumberCharacter(char ch)
		{
			return char.IsDigit(ch);
		}

		public static bool ContainsChinese(string word)
		{
			if (!IsNullOrEmpty(word))
			{
				for (int i = 0; i < word.Length; i++)
				{
					if (IsChineseCharacter(word[i]))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public static string RemoveParenthesis(string source)
		{
			return RemoveParenthesis(source, new string[] { "(" }, new string[] { ")" });
		}

		public static string RemoveParenthesis(string source, string[] starts, string[] ends)
		{
			for (int i = 0; i < starts.Length; i++)
			{
				source = source.Replace(starts[i], "(");
				source = source.Replace(ends[i], ")");
			}
			int[] si = GetAllIndex(source, "(");
			int[] ei = GetAllIndex(source, ")");
			string ns = string.Empty;
			if (si.Length > 0)
			{
				int es = 0;
				bool find = false;
				foreach (int e in ei)
				{
					if (e > si[0])
					{
						find = true;
						break;
					}
					else
					{
						es++;
					}
				}
				int lastposition = 0;
				if (find)
				{
					for (int i_1 = 0; i_1 < si.Length && es < ei.Length; i_1++)
					{
						if (si[i_1] >= lastposition)
						{
							ns = ns + " " + Sharpen.Runtime.Substring(source, lastposition, si[i_1]);
							lastposition = ei[es] + 1;
							es++;
						}
					}
				}
				ns = ns + " " + Sharpen.Runtime.Substring(source, lastposition);
				return ns.Trim();
			}
			return source;
		}

		public static bool ContainsEnglishOrNumber(string word)
		{
			if (!IsNullOrEmpty(word))
			{
				for (int i = 0; i < word.Length; i++)
				{
					if (IsEnglishOrNumberCharacter(word[i]))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public static bool ContainsEnglish(string word)
		{
			if (!IsNullOrEmpty(word))
			{
				for (int i = 0; i < word.Length; i++)
				{
					if (IsEnglishCharacter(word[i]))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public static bool IsAllChineseCharacter(string word)
		{
			if (!IsNullOrEmpty(word))
			{
				for (int i = 0; i < word.Length; i++)
				{
					if (!IsChineseCharacter(word[i]))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		/// <summary>返回所有的rex在souce串中独立出现的位置 此处独立的意思为英文和数字的两边不能为英文和数字</summary>
		/// <param name="source">源字符串</param>
		/// <param name="rex">表达式</param>
		/// <returns>所有的位置信息</returns>
		public static int[] GetAllInDependentIndex(string source, string rex)
		{
			List<int> list = new List<int>();
			if (IsCharOrNumberString(rex))
			{
				int position = 0;
				while (position < source.Length)
				{
					int index = source.IndexOf(rex, position);
					if (index > -1)
					{
						if (index > 0 && index + rex.Length < source.Length)
						{
							if (!IsEnglishOrNumberCharacter(source[index - 1]) && !IsEnglishOrNumberCharacter(source[index + rex.Length]))
							{
								list.Add(source.IndexOf(rex, position));
							}
						}
						else
						{
							if (index > 0)
							{
								if (!IsEnglishOrNumberCharacter(source[index - 1]))
								{
									list.Add(source.IndexOf(rex, position));
								}
							}
							else
							{
								if (index + rex.Length < source.Length)
								{
									// if (!isEnglishOrNumberCharacter(source.charAt(index
									// + rex.length()))) {
									list.Add(source.IndexOf(rex, position));
								}
								else
								{
									// }
									if (index + rex.Length == source.Length)
									{
										list.Add(source.IndexOf(rex, position));
									}
								}
							}
						}
						position = index + 1;
					}
					else
					{
						break;
					}
				}
			}
			else
			{
				return GetAllIndex(source, rex);
			}
			int[] ins = new int[list.Count];
			for (int i = 0; i < ins.Length; i++)
			{
				ins[i] = list[i];
			}
			return ins;
		}

		/// <summary>返回所有的rex在souce串中出现的位置</summary>
		/// <param name="source">源字符串</param>
		/// <param name="rex">表达式</param>
		/// <returns>所有的位置信息</returns>
		public static int[] GetAllIndex(string source, string rex)
		{
			List<int> list = new List<int>();
			int position = 0;
			while (position < source.Length)
			{
				int index = source.IndexOf(rex, position);
				if (index > -1)
				{
					list.Add(source.IndexOf(rex, position));
					position = index + 1;
				}
				else
				{
					break;
				}
			}
			int[] ins = new int[list.Count];
			for (int i = 0; i < ins.Length; i++)
			{
				ins[i] = list[i];
			}
			return ins;
		}

		/// <summary>返回所有的rex在souce串中出现的位置</summary>
		/// <param name="source">源字符串</param>
		/// <param name="rex">表达式</param>
		/// <returns>所有的位置信息</returns>
		public static int[] GetAllIndex(string source, string[] rexs)
		{
			List<int> list = new List<int>();
			foreach (string s in rexs)
			{
				int[] indexs = GetAllIndex(source, s);
				foreach (int index in indexs)
				{
					list.Add(index);
				}
			}
			int[] ins = new int[list.Count];
			for (int i = 0; i < ins.Length; i++)
			{
				ins[i] = list[i];
			}
			return ins;
		}

		/// <summary>倒转字符串，输入abc，返回cba</summary>
		/// <param name="string">字符串</param>
		/// <returns>倒转后的值</returns>
		public static string ReverseString(string @string)
		{
			if (IsNullOrEmpty(@string))
			{
				return string.Empty;
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				for (int i = 1; i <= @string.Length; i++)
				{
					sb.Append(@string[@string.Length - i]);
				}
				return sb.ToString();
			}
		}

		public static string GetNotNullValue(string @string)
		{
			if (@string == null)
			{
				@string = string.Empty;
			}
			return @string;
		}

		/// <summary>全角转半角</summary>
		/// <param name="QJstr">全角字符</param>
		/// <returns/>
		public static string QBchange(string QJstr)
		{
			if (IsNullOrEmpty(QJstr))
			{
				return string.Empty;
			}
			char[] c = QJstr.ToCharArray();
			for (int i = 0; i < c.Length; i++)
			{
				if (c[i] == 12288)
				{
					c[i] = (char)32;
					continue;
				}
				if (c[i] > 65280 && c[i] < 65375)
				{
					c[i] = (char)(c[i] - 65248);
				}
			}
			return new string(c);
		}

		public static string GetTimeString(string time, int length)
		{
			if (time.Contains("."))
			{
				int dl = time.Length - time.IndexOf(".") - 1;
				if (dl > length)
				{
					time = Sharpen.Runtime.Substring(time, 0, time.Length - dl + length);
				}
			}
			return time;
		}

		public static bool ParseBoolean(string @bool)
		{
			try
			{
				return bool.Parse(@bool);
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static int GetChineseLength(string @string)
		{
			if (@string == null)
			{
				return 0;
			}
			else
			{
				int length = 0;
				foreach (char c in @string.ToCharArray())
				{
					if (IsChineseCharacter(c))
					{
						length++;
					}
				}
				return length;
			}
		}

		public static int GetEnglishLength(string @string)
		{
			if (@string == null)
			{
				return 0;
			}
			else
			{
				int length = 0;
				foreach (char c in @string.ToCharArray())
				{
					if (IsEnglishCharacter(c))
					{
						length++;
					}
				}
				return length;
			}
		}

		public static string GetBlankString(int len)
		{
			string s = string.Empty;
			for (int i = 0; i < len; i++)
			{
				s = s + " ";
			}
			return s;
		}

		public static bool SameTypeCharacter(string a, string b)
		{
			if (a == null || b == null)
			{
				return false;
			}
			else
			{
				if (ContainsChinese(a))
				{
					if (ContainsChinese(b))
					{
						return true;
					}
				}
				if (ContainsEnglish(a))
				{
					if (ContainsEnglish(b))
					{
						return true;
					}
				}
				if (ContainsNumber(a))
				{
					if (ContainsNumber(b))
					{
						return true;
					}
				}
				return false;
			}
		}

		public static bool StringEquals(string a, string b)
		{
			if (a == null && b == null)
			{
				return true;
			}
			else
			{
				if (a != null && b != null)
				{
					return a.Equals(b);
				}
				else
				{
					return false;
				}
			}
		}

		public static string ReplaceFirst(string @string, string source, string target)
		{
			int[] ids = GetAllIndex(@string, source);
			if (ids.Length > 0)
			{
				return Sharpen.Runtime.Substring(@string, 0, ids[0]) + target + Sharpen.Runtime.Substring(@string, ids[0] + source.Length);
			}
			return @string;
		}

		public static string ReplaceOnce(string source, string rex)
		{
			if (IsNullOrEmpty(rex))
			{
				return source;
			}
			if (!GetStandardString(source).Contains(GetStandardString(rex)))
			{
				return source;
			}
			bool replaced = false;
			bool loop = true;
			// 替代独立的
			while (loop)
			{
				loop = false;
				int[] ids = GetAllIndex(source, rex);
				foreach (int i in ids)
				{
					if (IsOneSideAbsoluteWord(source, rex[0], i - 1) && IsOneSideAbsoluteWord(source, rex[rex.Length - 1], i + rex.Length))
					{
						source = Sharpen.Runtime.Substring(source, 0, i) + " " + Sharpen.Runtime.Substring(source, i + rex.Length, source.Length);
						replaced = true;
						loop = true;
						break;
					}
				}
			}
			// 如果没有独立的，替代第一个不独立的
			foreach (int i_1 in GetAllIndex(source, rex))
			{
				if (IsOneSideAbsoluteWord(source, rex[0], i_1 - 1) || IsOneSideAbsoluteWord(source, rex[rex.Length - 1], i_1 + rex.Length))
				{
					if (!replaced)
					{
						return Sharpen.Runtime.Substring(source, 0, i_1) + " " + Sharpen.Runtime.Substring(source, i_1 + rex.Length, source.Length);
					}
				}
			}
			return source;
		}

		private static bool IsOneSideAbsoluteWord(string source, char rex, int i)
		{
			if (i >= 0 && i < source.Length)
			{
				if (IsEnglishCharacter(rex) || IsNumberCharacter(rex))
				{
					if (IsEnglishCharacter(source[i]) || IsNumberCharacter(source[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// 约定table field和object field之间的关系
		/// ccDdEfg = &gt; cc_dd_efg
		/// </summary>
		/// <param name="tableField"/>
		/// <returns>Object field name</returns>
		public static string GetTableField(string objectField)
		{
			if (StringUtil.IsNullOrEmpty(objectField))
			{
				throw new Exception("empty string is not allowed");
			}
			try
			{
				char[] chars = objectField.ToCharArray();
				IList<int> ids = new List<int>();
				for (int i = 0; i < chars.Length; i++)
				{
					if (chars[i] >= 'A' && chars[i] <= 'Z')
					{
						ids.Add(i);
					}
				}
				if (ids.Count == 0)
				{
					return objectField;
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					sb.Append(Sharpen.Runtime.Substring(objectField, 0, ids[0]));
					for (int i_1 = 0; i_1 < ids.Count - 1; i_1++)
					{
						sb.Append("_" + Sharpen.Runtime.Substring(objectField, ids[i_1], ids[i_1 + 1]).ToLower());
					}
					sb.Append("_" + Sharpen.Runtime.Substring(objectField, ids[ids.Count - 1], objectField.Length).ToLower());
					return sb.ToString();
				}
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		/// <summary>把字符串分割为字母，数字，汉字，符号，依次输出</summary>
		/// <param name="s"/>
		/// <param name="keepMark">是否保留符号</param>
		/// <returns/>
		public static string[] GetSeparatedString(string s, bool keepMark)
		{
			IList<string> list = new List<string>();
			char[] cs = s.ToCharArray();
			int lastType = -1;
			string currentString = string.Empty;
			for (int i = 0; i < cs.Length; i++)
			{
				if (i == 0)
				{
					currentString = string.Empty + cs[i];
					lastType = GetType(cs[i]);
				}
				else
				{
					int type = GetType(cs[i]);
					if (type == lastType)
					{
						currentString = currentString + cs[i];
					}
					else
					{
						if ((lastType > 0 || keepMark) && !StringUtil.IsNullOrEmpty(currentString))
						{
							list.Add(currentString);
						}
						currentString = string.Empty + cs[i];
						lastType = type;
					}
				}
				if (i == cs.Length - 1)
				{
					if ((lastType > 0 || keepMark) && !StringUtil.IsNullOrEmpty(currentString))
					{
						list.Add(currentString);
					}
				}
			}
			return Sharpen.Collections.ToArray(list, new string[] {  });
		}

		private static int GetType(char c)
		{
			if (StringUtil.IsEnglishCharacter(c))
			{
				return 1;
			}
			else
			{
				if (StringUtil.IsNumberCharacter(c) || c == '.')
				{
					return 2;
				}
				else
				{
					if (StringUtil.IsChineseCharacter(c))
					{
						return 3;
					}
				}
			}
			return -1;
		}

		public static int GetDependency(string source, string word, int start)
		{
			int dependency = 0;
			if (word.Length > 0)
			{
				if (start == 0)
				{
					dependency = dependency + 1;
				}
				else
				{
					string left = source[start - 1].ToString();
					string left2 = Sharpen.Runtime.Substring(source, (int)MyMath.Min(0, start - 2), start);
					if (!IsSameType(Sharpen.Runtime.Substring(word, 0, 1), left))
					{
						// aa-word
						if (StringUtil.ContainsEnglishOrNumber(word) && left.Equals("-") && StringUtil.ContainsEnglishOrNumber(left2))
						{
							dependency = dependency + 0;
						}
						else
						{
							dependency = dependency + 1;
						}
					}
				}
				if (start + word.Length == source.Length)
				{
					dependency = dependency + 1;
				}
				else
				{
					string right = Sharpen.Runtime.Substring(source, start + word.Length, start + word.Length + 1);
					string right2 = Sharpen.Runtime.Substring(source, start + word.Length, (int)MyMath.Min(source.Length, start + word.Length + 2));
					if (!IsSameType(Sharpen.Runtime.Substring(word, word.Length - 1), right))
					{
						// word-aa
						if (StringUtil.ContainsEnglishOrNumber(word) && right.Equals("-") && StringUtil.ContainsEnglishOrNumber(right2))
						{
							dependency = dependency + 0;
						}
						else
						{
							dependency = dependency + 1;
						}
					}
				}
				return dependency;
			}
			return -1;
		}

		private static bool IsSameType(string s1, string s2)
		{
			return (StringUtil.IsCharOrNumberString(s1) && StringUtil.IsCharOrNumberString(s2)) || (StringUtil.IsAllChineseCharacter(s1) && StringUtil.IsAllChineseCharacter(s2));
		}

		public static string GetMeanfullString(string s)
		{
			if (StringUtil.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			IList<string> list = new List<string>();
			foreach (char c in s.ToCharArray())
			{
				if (IsChineseCharacter(c) || IsEnglishOrNumberCharacter(c))
				{
					list.Add(c.ToString());
				}
			}
			return StringUtil.GetStringFromStrings(list, string.Empty);
		}

		public static bool IsMeanfullString(string s)
		{
			foreach (char c in s.ToCharArray())
			{
				if (IsChineseCharacter(c) || IsEnglishOrNumberCharacter(c))
				{
					return true;
				}
			}
			return false;
		}

		public static string Replace(string temple, Dictionary<string, string> map)
		{
			Dictionary<int, string> indexMap = new MyDictionary<int, string>();
			HashSet<int> set = new HashSet<int>();
			foreach (string key in map.Keys)
			{
				int index = temple.IndexOf(key);
				if (index >= 0)
				{
					indexMap[index] = key;
					for (int i = index; i < index + key.Length; i++)
					{
						set.Add(i);
					}
				}
			}
			string s = string.Empty;
			for (int i_1 = 0; i_1 < temple.Length; i_1++)
			{
				if (!set.Contains(i_1))
				{
					s = s + temple[i_1];
				}
				else
				{
					if (indexMap.ContainsKey(i_1))
					{
						s = s + map[indexMap[i_1]];
					}
				}
			}
			return s;
		}

		public static string ReplaceAll(string temple, Dictionary<string, string> map)
		{
			string t = temple;
			foreach (string key in map.Keys)
			{
				t = t.Replace(key, map[key]);
			}
			return t;
		}

		public static bool Contains(string big, string small, string spliter)
		{
			string[] ss = big.Split(spliter);
			foreach (string s in ss)
			{
				if (s.Trim().ToLower().Equals(small.Trim().ToLower()))
				{
					return true;
				}
			}
			return false;
		}

		public static string[] Split(string source, string front, string back)
		{
			int[] fs = StringUtil.GetAllIndex(source, front);
			int[] bs = StringUtil.GetAllIndex(source, back);
			IList<string> list = new List<string>();
			for (int i = 0; i < fs.Length; i++)
			{
				int f1 = fs[i];
				for (int j = i + 1; j <= fs.Length; j++)
				{
					int end = TryNext(source, f1, i, fs, bs, list, j);
					if (end >= 0)
					{
						if (i == 0)
						{
							list.Add(0, Sharpen.Runtime.Substring(source, 0, fs[i]));
						}
						else
						{
							if (end == bs.Length - 1)
							{
								list.Add(Sharpen.Runtime.Substring(source, bs[end] + 1, source.Length));
							}
							else
							{
								if (end == bs.Length)
								{
									list.Add(Sharpen.Runtime.Substring(source, bs[end - 1] + 1, source.Length));
								}
							}
						}
						i = j - 1;
						break;
					}
				}
			}
			return Sharpen.Collections.ToArray(list, new string[] {  });
		}

		private static int TryNext(string source, int f1, int i, int[] fs, int[] bs, IList<string> list, int end)
		{
			int f2 = source.Length;
			if (end < fs.Length)
			{
				f2 = fs[end];
			}
			for (int j = bs.Length - 1; j >= 0; j--)
			{
				int b = bs[j];
				if (f1 < b && f2 > b)
				{
					list.Add(Sharpen.Runtime.Substring(source, f1 + 1, b));
					return j;
				}
			}
			return -1;
		}

		public static string[] SplitStrict(string source, string front, string back)
		{
			int[] fs = StringUtil.GetAllIndex(source, front);
			int[] bs = StringUtil.GetAllIndex(source, back);
			if (fs.Length == bs.Length)
			{
				for (int i = 0; i < bs.Length; i++)
				{
					if (fs[i] > bs[i])
					{
						return new string[] {  };
					}
				}
				IList<string> list = new List<string>();
				for (int i_1 = 0; i_1 < fs.Length; i_1++)
				{
					string head = string.Empty;
					if (i_1 == 0)
					{
						head = Sharpen.Runtime.Substring(source, 0, fs[i_1]);
					}
					else
					{
						head = Sharpen.Runtime.Substring(source, bs[i_1 - 1] + 1, fs[i_1]);
					}
					if (StringUtil.IsMeanfullString(head))
					{
						list.Add(head);
					}
					list.Add(Sharpen.Runtime.Substring(source, fs[i_1] + 1, bs[i_1]));
				}
				if (bs[bs.Length - 1] < source.Length)
				{
					list.Add(Sharpen.Runtime.Substring(source, bs[bs.Length - 1] + 1));
				}
				return Sharpen.Collections.ToArray(list, new string[] {  });
			}
			return new string[] {  };
		}
	}
}
