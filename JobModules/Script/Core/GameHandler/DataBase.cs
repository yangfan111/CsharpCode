using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Utils;

namespace Core.GameHandler
{
    public interface IDataBase
    {

    }

    public class DataBase<T> : IDataBase where T: struct
    {
        private Dictionary<string, T> _dataCluster = new Dictionary<string, T>();

        public bool CompareAndStore(string key, T val) 
        {
            T storedVal;
            bool isDiff = true;
            if (_dataCluster.TryGetValue(key, out storedVal))
            {
                isDiff = !val.Equals(storedVal);
            }

            if (isDiff)
            {
                _dataCluster[key] = val;
            }

            return isDiff;
        }
    }
}
