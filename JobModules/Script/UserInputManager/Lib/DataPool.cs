using System.Collections.Generic;

namespace UserInputManager.Lib
{
    public interface IReusableData
    {
        void Reset();
    }

    public class DataPool<T> where T : IReusableData, new()
    {
        public List<T> _keyDatas = new List<T>();
        public Queue<T> _unusedKeyDatas = new Queue<T>();

        public T GetData()
        {
            if(_unusedKeyDatas.Count < 1)
            {
                var data = new T();
                _keyDatas.Add(data);
                return data;
            }
            return _unusedKeyDatas.Dequeue();
        }

        public void RecycleAll()
        {
            _unusedKeyDatas.Clear();
            foreach(var data in _keyDatas)
            {
                data.Reset();
                _unusedKeyDatas.Enqueue(data);
            }
        }
    }
}