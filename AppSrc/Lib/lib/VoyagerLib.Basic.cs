using System;
using System.Collections.Generic;
using System.Reflection;
using YF.FileUtil;
using YF.Utils;

namespace YF
{
    public interface IVoyagerLib
    {
        /// <summary>
        /// 获取Enum所有string[]
        /// </summary>
        /// <param name="etype"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        string[] CollectEnumNames<T>(T etype) where T : Enum;
        /// <summary>
        /// 根据Enum名称反射获取实例字段信息，封装成FieldCollectionStruct并执行
        /// 适用于配置属性的快速获取
        /// </summary>
        /// <param name="etype"></param>
        /// <param name="instance"></param>
        /// <param name="fieldAction"></param>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        void HandleInstanceFieldsByEnum<T1, T2>(T1 etype, T2 instance, Action<FieldCollectionStruct> fieldAction)
        where T1 : Enum;
        //获取该类型所有子类型短位id => type 字典
        Dictionary<uint, string> GetAllDerivedTypes(System.Type T);
    }

    public struct FieldCollectionStruct
    {
        public Type fieldType; //字段类型
        public object fieldValue;//字段值

        public FieldCollectionStruct(FieldInfo field,object instance)
        {
            fieldType = field.GetType();
            fieldValue = field.GetValue(instance);
        }
    }
    
        
        

}

