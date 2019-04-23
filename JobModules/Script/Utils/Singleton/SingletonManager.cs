using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;

namespace Utils.Singleton
{
    public class Singleton<T> where T : Singleton<T>, new()
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(Singleton<T>));
        internal static T Instance;

        static Singleton()
        {
            Instance = new T();
            SingletonManager.RegisterSingleton<T>();
            _logger.DebugFormat("Singleton {0} is initialized.", typeof(T));
        }
    }

    public interface IDisposableSingleton : IDisposable
    { }

    public abstract class DisposableSingleton<T>: Singleton<T>, IDisposableSingleton where T: DisposableSingleton<T>, new()
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(DisposableSingleton<T>));

        public void Dispose()
        {
            OnDispose();
            SingletonManager.UnregisterSingleton<T>();
            _logger.DebugFormat("DisposableSingleton {0} is disposed.", typeof(T));

            //creat a new singleton on dispose.
            Instance = new T();
            SingletonManager.RegisterSingleton<T>();
            _logger.DebugFormat("DisposableSingleton {0} is initialized.", typeof(T));
        }

        protected abstract void OnDispose();
    }

    public class SingletonManager
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SingletonManager));

        private static readonly object LockObj = new object();
        private static SingletonManager _instance;

        private HashSet<IDisposable> _singletons = new HashSet<IDisposable>();
        private bool _disposed;


        private SingletonManager()
        {
           
        }

        ~SingletonManager()
        {
            Dispose(false);
        }

        internal static void RegisterSingleton<T>() where T: Singleton<T>, new()
        {
            var singleton = Get<T>() as IDisposableSingleton;
            if (singleton != null)
            {
                lock (LockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new SingletonManager();
                    }

                    _instance._singletons.Add(singleton);
                }

            }

        }

        internal static void UnregisterSingleton<T>() where T : Singleton<T>, new()
        {
            var singleton = Get<T>() as IDisposableSingleton;
            if (singleton != null)
            {
                lock (LockObj)
                {
                    if(_instance != null)
                        _instance._singletons.Remove(singleton);
                }
            }
            
        }

        public static void Dispose()
        {
            lock (LockObj)
            { 
                var instance = _instance;
                _instance = null;
                if (instance != null)
                {
                    instance.InternalDispose();
                }

                
            }
            
        }

        public static T Get<T>() where T : Singleton<T>, new()
        {
            return Singleton<T>.Instance;
        }

        private void InternalDispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                var singletons = new  List<IDisposable>(_singletons);
                foreach (var singleton in singletons)
                {
                    try
                    {
                        singleton.Dispose();
                    }
                    catch (Exception e)
                    {
                       _logger.ErrorFormat("Dispose Singleton Error {0}", e);
                    }
                    
                }

                _disposed = true;
            }
        }

    }
}
