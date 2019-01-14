
using System;
using System.Reflection;
using Core.Bag;
namespace App.Shared.Util
{
  
    public enum Err_WeaponLogicErrCode
    {
        Sucess = 0,
        Err_Default =1,
        Err_IdNotExisted =2,
        Err_PlayerDontHaveThrowingComponent=3,
        Err_PlayerGrenadePullVertifyFailed=4,
        Err_GrenadeListShowCountZero=5,
        Err_GrenadeNotFindNextUsableId =6,
        Err_SameWeaponPosition =7,
        Err_SlotNone =8,
        Err_NullComponent =9,
        Err_SlotInvailed =10,

    }
   
    public static class WeaponLogicUtil
    {
        /// <summary>
        /// 轮询下一个手雷id，没有不可换
        /// </summary>
        /// <param name="usableIdList"></param>
        /// <param name="currId"></param>
        /// <returns></returns>
     
    }
    
  
    public static class CommonUtil
    {

        public static T GetAttribute<T>(MemberInfo t) where T : Attribute
        {
            return Attribute.GetCustomAttribute(t, typeof(T)) as T;
        }

        public static void ProcessDerivedTypeInstances(Type baseType,bool includeSelf,Action<Type,Object> process)
        {

#if UNITY_WSA && !UNITY_EDITOR
		var baseTypeInfo = System.Reflection.IntrospectionExtensions.GetTypeInfo(baseType);
		var typeInfos = baseTypeInfo.Assembly.DefinedTypes;

		foreach (var typeInfo in typeInfos)
		{
			if (typeInfo.IsClass && (typeInfo.IsSubclassOf(baseType) || baseTypeInfo.IsAssignableFrom(typeInfo) && baseType != typeInfo.AsType()))
			{
               if (!includeSelf && baseType == typeInfo) continue;
                 instance = Activator.CreateInstance(typeInfo);
                 process(typeInfo, instance);
			}
		}
#else
            var types = baseType.Assembly.GetTypes();
            System.Object instance;
            for (var i = 0; i < types.Length; i++)
            {
                if (types[i].IsClass &&
                    (types[i].IsSubclassOf(baseType) || baseType.IsAssignableFrom(types[i]) ))
                {
                    if (!includeSelf && baseType == types[i]) continue;
                     instance = Activator.CreateInstance(types[i]);
                    process(types[i], instance);
                }
            }
#endif
          

        }
    }
   
}


