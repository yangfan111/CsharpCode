using System;
using System.Collections.Generic;
using System.Reflection;
using YF.FileUtil;
using YF.Utils;

namespace YF
{


    public static class IdGenerateUtil 
    {
        
        public static int GenerateByTwoSimpleAssy(int enumOne, int enumTwo) 
        {
            return (enumOne << 16) + enumTwo;
        }
    }
    public   class IDGenerateUtil:IVoyagerLib
    {
        #region //reflection
        public static string[] CollectEnumNames<T>(T etype) where T : Enum
        {
            Type attrType = etype.GetType();
            return Enum.GetNames(attrType);

        }

        string[] IVoyagerLib.CollectEnumNames<T>(T etype)
        {
            return CollectEnumNames(etype);
        }

        public  void HandleInstanceFieldsByEnum<T1,T2>(T1 etype,T2 instance,Action<FieldCollectionStruct> fieldAction ) where T1 : Enum
        {
            var instanceType = instance.GetType();
            
            var enumNames = CollectEnumNames(etype);
            foreach (var enumName in enumNames)
            {
                FieldInfo field =  instanceType.GetField(enumName);
                if (field == null) continue;
                FieldCollectionStruct fieldParam = new FieldCollectionStruct(field,instance);
                fieldAction(fieldParam);
            }

        }
        ///反射获取全局类型索引值 [类名/类名截取->短id计算] ---> 类名
        public  Dictionary<uint, string> GetAllDerivedTypes(System.Type T)
        {
            var derivedTypes = new Dictionary<uint, string>();

            var baseType = T; //typeof(AudioTriggerBase);
         
#if UNITY_WSA && !UNITY_EDITOR
		   var baseTypeInfo = System.Reflection.IntrospectionExtensions.GetTypeInfo(baseType);
            var typeInfos    = baseTypeInfo.Assembly.DefinedTypes;

            foreach (var typeInfo in typeInfos)
            {
                if (typeInfo.IsClass && (typeInfo.IsSubclassOf(baseType) || baseTypeInfo.IsAssignableFrom(typeInfo) && baseType != typeInfo.AsType()))
                {
                    var typeName = typeInfo.Name;
                    derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute(typeName), typeName);
                }
            }
#else
            var types = baseType.Assembly.GetTypes();

            for (var i = 0; i < types.Length; i++)
            {
                if (types[i].IsClass &&
                    (types[i].IsSubclassOf(baseType) || baseType.IsAssignableFrom(types[i]) && baseType != types[i]))
                {
                    var typeName   = types[i].Name;
                    var typeSplits = typeName.Split('_');
                    derivedTypes.Add(GenShortUintIdByName(typeName), typeName);
                }
            }
#endif

            //Add the Awake, Start and Destroy triggers and build the displayed list.
            //derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Awake"), "Awake");
            //derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Start"), "Start");
            //derivedTypes.Add(AkUtilities.ShortIDGenerator.Compute("Destroy"), "Destroy");

            return derivedTypes;
        }

        #endregion
        
         
        
        
        
        
        
        
        
        ///按名字获取短位ID方法
        public  uint GenShortUintIdByName(string inName)
        {
            return CommonUtil.ShortIDGenerator.Compute(inName);
        }
        
       
        ///反射获取全局类型索引值 [类名/类名截取->短id计算] ---> 类名
        /// baseType.Assembly.GetTypes();
        /// types[i].IsSubclassOf(baseType)
        /// 非Eidotr环境下
        /// var baseTypeInfo = System.Reflection.IntrospectionExtensions.GetTypeInfo(baseType);
		/// var typeInfos = baseTypeInfo.Assembly.DefinedTypes;
 
        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CombineAppPath(string path)
        {
            path = PS.NormalizePath(path);
            return System.IO.Path.Combine(UnityEngine.Application.dataPath
                , path);
        }
    
    }
}

