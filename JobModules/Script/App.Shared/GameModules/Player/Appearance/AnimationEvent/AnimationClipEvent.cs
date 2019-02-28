using App.Shared.GameModules.Player.Animation;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Core.CharacterState.Posture;
using Sharpen;
using UnityEngine;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class AnimationClipEvent : MonoBehaviour
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AnimationClipEvent));
        private static string NameSpace = "App.Shared.GameModules.Player.Appearance.AnimationEvent";
        private readonly AnimationClipNameMatcher _matcher = new AnimationClipNameMatcher();
        private static string _methodName = "AnimationEventCallback";
        private static string _cleanStr = "Clean";
        private static Dictionary<string, IAnimationEventCallback> _eventCallbacks = new Dictionary<string, IAnimationEventCallback>();
        private static Dictionary<short, string> _eventIdToClassName = new Dictionary<short, string>();
        private static Dictionary<string,short> _classNameToEventId = new Dictionary<string,short>();
        
        public List<KeyValuePair<short,AnimationEventParam>> EventParams = new List<KeyValuePair<short, AnimationEventParam>>();

        #region MyRegion

        

      
        static AnimationClipEvent()
        {
            _logger = new LoggerAdapter(typeof(AnimationClipEvent));
            var types = FindAllAnimationEventCallbacktType();
            foreach (var type in types)
            {

                try
                {
                    IAnimationEventCallback instance = (IAnimationEventCallback) Activator.CreateInstance(type);
                    _eventCallbacks.Add(type.Name, instance);
                    _logger.InfoFormat("  {0} in {1}", type.Name, instance);
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("AnimationClipEvent:{0}",e);
                }
            }

            //假定长度一样
            List<string> className = new List<string>();
            foreach (KeyValuePair<string,IAnimationEventCallback> animationEventCallback in _eventCallbacks)
            {
                className.Add(animationEventCallback.Key);
            }
            className.Sort();
            for (int i = 0; i < className.Count; ++i)
            {
                if (i > short.MaxValue)
                {
                    throw new Exception(string.Format("className count:{0} is exceeted the short max:{1}", className.Count, short.MaxValue));
                }
                _eventIdToClassName.Add((short)i, className[i]);
                _classNameToEventId.Add(className[i], (short)i);
            }
        }

        static List<Type> FindAllAnimationEventCallbacktType()
        {
            
            Assembly[] assemblyList = AppDomain.CurrentDomain.GetAssemblies();
            List<Type> resList = new List<Type>();
            foreach (var assembly in assemblyList)
            {
                try
                {

                    List<Type> typeListInAssembly = DiscoverInAssembly(assembly);
                    resList.AddRange(typeListInAssembly);

                }
                catch (Exception e)
                {
                    var v = assembly;
                    _logger.ErrorFormat("error {0} in {1}", e, v);
                }
            }
            return resList;
        }

        static List<Type> DiscoverInAssembly(Assembly assembly)
        {
            try
            {
                Type[] typeList = assembly.GetTypes();
                List<Type> resList = new List<Type>();
                foreach (var type in typeList)
                {
                    
                    if (typeof(IAnimationEventCallback).IsAssignableFrom(type) 
                        && !type.IsAbstract 
                        && !type.IsInterface
                        && type.Namespace.Equals(NameSpace))
                    {
                       
                            resList.Add(type);
                    }
                }
                return resList;
            }
            catch (NotSupportedException)
            {
                //dononthing
                return new List<Type>();
            }


             
        }
        #endregion
        public PlayerEntity Player { set; private get; }

        public AnimationClipEvent()
        {

        }

        public void AnimationEventFunc(UnityEngine.AnimationEvent eventParam)
        {
            string strParam = eventParam.stringParameter;
            var cleanArr = _matcher.Match(strParam);
            if (cleanArr.Equals(_cleanStr))
            {// 复位操作，正常不执行，打断时执行
                return;
            }

            Func(strParam, eventParam);
        }

        public void InterruptAnimationEventFunc(UnityEngine.AnimationEvent eventParam)
        {
            string strParam = eventParam.stringParameter;
            var paramArr = strParam.Split('_');
            Func(paramArr[0], eventParam);
        }

        public void ServerFunc(short eventId, AnimationEventParam param)
        {
            string className;
             _eventIdToClassName.TryGetValue(eventId, out className);
            if (_eventCallbacks.ContainsKey(className))
            {
                _eventCallbacks[className].AnimationEventCallback(Player,param.EventParam, param.ToAnimationEvent());
                _logger.DebugFormat("invoke :class name:{0}, param:{1}", className, param.ToString());
            }
            else
            {
                _logger.ErrorFormat("Class {0} not exist", className);
            }
        }

        private void Func(string strParam, UnityEngine.AnimationEvent eventParam)
        {
            var paramArr = strParam.Split('|');
            var className = paramArr[0];
            string param = null;
            if (paramArr.Length > 1)
                param = paramArr[1];
            if (_eventCallbacks.ContainsKey(className))
            {
                EventParams.Add(new KeyValuePair<short, AnimationEventParam>(_classNameToEventId[className], new AnimationEventParam
                {
                    EventParam  = string.IsNullOrEmpty(param) ? string.Empty : param,
                    FloatParameter = eventParam.floatParameter,
                    IntParameter = eventParam.intParameter,
                    StringParameter = string.IsNullOrEmpty(eventParam.stringParameter) ? string.Empty : eventParam.stringParameter
                }));
                _eventCallbacks[className].AnimationEventCallback(Player,param, eventParam);
            }
            else
            {
                _logger.ErrorFormat("Class {0} not exist", className);
            }
//            Type t = Type.GetType(NameSpace + "." + className);
//            if (null == t)
//            {
//                _logger.ErrorFormat("Class {0} not exist", className);
//                return;
//            }
//            // 构造
//            object[] constructParams = new object[] { };
//            object obj = Activator.CreateInstance(t, constructParams);
//            // 找到method
//            MethodInfo method = t.GetMethod(_methodName);
//            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance;
//            object[] parameters = new object[] { Player, param };
//            // 执行回调
//            object returnValue = method.Invoke(obj, flag, Type.DefaultBinder, parameters, null);
        }
    }
}
