using System.Collections.Generic;

namespace App.Shared
{
    public class GameModuleLogicManagent
    {

        private static readonly HashSet<object> entireInstances = new HashSet<object>();

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


    }
    public class ModuleLogicActivator<T> where T : ModuleLogicActivator<T>, new()
    {

        internal static T s_Default { get; private set; }
        private static readonly Dictionary<int, T> logics = new Dictionary<int, T>();
        internal static T Get(int cookie)
        {
            if (logics.ContainsKey(cookie)) return logics[cookie];
            return default(T);
        }
        internal static void Clear()
        {
            logics.Clear();
        }
        internal static T Allocate(System.Action<T> processor)
        {
            var instance = new T();
            if (processor != null)
                processor(instance);
            int cookie = instance.GetHashCode();
            if (s_Default == null) s_Default = instance;
            logics.Add(cookie, instance);
            return instance;
        }
        internal static T Allocate(int cookie, System.Action<T> processor)
        {
            if (logics.ContainsKey(cookie)) return logics[cookie] ;
            var instance = new T();
            if (processor != null)
                processor(instance);
            if (s_Default == null) s_Default = instance;
            logics.Add(cookie, instance);
            return instance;
        }



    }
}