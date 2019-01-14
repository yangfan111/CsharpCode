using System.Collections.Generic;
using Sharpen;

namespace com.wd.free.para
{
	public class ParaPool<T>
		where T : IPara
	{
		private Stack<T> intList = new Stack<T>();

		private T t;

		public ParaPool(T t)
		{
			this.t = t;
		}

		public virtual T BorrowObject()
		{
			if (intList.IsEmpty())
			{
				IPara p = (IPara)t.Copy();
				if (p is AbstractPara)
				{
					((AbstractPara)p).SetTemp(true);
				}
				intList.Push((T)p);
			}

		    return intList.Pop();
		}

		public virtual void ReturnObject(T para)
		{
			intList.Push(para);
		}
	}
}
