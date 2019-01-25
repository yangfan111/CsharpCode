using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Core.Utils;

namespace Core.GameHandler
{
    public class DataBaseComponent
    {
        protected Dictionary<Type, IDataBase> _dataBases = new Dictionary<Type, IDataBase>();

        public bool CompareAndStore<T>(string key, T value) where T: struct
        {
            IDataBase pool;
            if (!_dataBases.TryGetValue(typeof(T), out pool))
            {
                pool = new DataBase<T>();
                _dataBases[typeof(T)] = pool;
            }

            return (pool as DataBase<T>).CompareAndStore(key, value);
        }
    }
}
