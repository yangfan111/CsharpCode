using System;
using Sharpen;

namespace com.graphbuilder.struc
{
	public class Bag
	{
		protected internal object[] data = null;

		protected internal int size = 0;

		public Bag()
		{
			data = new object[2];
		}

		public Bag(int initialCapacity)
		{
			data = new object[initialCapacity];
		}

		public virtual void Add(object o)
		{
			Insert(o, size);
		}

		public virtual int Size()
		{
			return size;
		}

		public virtual void Insert(object o, int i)
		{
			if (i < 0 || i > size)
			{
				throw new ArgumentException("required: (i >= 0 && i <= size) but: (i = " + i + ", size = " + size + ")");
			}
			EnsureCapacity(size + 1);
			for (int j = size; j > i; j--)
			{
				data[j] = data[j - 1];
			}
			data[i] = o;
			size++;
		}

		public virtual void EnsureCapacity(int minCapacity)
		{
			if (minCapacity > data.Length)
			{
				int x = 2 * data.Length;
				if (x < minCapacity)
				{
					x = minCapacity;
				}
				object[] arr = new object[x];
				for (int i = 0; i < size; i++)
				{
					arr[i] = data[i];
				}
				data = arr;
			}
		}

		public virtual int GetCapacity()
		{
			return data.Length;
		}

		private int IndexOf(object o, int i, bool forward)
		{
			if (size == 0)
			{
				return -1;
			}
			if (i < 0 || i >= size)
			{
				throw new ArgumentException("required: (i >= 0 && i < size) when: (size > 0) but: (i = " + i + ", size = " + size + ")");
			}
			if (forward)
			{
				if (o == null)
				{
					for (; i < size; i++)
					{
						if (data[i] == null)
						{
							return i;
						}
					}
				}
				else
				{
					for (; i < size; i++)
					{
						if (o.Equals(data[i]))
						{
							return i;
						}
					}
				}
			}
			else
			{
				if (o == null)
				{
					for (; i >= 0; i--)
					{
						if (data[i] == null)
						{
							return i;
						}
					}
				}
				else
				{
					for (; i >= 0; i--)
					{
						if (o.Equals(data[i]))
						{
							return i;
						}
					}
				}
			}
			return -1;
		}

		public virtual int Remove(object o)
		{
			int i = IndexOf(o, 0, true);
			if (i >= 0)
			{
				Remove(i);
			}
			return i;
		}

		public virtual object Remove(int i)
		{
			if (i < 0 || i >= size)
			{
				throw new ArgumentException("required: (i >= 0 && i < size) but: (i = " + i + ", size = " + size + ")");
			}
			object o = data[i];
			for (int j = i + 1; j < size; j++)
			{
				data[j - 1] = data[j];
			}
			data[--size] = null;
			return o;
		}

		public virtual object Get(int i)
		{
			if (i < 0 || i >= size)
			{
				throw new ArgumentException("required: (i >= 0 && i < size) but: (i = " + i + ", size = " + size + ")");
			}
			return data[i];
		}

		public virtual bool Contains(object o)
		{
			return IndexOf(o, 0, true) >= 0;
		}
	}
}
