using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

    public static class CommonUtil
    {
        public static void InactiveSelf(this Transform transform,bool recursive)
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var trans = transform.GetChild(i);
                if(recursive)
                    InactiveSelf(trans,recursive);
                else
                    trans.gameObject.SetActive(false);            
            }
            transform.gameObject.SetActive(false);            
        }
        public static void SetParentWithInitialize(this Transform transform,Transform parent)
        {
            if (transform.parent && transform.parent == parent)
                return;
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }
        public static void ProcessEnumElements<T>(Action<T> process)
        {
            var typeT = typeof(T);
            var enumValues = Enum.GetValues(typeT);
            foreach (T enumValue in enumValues)
            {
                process(enumValue);
            }
        
        }
        public static T GetAttribute<T>(MemberInfo t) where T : Attribute
        {
            return Attribute.GetCustomAttribute(t, typeof(T)) as T;
        }
        public static void WeakAssert(bool condition)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(condition);
#endif
        }
        public static void WeakAssert(bool condition, string message, params object[] args)
        {

#if UNITY_EDITOR
            var text = string.Format(message, args);
            UnityEngine.Debug.Assert(condition, text);
#endif
        }

        public static void ProcessDerivedTypes(Type baseType, bool includeSelf, Action<Type> process)
        {
            var types = baseType.Assembly.GetTypes();
            for (var i = 0; i < types.Length; i++)
            { 
                if (types[i].IsClass &&
                    (types[i].IsSubclassOf(baseType) || baseType.IsAssignableFrom(types[i])))
                {
                    if (!includeSelf && baseType == types[i]) continue;
                    //   instance = Activator.CreateInstance(types[i]);
                    process(types[i]);
                }
            }
        }
    }