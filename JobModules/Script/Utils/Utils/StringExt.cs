using System;
using System.Collections.Generic;

namespace Core.Utils
{
    public static class StringExt
    {
        public static bool EqualsIgnoreCase(this string lhs, string rhs)
        {
            return lhs.ToLower() == rhs.ToLower();
        }

        public static List<T> ToList<T>(this string str, char split, Converter<string, T> convertHandler)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new List<T>();
            }
            else
            {
                string[] arr = str.Split(new char[] { split }, StringSplitOptions.RemoveEmptyEntries);
                T[] Tarr = Array.ConvertAll(arr, convertHandler);
                return new List<T>(Tarr);
            }
        }

        public static HashSet<T> ToHashSet<T>(this string str, char split, Converter<string, T> convertHandler)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new HashSet<T>();
            }
            else
            {
                string[] arr = str.Split(new char[] { split }, StringSplitOptions.RemoveEmptyEntries);
                T[] Tarr = Array.ConvertAll(arr, convertHandler);
                return new HashSet<T>(Tarr);
            }
        }

        public static Dictionary<int, int> ToDict(this string str, char splitChar0 = '|', char splitChar1 = '&')
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            if (!string.IsNullOrEmpty(str))
            {
                string[] infos = str.Split(splitChar0);
                for (int i = 0; i < infos.Length; i++)
                {
                    if (string.IsNullOrEmpty(infos[i]))
                        continue;

                    string[] tmps = infos[i].Split(splitChar1);
                    if (tmps.Length == 2)
                    {
                        int key = 0, value = 0;
                        int.TryParse(tmps[0], out key);
                        int.TryParse(tmps[1], out value);
                        if (dict.ContainsKey(key))
                        {
                            dict[key] = value;
                        }
                        else
                        {
                            dict.Add(key, value);
                        }
                    }
                }

            }
            return dict;
        }

    }

}