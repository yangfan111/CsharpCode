using System.Collections;
using System.Collections.Generic;

using ConsoleApp;
using YF.FileUtil;
namespace YF
{
    public partial class RunLib
    {
        ///按名字获取短位ID方法
        public static uint GenShortUintIdByName(string inName)
        {
            return ShortIDGenerator.Compute(inName);
        }
        ///反射获取全局类型索引值 [类名/类名截取->短id计算] ---> 类名
        /// baseType.Assembly.GetTypes();
        /// types[i].IsSubclassOf(baseType)
        /// 非Eidotr环境下
        /// var baseTypeInfo = System.Reflection.IntrospectionExtensions.GetTypeInfo(baseType);
		/// var typeInfos = baseTypeInfo.Assembly.DefinedTypes;
        public static Dictionary<uint, string> GetAllDerivedTypes(System.Type T)
        {
            var derivedTypes = new Dictionary<uint, string>();

            var baseType = T;//typeof(AudioTriggerBase);

#if UNITY_WSA && !UNITY_EDITOR
		var baseTypeInfo = System.Reflection.IntrospectionExtensions.GetTypeInfo(baseType);
		var typeInfos = baseTypeInfo.Assembly.DefinedTypes;

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
                    var typeName = types[i].Name;
                    var typeSplits = typeName.Split('_');
                    string computedSplit = typeSplits.Length == 2 ? typeSplits[1] : typeSplits[0];
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
        public static Dictionary<EnumType, string> GetEnumType_StringDict()
        {

            var triggerTypeBehaviors = new Dictionary<EnumType, string>();
            ///反射获取类型，Type et
            ///反射获取类型元素列表， Enum.GetValues(et)
            ///System.Enum.GetName(Type,TypeValue)
            System.Type et = typeof(EnumType);
            System.Array enumArr = System.Enum.GetValues(et);
            foreach (EnumType etype in enumArr)
            {
                string strName = System.Enum.GetName(et, etype);
                triggerTypeBehaviors.Add(etype, "On" + strName);
            }
            return triggerTypeBehaviors;
        }

        /// <summary>
        /// 自定义monobehavior的sendMessage回调结构
        /// </summary>TODO:
        /// <param name="callbackData"></param>
        /// <param name="in_cookie"></param>
        /// <param name="in_type"></param>
        /// <param name="in_info"></param>
        /// <param name="target"></param>
        //public static void ExcuteMonoCallback(EventCallbackData callbackData, object in_cookie, CustomCallbackType in_type, CallbackInfo in_info,UnityEngine.GameObject target)
        //{
        //    for (var i = 0; i < callbackData.callbackFunc.Count; i++)
        //    {
        //        if (((int)in_type & callbackData.callbackFlags[i]) != 0 && callbackData.callbackGameObj[i] != null)
        //        {
        //            var callbackInfo = new EventCallbackPackagage();
        //            callbackInfo.type = in_type;
        //            callbackInfo.sender = target;
        //            callbackInfo.info = in_info;

        //            callbackData.callbackGameObj[i].SendMessage(callbackData.callbackFunc[i], callbackInfo);
        //        }
        //    }
        //}
        //pause
        private void OnApplicationPause(bool pauseStatus)
        {

        }
        //focus
        private void OnApplicationFocus(bool focus)
        {

        }
        //quit
        private void OnApplicationQuit()
        {

        }

        //TODO:
        public static void SoundStop()
        {
            // Stop everything, and make sure the callback buffer is empty. We try emptying as much as possible, and wait 10 ms before retrying.
            //// Callbacks can take a long time to be posted after the call to RenderAudio().
            //AkSoundEngine.StopAll();
            //AkSoundEngine.ClearBanks();
            //AkSoundEngine.RenderAudio();
            //var retry = 5;
            //do
            //{
            //    var numCB = 0;
            //    do
            //    {
            //        numCB = AkCallbackManager.PostCallbacks();

            //        // This is a WSA-friendly sleep
            //        using (System.Threading.EventWaitHandle tmpEvent = new System.Threading.ManualResetEvent(false))
            //        {
            //            tmpEvent.WaitOne(System.TimeSpan.FromMilliseconds(1));
            //        }
            //    }
            //    while (numCB > 0);

            //    // This is a WSA-friendly sleep
            //    using (System.Threading.EventWaitHandle tmpEvent = new System.Threading.ManualResetEvent(false))
            //    {
            //        tmpEvent.WaitOne(System.TimeSpan.FromMilliseconds(10));
            //    }

            //    retry--;
            //}
            //while (retry > 0);

            //AkSoundEngine.Term();

            //// Make sure we have no callbacks left after Term. Some might be posted during termination.
            //AkCallbackManager.PostCallbacks();

            //AkCallbackManager.Term();
            //AkBankManager.Reset();
        }

    }
}

