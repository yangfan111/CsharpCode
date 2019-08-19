using System;
using System.Collections.Generic;
using UnityEngine;

namespace UserInputManager.Lib
{
    public abstract class AbstractProvider<TKeyReceiver,TData> where TKeyReceiver : IKeyReceiver<TData> where TData : KeyData,new()
    {
        protected AbsKeyDataCollection<TKeyReceiver,TData> collection;
        protected  KeyConverter keyConverter;
        protected Queue<KeyData> KeyDatas = new Queue<KeyData>();
        private Queue<KeyData> insertQueue = new Queue<KeyData>();
        protected DataPool<TData> dataPool = new DataPool<TData>();

        public void SetConverter(KeyConverter converter)
        {
            keyConverter = converter;
        }

        public virtual void SetConfig(InputProviderConfig config)
        {
           
        }

        public void SetCollection(AbsKeyDataCollection<TKeyReceiver,TData> collection)
        {
            this.collection = collection;
        }

        public void SetDataPool(DataPool<TData> dataPool)
        {
            this.dataPool = dataPool;
        }

        public KeyData GetKeyData()
        {
            while (insertQueue.Count > 0)
            {
                KeyDatas.Enqueue(insertQueue.Dequeue());
            }
            return KeyDatas.Count > 0 ? KeyDatas.Dequeue() : null;
        }

        public void Insert(KeyData key)
        {
            insertQueue.Enqueue(key);
        }
        private List<UserInputKey> resultList = new List<UserInputKey>();
        protected List<UserInputKey> GetKeyList(TKeyReceiver data)
        {
         
            resultList.Clear();
            Dictionary<TData, KeyPointAction> binding = data.GetBindingActions();
            foreach (var pair in binding)
            {
                resultList.Add(pair.Key.Key);     
            }

            return resultList;

        }
        protected virtual void SetRaycastTargetData(KeyData data,  RayCastTarget target)
        {
        }

        public void Collect()
        {
            dataPool.RecycleAll();
            DoCollect();
        }

        protected abstract void DoCollect();
    }
}
