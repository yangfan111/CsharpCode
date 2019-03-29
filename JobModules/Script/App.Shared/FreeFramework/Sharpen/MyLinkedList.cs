using System.Collections.Generic;

namespace Sharpen
{
    public class MyLinkedList<T> : LinkedList<T>
    {
        public T RemoveHead()
        {
            T t = this.First.Value;
            RemoveFirst();
            return t;
        }

        public void AddTail(T t)
        {
            AddLast(t);
        }
    }
}