using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.Appearance
{
    public abstract class AdaptiveContainerImpl<T, T1> : IAdaptiveContainer<T>
    where T1:T, new()
    {
        protected Func<T, bool> _defaultGetItemCondition = null;

        protected AdaptiveContainerImpl(int initSize)
        {
            _list = new T[initSize];
            for (int i = 0; i < initSize; ++i)
            {
                _list[i] = new T1();
            }
        }

        private T[] _list;

        public T GetAvailableItem()
        {
            int ret = GetAvailable(_defaultGetItemCondition);
            return _list[ret];
        }

        public T GetAvailableItem(Func<T, bool> getItemCondition)
        {
            int ret = GetAvailable(getItemCondition);
            return _list[ret];
        }

        public T this[int index]
        {
            get { return _list[index];}
            set { _list[index] = value; }
        }

        public int Length
        {
            get { return _list.Length; }
        }

        protected virtual int GetAvailable(Func<T, bool> getItemCondition)
        {
            throw new NotImplementedException();
        }

        //Array.Resize会创建一个新的数组，并且把旧的数据复制到新的数组上，返回新的数组,_list引用不是原来的引用了
        protected void Resize(int newSize)
        {
            Array.Resize(ref _list, newSize);
        }
    }
}
