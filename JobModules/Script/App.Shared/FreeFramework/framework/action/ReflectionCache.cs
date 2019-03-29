using System;
using System.Collections.Generic;
using System.Reflection;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.exception;
using com.wd.free.para;

namespace com.wd.free.action
{
    public class ReflectionCache
    {
        private static ICollection<string> types;

        private static MyDictionary<string, MyDictionary<string, FieldInfo>> cache;
        private static MyDictionary<string, MyDictionary<string, FieldInfo>> simpleFields;

        static ReflectionCache()
        {
            cache = new MyDictionary<string, MyDictionary<string, FieldInfo>>();
            simpleFields = new MyDictionary<string, MyDictionary<string, FieldInfo>>();
            types = new HashSet<string>();
            Sharpen.Collections.AddAll(types, Arrays.AsList(new string[] { "double", "single", "int64", "int32", "string", "boolean" }));
        }

        public static bool HasField(object obj, string field)
        {
            Initial(obj);
            if (obj == null)
            {
                return false;
            }
            string key = obj.GetType().FullName;
            return cache[key].ContainsKey(field);
        }

        public static string[] GetSimpleFieldNames(object obj)
        {
            Initial(obj);
            if (obj == null)
            {
                return new string[0];
            }
            string key = obj.GetType().FullName;
            return Sharpen.Collections.ToArray(simpleFields[key].Keys, new string[] { });
        }

        public static bool IsSimpleField(string type)
        {
            return types.Contains(type);
        }

        public static string[] GetFieldNames(object obj)
        {
            Initial(obj);
            if (obj == null)
            {
                return new string[0];
            }
            string key = obj.GetType().FullName;
            return Sharpen.Collections.ToArray(cache[key].Keys, new string[] { });
        }

        private static void Initial(object obj)
        {
            if (obj != null)
            {
                string key = obj.GetType().FullName;
                if (!cache.ContainsKey(key))
                {
                    Type cl = obj.GetType();
                    cache[key] = new MyDictionary<string, FieldInfo>();
                    simpleFields[key] = new MyDictionary<string, FieldInfo>();
                    while (cl != null && !cl.FullName.Equals("java.lang.Object"))
                    {
                        foreach (FieldInfo f in cl.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                        {
                            string type = f.FieldType.Name.ToLower();

                            string name = f.Name;
                            if (name.Contains("<") && name.Contains(">"))
                            {
                                name = name.Substring(1, name.IndexOf(">") - 1);
                            }

                            if (!cache[key].ContainsKey(name))
                            {
                                cache[key][name] = f;
                            }

                            if (types.Contains(type))
                            {
                                if (!simpleFields[key].ContainsKey(name))
                                {
                                    simpleFields[key][name] = f;
                                }
                            }
                        }
                        cl = cl.BaseType;
                    }
                }
            }
        }

        public static IPara GetPara(object obj, string field)
        {
            if (obj == null)
            {
                return null;
            }
            FieldInfo f = GetField(obj, field);
            AbstractPara para = null;
            if (f != null)
            {
                string type = f.FieldType.Name.ToLower();
                if ("int64".Equals(type))
                {
                    para = new LongPara(field);
                }
                if ("int32".Equals(type))
                {
                    para = new IntPara(field);
                }
                if ("single".Equals(type))
                {
                    para = new FloatPara(field);
                }
                if ("double".Equals(type))
                {
                    para = new DoublePara(field);
                }
                if ("string".Equals(type))
                {
                    para = new StringPara(field);
                }
                if ("boolean".Equals(type))
                {
                    para = new BoolPara(field);
                }
                try
                {
                    if (para != null)
                    {
                        para.SetValue(f.GetValue(obj));
                    }
                }
                catch (Exception e)
                {
                    throw new GameConfigExpception(field + " is not a valid field.\n" + ExceptionUtil.GetExceptionContent(e));
                }
            }
            return para;
        }

        public static bool ContainsField(object obj, string field)
        {
            Initial(obj);
            string key = obj.GetType().FullName;

            return cache[key].ContainsKey(field);
        }

        public static object GetValue(FieldInfo field, string stringValue)
        {
            string type = field.FieldType.Name.ToLower();
            if ("int64".Equals(type))
            {
                return long.Parse(stringValue);
            }
            if ("int32".Equals(type))
            {
                return int.Parse(stringValue);
            }
            if ("single".Equals(type))
            {
                return float.Parse(stringValue);
            }
            if ("double".Equals(type))
            {
                return double.Parse(stringValue);
            }
            if ("boolean".Equals(type))
            {
                return bool.Parse(stringValue);
            }
            return stringValue;
        }

        public static FieldInfo GetField(object obj, string field)
        {
            Initial(obj);
            string key = obj.GetType().FullName;
            if (cache[key].ContainsKey(field))
            {
                return cache[key][field];
            }
            else
            {
                throw new GameConfigExpception(field + " is not a valid field at " + key);
            }
        }
    }
}
