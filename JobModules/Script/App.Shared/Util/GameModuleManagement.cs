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
        public static T Allocate<T>(System.Action<T> processor = null) where T : ModuleLogicActivator<T>, new()
        {
            var instance = ModuleLogicActivator<T>.Allocate(processor);
            Submit(instance);
            return instance;
        }
        public static T Allocate<T>(int cookie, System.Action<T> processor = null) where T : ModuleLogicActivator<T>, new()
        {
            var instance = ModuleLogicActivator<T>.Allocate(cookie, processor);
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
            disposeArray.Clear();
        }
        internal static void UnCache(IDisposable instance)
        {
            entireInstances.Remove(instance);
        }
     

    }
    public class ModuleLogicActivator<T> : IDisposable where T : ModuleLogicActivator<T>, new()
    {

        internal static T s_Default { get; private set; }
        private static readonly Dictionary<int, T> logics = new Dictionary<int, T>();
        private int cookie;
        internal static T Get(int cookie)
        {
            if (s_Default.cookie == cookie) return s_Default;
            if (logics.ContainsKey(cookie)) return logics[cookie];
            return default(T);
        }
        internal static void Clear()
        {
            s_Default = null;
            logics.Clear();
        }
        internal static T Allocate(System.Action<T> processor)
        {
            var instance = new T();
            int cookie = instance.GetHashCode();
            instance.cookie = cookie;
            if (processor != null)
                processor(instance);
            if (s_Default == null) s_Default = instance;
            logics.Add(cookie, instance);
            return instance;
        }
        internal static T Allocate(int cookie, System.Action<T> processor)
        {
            if (logics.ContainsKey(cookie)) return logics[cookie] ;
            var instance = new T();
            instance.cookie = cookie;
            if (processor != null)
                processor(instance);
            if (s_Default == null) s_Default = instance;
            logics.Add(cookie, instance);
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
            logics.Remove(cookie);
            if (s_Default == this)
                s_Default = null;
            GameModuleManagement.UnCache(this);
        }
       
    }
}