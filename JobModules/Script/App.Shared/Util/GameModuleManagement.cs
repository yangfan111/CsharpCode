using Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Shared
{
    public class GameModuleManagement
    {
        private static readonly HashSet<IDisposable> entireInstances = new HashSet<IDisposable>();

        internal static void Submit<T>(T instance) where T : ModuleLogicActivator<T>, new()
        {
            entireInstances.Add(instance);
        }
        public static T Get<T>() where T : ModuleLogicActivator<T>, new()
        {
            return ModuleLogicActivator<T>.s_Default;
        }
        public static T Get<T>(int cookie) where T : ModuleLogicActivator<T>, new()
        {
            return ModuleLogicActivator<T>.Get(cookie);
        }
       
        public static T Allocate<T>(int cookie, System.Action<T> processor = null) where T : ModuleLogicActivator<T>, new()
        {
            var instance = ModuleLogicActivator<T>.Allocate(cookie, processor);
            Submit(instance);
            return instance;
        }
        public static void ForceCache<T>(int cookie,T instance) where T : ModuleLogicActivator<T>, new()
        {
            ModuleLogicActivator<T>.Cache(instance,cookie);
        }
        public static T ForceAllocate<T>(int cookie, System.Action<T> processor = null) where T : ModuleLogicActivator<T>, new()
        {
            var instance = ModuleLogicActivator<T>.ForceAllocate(cookie, processor);
            Submit(instance);
            return instance;
        }
        public static void Dispose()
        {
            var disposeArray = entireInstances.ToList();
            foreach(var data in disposeArray)
            {
                data.Dispose();
            }
        }
        internal static void UnCache(IDisposable instance)
        {
            entireInstances.Remove(instance);
        }
     

    }
    public class ModuleLogicActivator<T> : IDisposable where T : ModuleLogicActivator<T>, new()
    {

        internal static T s_Default { get; private set; }
        private readonly static Dictionary<int, T> logics = new Dictionary<int, T>();
        private int cookie;
        internal static T Get(int cookie)
        {
            
            if (s_Default != null && s_Default.cookie == cookie) return s_Default;
            if (logics.ContainsKey(cookie))
            {
          //      DebugUtil.LogInUnity(logics[cookie].ToString());
                return logics[cookie];
            }
            return default(T);
        }
        internal static void Cache(T instance,int cookie)
        {
            instance.cookie = cookie;
            if (logics.ContainsKey(cookie))
            {
                var tmp = logics[cookie];
                tmp.Dispose();
            }
            if (s_Default == null) s_Default = instance;
            logics[cookie] = instance;
        }
        internal static T Allocate(int cookie, System.Action<T> processor)
        {
            var instance = new T();
            instance.cookie = cookie;
            if (s_Default == null) s_Default = instance;
            logics[cookie] = instance;
            if (processor != null)
                processor(instance);
            return instance;
        }
        internal static T ForceAllocate(int cookie, System.Action<T> processor)
        {
            if (logics.ContainsKey(cookie))
            {
                var tmp = logics[cookie];
                tmp.Dispose();
            }
           return Allocate(cookie, processor);
        }
        public void Dispose()
        {
            if(logics.ContainsKey(cookie))
                logics.Remove(cookie);
            if (s_Default == this)
                s_Default = null;
            GameModuleManagement.UnCache(this);
        }
       
    }
}