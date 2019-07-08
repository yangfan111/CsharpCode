using System.Collections.Generic;
using Sharpen;
using System;

namespace com.wd.free.para
{
	public class ParaPool
	{
		private Stack<object> intList = new Stack<object>();

		private Type t;

        private object obj;

        protected long miMaxCount;

        protected long miNoRecycleCount;

        protected long Count;

        public ParaPool(object obj, int iMaxCount = 0)
		{
            this.obj = obj;
			this.t = obj.GetType();
            this.miMaxCount = iMaxCount;
            this.Count = 0;
            for (int i = 0; i < iMaxCount; i++) {
                this.intList.Push(Activator.CreateInstance(this.t));
                this.Count++;
            }
		}

        public void Enlarge(int iExpandCound)
        {
            if (iExpandCound < 0)
            {
                this.miMaxCount = -1;
                return;
            }
            if (this.miMaxCount >= 0)
            {
                this.miMaxCount = this.miNoRecycleCount + this/*.intList.Count*/.Count + iExpandCound;
            }
            for (int i = 0; i < iExpandCound; i++)
            {
                this.intList.Push(Activator.CreateInstance(this.t));
                this.Count++;
            }
        }

        public object Spawn(bool bCreateIfPoolEmpty)
        {
            if (this.Count > 0)
            {
                object t = this.intList.Pop();
                this.Count--;
                if (t == null)
                {
                    if (!bCreateIfPoolEmpty)
                    {
                        return (object)null;
                    }
                    t = Activator.CreateInstance(this.t);
                }
                this.miNoRecycleCount++;
                return t;
            }
            if (bCreateIfPoolEmpty)
            {
                object result = Activator.CreateInstance(this.t);
                this.miNoRecycleCount++;
                return result;
            }
            return (object)null;
        }

        public bool Recycle(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            this.miNoRecycleCount--;
            if (this/*.intList.Count*/.Count >= this.miMaxCount && this.miMaxCount > 0)
            {
                obj = (object)null;
                return false;
            }
            this.intList.Push(obj);
            this.Count++;
            return true;
        }

        public void Clear()
        {
            if (this.intList != null)
            {
                this.intList.Clear();
                this.Count = 0;
            }
        }

        public virtual IPara BorrowObject()
		{
			if (/*intList.IsEmpty()*/0 == this.Count)
			{
				IPara p = (IPara)this.obj.Copy();
				if (p is AbstractPara)
				{
					((AbstractPara)p).SetTemp(true);
				}
				intList.Push(p);
                this.Count++;
            }
            object temp = intList.Pop();
            this.Count--;
            return temp as IPara;
		}
	}
}
