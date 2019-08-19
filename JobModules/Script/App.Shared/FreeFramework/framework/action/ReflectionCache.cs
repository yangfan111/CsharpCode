using System;
using System.Collections.Generic;
using System.Reflection;
using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.exception;
using com.wd.free.para;
using com.cpkf.yyjd.tools.util.math;
using Core.Free;

namespace com.wd.free.action
{
    public class ReflectionCache
    {
        /*private static ICollection<string> types;*/
        private static MyDictionary<string, FieldInfo>[] cache;
        private static MyDictionary<string, FieldInfo>[] simpleFields;

        //private static MyDictionary<string, MyDictionary<string, FieldInfo>> cache;
        //private static MyDictionary<string, MyDictionary<string, FieldInfo>> simpleFields;

        static ReflectionCache()
        {
            cache = new MyDictionary<string, FieldInfo>[(int)ERuleIds.ZLength];
            simpleFields = new MyDictionary<string, FieldInfo>[(int)ERuleIds.ZLength];
            //cache = new MyDictionary<string, MyDictionary<string, FieldInfo>>();
            //simpleFields = new MyDictionary<string, MyDictionary<string, FieldInfo>>();
            /*types = new HashSet<string>();*/
            /*Sharpen.Collections.AddAll(types, Arrays.AsList(new string[] { "double", "single", "int64", "int32", "string", "boolean" }));*/
        }

        public static bool ContainType(string type)
        {
            return (ParseUtility.IsBoolean(type) || ParseUtility.IsSingle(type) || ParseUtility.IsDouble(type) || ParseUtility.IsInt64(type) || ParseUtility.IsInt32(type) || ParseUtility.IsString(type));
        }

        public static bool HasField(object obj, string field)
        {
            IRule ruleObj = obj as IRule;
            Initial(ruleObj);
            if (obj == null)
            {
                return false;
            }
            /*string key = obj.GetType().FullName;*/
            if (ruleObj == null)
            {
                throw new GameConfigExpception(obj.GetType().ToString() + " is null or not defined.");
            } 
            int id = ruleObj.GetRuleID();
            FieldInfo v = null;
            return cache[id].TryGetValue(field, out v);
        }

        public static string[] GetSimpleFieldNames(object obj)
        {
            IRule ruleObj = obj as IRule;
            Initial(ruleObj);
            if (obj == null)
            {
                return new string[0];
            }
            /*string key = obj.GetType().FullName;*/
            int id = ruleObj.GetRuleID();
            return Sharpen.Collections.ToArray(simpleFields[id].Keys, new string[] { });
        }

        public static bool IsSimpleField(string type)
        {
            return ContainType(type);
        }

        public static string[] GetFieldNames(object obj)
        {
            IRule ruleObj = obj as IRule;
            Initial(ruleObj);
            if (obj == null)
            {
                return new string[0];
            }
            /*string key = obj.GetType().FullName;*/
            int id = ruleObj.GetRuleID();
            return Sharpen.Collections.ToArray(cache[id].Keys, new string[] { });
        }

        private static void Initial(/*object*/IRule obj)
        {
            if (obj != null)
            {
                /*string key = obj.GetType().FullName;*/
                int id = obj.GetRuleID();
                if (null == cache[id])
                {
                    Type cl = obj.GetType();
                    MyDictionary<string, FieldInfo> c = new MyDictionary<string, FieldInfo>();
                    cache[id] = c;
                    MyDictionary<string, FieldInfo> s = new MyDictionary<string, FieldInfo>();
                    simpleFields[id] = s;
                    while (cl != null && !cl.FullName.Equals("java.lang.Object"))
                    {
                        FieldInfo[] fields = cl.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        for (int i = 0, maxi = (fields == null ? 0 : fields.Length); i < maxi; i++)
                        {
                            FieldInfo f = fields[i];
                            string type = f.FieldType.Name;

                            string name = f.Name;

                            int index = name.IndexOf('<');
                            int index2 = name.IndexOf('>');

                            if (index > -1 && index2 > -1)
                            {
                                name = name.Substring(1, index2 - 1);
                            }

                            FieldInfo v = null;
                            if (!c.TryGetValue(name, out v))
                            {
                                c[name] = f;
                            }

                            if (ContainType(type))
                            {
                                if (!s.TryGetValue(name, out v))
                                {
                                    s[name] = f;
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
                string type = f.FieldType.Name;

                if (ParseUtility.IsInt64(type))
                {
                    para = new LongPara(field);
                }
                if (ParseUtility.IsInt32(type))
                {
                    para = new IntPara(field);
                }
                if (ParseUtility.IsSingle(type))
                {
                    para = new FloatPara(field);
                }
                if (ParseUtility.IsDouble(type))
                {
                    para = new DoublePara(field);
                }
                if (ParseUtility.IsString(type))
                {
                    para = new StringPara(field);
                }
                if (ParseUtility.IsBoolean(type))
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

        public static bool ContainsField(/*object*/IRule obj, string field)
        {
            Initial(obj);
            /*string key = obj.GetType().FullName;*/
            int id = obj.GetRuleID();

            return cache[id].ContainsKey(field);
        }

        public static object GetValue(FieldInfo field, string stringValue)
        {
            string type = field.FieldType.Name;
            if (ParseUtility.IsInt64(type))
            {
                return long.Parse(stringValue);
            }
            if (ParseUtility.IsInt32(type))
            {
                return int.Parse(stringValue);
            }
            if (ParseUtility.IsSingle(type))
            {
                return float.Parse(stringValue);
            }
            if (ParseUtility.IsDouble(type))
            {
                return double.Parse(stringValue);
            }
            if (ParseUtility.IsBoolean(type))
            {
                return bool.Parse(stringValue);
            }
            return stringValue;
        }

        public static FieldInfo GetField(object obj, string field)
        {
            IRule ruleObj = obj as IRule;
            Initial(ruleObj);
            /*string key = obj.GetType().FullName;*/
            int id = ruleObj.GetRuleID();
            if (cache[id].ContainsKey(field))
            {
                return cache[id][field];
            }
            else
            {
                throw new GameConfigExpception(field + " is not a valid field at " + id + " and type is " + ruleObj.GetType());
            }
        }
    }
}
