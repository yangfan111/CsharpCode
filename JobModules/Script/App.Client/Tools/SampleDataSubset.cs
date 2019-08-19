using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityVector2;
using Core.Utils;

namespace Assets.App.Client.Tools
{
    public class SampleDataSubset : IEnumerable<KeyValuePair<string, float>>, IResetable
    {
        private Dictionary<string, float> _dataSet = new Dictionary<string, float>();

        public void Add(string name, float value)
        {
            float originalValue = default(float);
            if (!_dataSet.TryGetValue(name, out originalValue))
            {
                _dataSet.Add(name, value);
            }
            else
            {
                _dataSet[name] = originalValue + value;
            }
        }

        public void Add(SampleDataSubset set)
        {
            foreach (var data in set._dataSet)
            {
                float val;
                if (!_dataSet.TryGetValue(data.Key, out val))
                {
                    _dataSet.Add(data.Key, val);
                }
                else
                {
                    _dataSet[data.Key] = val + data.Value;
                }
            }
        }

        static List<string> _keysList = new List<string>();
        public void Divide(float val)
        {
            AssertUtility.Assert(val > 0);

            _keysList.Clear();
            _keysList.AddRange(_dataSet.Keys);
            foreach (var key in _keysList)
            {
                _dataSet[key] = _dataSet[key] / val;
            }
        }

        public IEnumerator<KeyValuePair<string, float>> GetEnumerator()
        {
            return _dataSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dataSet.GetEnumerator();
        }


        public void Reset()
        {
            _dataSet.Clear();
        }
    }
}
