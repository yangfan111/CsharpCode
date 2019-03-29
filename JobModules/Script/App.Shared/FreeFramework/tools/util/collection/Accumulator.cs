using System;
using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.cpkf.yyjd.tools.util.collection
{
	[System.Serializable]
	public class Accumulator<T>
	{
		private const long serialVersionUID = 9056230844665828614L;

		private MyDictionary<T, int> map;

		private int allCount;

		public Accumulator()
		{
			this.map = new MyDictionary<T, int>();
		}

		public virtual void AddKey(T t)
		{
			if (!map.ContainsKey(t))
			{
				map[t] = 0;
			}
			map[t] = map[t] + 1;
			allCount++;
		}

		public virtual void AddKey(T t, int count)
		{
			if (!map.ContainsKey(t))
			{
				map[t] = 0;
			}
			map[t] = map[t] + count;
			allCount = allCount + count;
		}

		public virtual void MinusKey(T t)
		{
			if (map.ContainsKey(t))
			{
				map[t] = map[t] - 1;
				allCount--;
				if (map[t] == 0)
				{
					map.Remove(t);
				}
			}
		}

		public virtual bool ContainsKey(T t)
		{
			return map.ContainsKey(t);
		}

		public virtual ICollection<T> GetKeys()
		{
			return map.Keys;
		}

		public virtual void Clear()
		{
			this.map.Clear();
			allCount = 0;
		}

		public virtual void Clear(T t)
		{
			allCount = allCount - map[t];
			this.map[t] = 0;
		}

		public virtual void Delete(T t)
		{
			allCount = allCount - map[t];
			this.map.Remove(t);
		}

		public virtual int GetCount(T t)
		{
			if (map.ContainsKey(t))
			{
				return map[t];
			}
			else
			{
				return 0;
			}
		}

		public virtual int GetAllCount()
		{
			return allCount;
		}

		public virtual int KeyCount()
		{
			return map.Count;
		}

		public virtual IList<T> KeysSortedByValue()
		{
			List<KV> list = new List<KV>();
			foreach (T k in map.Keys)
			{
				list.Add(new KV(this, k, map[k]));
			}
			list.Sort();
			List<T> result = new List<T>();
			foreach (KV kv in list)
			{
				result.Add(kv.GetK());
			}
			return result;
		}

		public virtual IList<T> KeysSortedByValue(int count)
		{
			List<T> list = new List<T>();
			foreach (object obj in SortUtil.GetTopsByValue(map, count).Keys)
			{
				list.Add((T)obj);
			}
			return list;
		}

		public virtual IList<T> KeysSortedByKey()
		{
			List<IComparable> list = new List<IComparable>();
			foreach (T key in map.Keys)
			{
				list.Add((IComparable)key);
			}
			list.Sort();
			return (IList<T>)list;
		}

		public virtual com.cpkf.yyjd.tools.util.collection.Accumulator<T> Clone()
		{
			com.cpkf.yyjd.tools.util.collection.Accumulator<T> clone = new com.cpkf.yyjd.tools.util.collection.Accumulator<T>();
			clone.map = new MyDictionary<T, int>();
			foreach (T key in this.map.Keys)
			{
				clone.map[key] = this.map[key];
			}
			clone.allCount = allCount;
			return clone;
		}

		public virtual com.cpkf.yyjd.tools.util.collection.Accumulator<int> GetStatistics()
		{
			com.cpkf.yyjd.tools.util.collection.Accumulator<int> acc = new com.cpkf.yyjd.tools.util.collection.Accumulator<int>();
			foreach (int v in map.Values)
			{
				acc.AddKey(v);
			}
			return acc;
		}

		public override string ToString()
		{
			return this.map.ToString();
		}

		internal class KV : IComparable<KV>
		{
			private T k;

			private int v;

			public KV(Accumulator<T> _enclosing, T k, int v)
			{
				this._enclosing = _enclosing;
				this.k = k;
				this.v = v;
			}

			public virtual T GetK()
			{
				return this.k;
			}

			public virtual void SetK(T k)
			{
				this.k = k;
			}

			public virtual int GetV()
			{
				return this.v;
			}

			public virtual void SetV(int v)
			{
				this.v = v;
			}

			public virtual int CompareTo(KV kv)
			{
				if (this.v < kv.v)
				{
					return 1;
				}
				else
				{
					if (this.v == kv.v)
					{
						return 0;
					}
					else
					{
						return -1;
					}
				}
			}

			private readonly Accumulator<T> _enclosing;
		}
	}
}
