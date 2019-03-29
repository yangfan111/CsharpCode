using com.cpkf.yyjd.tools.util;
using com.wd.free.exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.UnitTest
{
    public class TestValue
    {
        public const string FieldValue = "value";
        public const string FieldColor = "color";

        private const int TypeInt = 1;
        private const int TypeDouble = 2;
        private const int TypeFloat = 3;
        private const int TypeLong = 4;
        private const int TypeBool = 5;
        private const int TypeString = 6;

        public string Name;
        private Dictionary<string, object> dic;

        public TestValue()
        {
            dic = new Dictionary<string, object>();
        }

        public static string ToRecords(TestValue[] tvs)
        {
            List<string> list = new List<string>();

            foreach (TestValue tv in tvs)
            {
                list.Add(tv.ToRecord());
            }

            return string.Join(StringUtil.SPLITER_RECORD, list.ToArray());
        }

        public string ToRecord()
        {
            List<string> list = new List<string>();
            foreach (string key in dic.Keys)
            {
                list.Add(key + ":" + GetType(dic[key]) + ":" + dic[key]);
            }

            return string.Join(StringUtil.SPLITER_FIELD, list.ToArray());
        }

        public static TestValue[] RecordsFromString(string v)
        {
            List<TestValue> list = new List<TestValue>();
            string[] tvs = StringUtil.Split(v, StringUtil.SPLITER_RECORD);

            foreach (string t in tvs)
            {
                string[] rs = StringUtil.Split(t, StringUtil.SPLITER_FIELD);
                TestValue tv = new TestValue();
                foreach (string r in rs)
                {
                    string[] fs = r.Split(':');
                    if (fs.Length == 3)
                    {
                        tv.AddField(fs[0], tv.GetValue(int.Parse(fs[1]), fs[2]));
                    }
                }

                list.Add(tv);
            }

            return list.ToArray();
        }

        public static TestValue RecordFromString(string v)
        {
            TestValue tv = new TestValue();
            string[] rs = StringUtil.Split(v, StringUtil.SPLITER_FIELD);

            foreach (string r in rs)
            {
                string[] fs = r.Split(':');
                if (fs.Length == 3)
                {
                    tv.AddField(fs[0], tv.GetValue(int.Parse(fs[1]), fs[2]));
                }
            }

            return tv;
        }

        public void AddValue(object value)
        {
            AddField(FieldValue, value);
        }

        public void AddField(string field, object value)
        {
            if (dic.ContainsKey(field))
            {
                dic.Remove(field);
            }
            dic.Add(field, value);
        }

        public bool IsSimilar(TestValue other, int diffPercent)
        {
            if (dic.Count != other.dic.Count)
            {
                return false;
            }

            foreach (string key in dic.Keys)
            {
                if (!other.dic.ContainsKey(key))
                {
                    return false;
                }

                if (dic[key] != other.dic[key])
                {
                    try
                    {
                        double v = GetDouble(dic[key]);
                        double v1 = GetDouble(other.dic[key]);

                        if (Math.Abs(v1 - v) / Math.Abs(v) > diffPercent / 100)
                        {
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private object GetValue(int type, string v)
        {
            switch (type)
            {
                case TypeDouble:
                    return double.Parse(v);
                case TypeFloat:
                    return float.Parse(v);
                case TypeInt:
                    return int.Parse(v);
                case TypeLong:
                    return long.Parse(v);
                case TypeBool:
                    return bool.Parse(v);
                case TypeString:
                    return v;
                default:
                    return v;
            }
        }

        private int GetType(object v)
        {
            if (v is double)
            {
                return TypeDouble;
            }
            if (v is int)
            {
                return TypeInt;
            }
            if (v is float)
            {
                return TypeFloat;
            }
            if (v is long)
            {
                return TypeLong;
            }
            if (v is bool)
            {
                return TypeBool;
            }
            if (v is string)
            {
                return TypeString;
            }

            return 0;
        }

        private double GetDouble(object v)
        {
            if (v is double)
            {
                return (double)v;
            }
            else
            {
                if (v is float)
                {
                    return (float)v;
                }
                else
                {
                    if (v is int)
                    {
                        return (int)v;
                    }
                    else
                    {
                        if (v is long)
                        {
                            return (long)v;
                        }
                    }
                }
            }
            throw new GameConfigExpception(v + " is not a valid double value");
        }

        public bool IsSame(TestValue other)
        {
            if (dic.Count != other.dic.Count)
            {
                return false;
            }

            foreach (string key in dic.Keys)
            {
                if (!other.dic.ContainsKey(key))
                {
                    return false;
                }

                if (dic[key].ToString() != other.dic[key].ToString())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
