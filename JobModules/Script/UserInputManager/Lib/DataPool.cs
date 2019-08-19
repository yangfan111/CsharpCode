using System.Collections.Generic;

namespace UserInputManager.Lib
{
    public interface IReusableData
    {
        void Reset();
    }

    public class DataPool<T> where T : IReusableData, new()
    {
        public List<T> usedKeyDatas = new List<T>();
        public Queue<T> unusedKeyDatas = new Queue<T>();

        public T GetData()
        {
            T data;
            if(unusedKeyDatas.Count < 1)
                data = new T();
            else data = unusedKeyDatas.Dequeue();
            usedKeyDatas.Add(data);
            return data;
        }

        public void RecycleAll()
        {
            foreach(var data in usedKeyDatas)
            {
                data.Reset();
                unusedKeyDatas.Enqueue(data);
            }
            usedKeyDatas.Clear();
        }
    }
}