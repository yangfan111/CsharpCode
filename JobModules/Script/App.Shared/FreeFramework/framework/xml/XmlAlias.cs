using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using Sharpen;
using UnityEngine;
using Core.Utils;

namespace com.wd.free.xml
{
    public class XmlAlias
    {
        private static LoggerAdapter Logger = new LoggerAdapter(typeof(XmlAlias));


        private static MyDictionary<string, string[]> extendsCache = new MyDictionary<string, string[]>();

        private MyDictionary<string, object> objDic;
        private MyDictionary<string, MyDictionary<string, string>> attrDic;
        private MyDictionary<string, MyDictionary<string, string>> fieldDic;
        private MyDictionary<string, string> classNameDic;

        public XmlAlias()
        {
            objDic = new MyDictionary<string, object>();
            attrDic = new MyDictionary<string, MyDictionary<string, string>>();
            fieldDic = new MyDictionary<string, MyDictionary<string, string>>();
            classNameDic = new MyDictionary<string, string>();
        }

        public void AddClass(string name, object obj)
        {
            this.objDic[name] = obj;
            this.classNameDic[obj.GetType().Name] = name;
        }

        public void AddField(string name, string field, string alias)
        {
            if (!fieldDic.ContainsKey(name))
            {
                fieldDic[name] = new MyDictionary<string, string>();
            }

            fieldDic[name][alias] = field;
        }

        public void AddAttribue(string name, string field, string alias)
        {
            if (!attrDic.ContainsKey(name))
            {
                attrDic[name] = new MyDictionary<string, string>();
            }

            attrDic[name][alias] = field;
        }

        public void SetField(string parentName, object parent, object child, string field)
        {
            string realField = GetFieldRealName(parentName, parent, field);

            try
            {
                FieldInfo fi = ReflectionCache.GetField(parent, realField);
                if (fi.FieldType.Name != "IList")
                {
                    fi.SetValue(parent, child);
                }
                else
                {
                    throw new RuntimeException();
                }
            }
            catch (Exception)
            {
                foreach (string f in ReflectionCache.GetFieldNames(parent))
                {
                    FieldInfo fi = ReflectionCache.GetField(parent, f);
                    if (fi.FieldType.Name == "IList`1" || fi.FieldType.Name == "List`1")
                    {
                        Type t = fi.FieldType.GetGenericArguments()[0];
                        if (t.IsAssignableFrom(child.GetType()))
                        {
                            if (fi.GetValue(parent) == null)
                            {
                                fi.SetValue(parent, MakeList(t, child));
                            }
                            else
                            {
                                System.Collections.IList ilist = fi.GetValue(parent) as System.Collections.IList;
                                ilist.Add(child);
                            }

                            break;
                        }
                    }
                }

            }
        }

        private string GetFieldRealName(string name, object obj, string field)
        {
            if (attrDic.ContainsKey(name) && attrDic[name].ContainsKey(field))
            {
                return attrDic[name][field];
            }

            string[] parents = GetExtends(obj);
            foreach (string parent in parents)
            {
                if (attrDic.ContainsKey(parent) && attrDic[parent].ContainsKey(field))
                {
                    return attrDic[parent][field];
                }

                string temp = classNameDic[parent];
                if (temp != null)
                {
                    if (attrDic.ContainsKey(temp) && attrDic[temp].ContainsKey(field))
                    {
                        return attrDic[temp][field];
                    }
                }
            }

            if (fieldDic.ContainsKey(name) && fieldDic[name].ContainsKey(field))
            {
                return fieldDic[name][field];
            }

            foreach (string parent in parents)
            {
                if (fieldDic.ContainsKey(parent) && fieldDic[parent].ContainsKey(field))
                {
                    return fieldDic[parent][field];
                }

                string temp = classNameDic[parent];
                if (temp != null)
                {
                    if (fieldDic.ContainsKey(temp) && fieldDic[temp].ContainsKey(field))
                    {
                        return fieldDic[temp][field];
                    }
                }
            }

            return field;
        }

        private static string[] GetExtends(object obj)
        {
            if (!extendsCache.ContainsKey(obj.GetType().Name))
            {
                List<string> list = new List<string>();
                Type t = obj.GetType().BaseType;
                while (t != null)
                {
                    list.Add(t.Name);
                    t = t.BaseType;
                }

                extendsCache[obj.GetType().Name] = list.ToArray();
            }

            return extendsCache[obj.GetType().Name];
        }



        static object MakeList(Type t, params object[] items)
        {
            Type type = typeof(List<>).MakeGenericType(t);

            object list = Activator.CreateInstance(type);
            System.Collections.IList ilist = list as System.Collections.IList;
            foreach (object o in items)
            {
                ilist.Add(o);
            }

            return list;
        }

        public void SetFieldValue(string name, object obj, string field, string value)
        {
            string realField = GetFieldRealName(name, obj, field);
            FieldInfo fi = ReflectionCache.GetField(obj, realField);
            try
            {
                fi.SetValue(obj, ReflectionCache.GetValue(fi, value));
            }
            catch (Exception e)
            {
                Debug.LogError(obj.GetType().Name + "'s field '" + realField + " ' can not be set value '"+value+"'");
            }
        }

        public object GetObject(string name)
        {
            if (objDic.ContainsKey(name))
            {
                return SerializeUtil.Clone(objDic[name]);
            }

            Logger.ErrorFormat("{0} is not defined as object.", name);
            Debug.LogErrorFormat("{0} is not defined as object.", name);

            //throw new RuntimeException(name + " is not defined as object.");

            return null;
        }
    }
}
