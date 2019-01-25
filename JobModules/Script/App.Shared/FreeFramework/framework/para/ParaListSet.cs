using System;
using System.Collections;
using System.Collections.Generic;
using com.cpkf.yyjd.tools.data.sort;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.exception;
using com.wd.free.para.exp;
using Sharpen;

namespace com.wd.free.para
{
	[System.Serializable]
	public class ParaListSet : AbstractPara, IEnumerable<ParaList>
	{
		private const long serialVersionUID = 2162997136596560057L;

		public const string PARA_ORDER = "order";

		private string order;

		private string unique;

		private int capacity;

		private ICollection<string> uniKeys;

		private IList<ParaList> list;

		[System.NonSerialized]
		private MyDictionary<string, MyDictionary<string, ParaList>> cache;

		public ParaListSet()
		{
			this.list = new List<ParaList>();
			this.capacity = 100;
		}

		public ParaListSet(string order, int capacity)
			: this(string.Empty, order, capacity)
		{
		}

		public ParaListSet(string name, string order, int capacity)
			: base()
		{
			this.list = new List<ParaList>();
			this.name = name;
			this.order = order;
			this.capacity = capacity;
		}

		public virtual string GetOrder()
		{
			return order;
		}

		public virtual void SetOrder(string order)
		{
			this.order = order;
		}

		public virtual string GetUnique()
		{
			return unique;
		}

		public virtual void SetUnique(string unique)
		{
			this.unique = unique;
		}

		public virtual void Clear()
		{
			this.list.Clear();
		}

		public virtual void RemoveParaList(ParaList paras)
		{
			this.list.Remove(paras);
		}

		public override string[] GetConditions()
		{
			return new string[] { CON_EQUAL, CON_GE, CON_GREATER, CON_LE, CON_LESS, CON_NOT_EQUAL };
		}

		public override bool Meet(string con, IPara v)
		{
			int v1 = list.Count;
			int v2 = (int)v.GetValue();
			switch (con)
			{
				case CON_EQUAL:
				{
					return v1 == v2;
				}

				case CON_NOT_EQUAL:
				{
					return v1 != v2;
				}

				case CON_GE:
				{
					return v1 >= v2;
				}

				case CON_LE:
				{
					return v1 <= v2;
				}

				case CON_GREATER:
				{
					return v1 > v2;
				}

				case CON_LESS:
				{
					return v1 < v2;
				}
			}
			return false;
		}

		public virtual void ReOrder()
		{
			this.list = Select(this.list, order, capacity);
		}

		public virtual com.wd.free.para.ParaListSet Sort(string exp)
		{
			com.wd.free.para.ParaListSet pls = new com.wd.free.para.ParaListSet();
			pls.list = Select(this.list, exp, capacity);
			pls.order = order;
			pls.unique = unique;
			return pls;
		}

		public virtual ParaList GetParaList(string field, string value, IEventArgs args)
		{
			if (StringUtil.IsNullOrEmpty(order) && StringUtil.IsNullOrEmpty(unique))
			{
				if (cache == null)
				{
					cache = new MyDictionary<string, MyDictionary<string, ParaList>>();
				}
				if (cache.ContainsKey(field) && cache[field].ContainsKey(value))
				{
					return cache[field][value];
				}
			}
			else
			{
				foreach (ParaList pl in list)
				{
					IPara para = pl.Get(field);
					if (para != null)
					{
						try
						{
							IPara temp = para.Initial("==", value);
							bool m = para.Meet("==", temp);
							temp.Recycle();
							if (m)
							{
								return pl;
							}
						}
						catch (Exception)
						{
						}
						UnitPara up = UnitPara.ParseOne(value);
						IPara vPara = up.GetPara(args);
						if (vPara != null && para.Meet("==", vPara))
						{
							return pl;
						}
					}
				}
			}
			return null;
		}

		public virtual void AddParaList(ParaList paras)
		{
			if (cache == null)
			{
				cache = new MyDictionary<string, MyDictionary<string, ParaList>>();
			}
			if (StringUtil.IsNullOrEmpty(order) && StringUtil.IsNullOrEmpty(unique))
			{
				if (this.list.Count < this.capacity)
				{
					this.list.Add(paras);
					foreach (string f in paras.GetFields())
					{
						if (!cache.ContainsKey(f))
						{
							cache[f] = new MyDictionary<string, ParaList>();
						}
						IPara p = paras.Get(f);
						if (p != null && p.GetValue() != null)
						{
							cache[f][p.GetValue().ToString()] = paras;
						}
						else
						{
							throw new GameConfigExpception(paras + " has null element");
						}
					}
				}
			}
			else
			{
				this.list.Add(paras);
				this.list = Select(this.list, this.order, this.capacity);
			}
		}

		private string GetKey(ParaList paras)
		{
			if (!StringUtil.IsNullOrEmpty(unique))
			{
				if (uniKeys == null)
				{
					uniKeys = new HashSet<string>();
					foreach (string uni in StringUtil.Split(unique, ","))
					{
						uniKeys.Add(uni.Trim());
					}
				}
				IList<string> list = new List<string>();
				foreach (string uni_1 in uniKeys)
				{
					IPara para = paras.Get(uni_1);
					if (para != null)
					{
						list.Add(para.GetValue().ToString());
					}
				}
				string key = StringUtil.GetStringFromStrings(list, "-");
				if (!StringUtil.IsNullOrEmpty(key))
				{
					return key;
				}
			}
			return null;
		}

		

		public virtual IList<ParaList> Select(IList<ParaList> list, string order, int capacity)
		{
			ICollection<string> exists = new HashSet<string>();
			IList<ParaList> result = new List<ParaList>();
			if (StringUtil.IsNullOrEmpty(order))
			{
				foreach (ParaList pl in list)
				{
					string key = GetKey(pl);
					if (StringUtil.IsNullOrEmpty(key) || !exists.Contains(key))
					{
						result.Add(pl);
						if (!StringUtil.IsNullOrEmpty(key))
						{
							exists.Add(key);
						}
					}
					if (capacity > 0 && result.Count >= capacity)
					{
						break;
					}
				}
			}
			else
			{
				DataBlock bl = new DataBlock();
				foreach (ParaList pl in list)
				{
					bl.AddData(new ParaListSet.ParaFeature(pl));
				}
				SelectMethod sm = new SelectMethod(order);
				int i = 1;
				foreach (IFeaturable fe in sm.Select(bl).GetAllDatas())
				{
					ParaList pl_1 = ((ParaListSet.ParaFeature)fe).paraList;
					pl_1.AddPara(new IntPara(PARA_ORDER, i++));
					string key = GetKey(pl_1);
					if (StringUtil.IsNullOrEmpty(key) || !exists.Contains(key))
					{
						result.Add(pl_1);
						if (!StringUtil.IsNullOrEmpty(key))
						{
							exists.Add(key);
						}
					}
					if (capacity > 0 && result.Count >= capacity)
					{
						break;
					}
				}
			}
			return result;
		}

		[System.Serializable]
		internal class ParaFeature : SimpleData
		{
			public ParaList paraList;

			public ParaFeature(ParaList paraList)
			{
				this.paraList = paraList;
				foreach (string f in paraList.GetFields())
				{
					if (paraList.Get(f) != null)
					{
						AddFeatureValue(f, paraList.Get(f).GetValue());
					}
				}
			}

			public virtual ParaList GetParaList()
			{
				return paraList;
			}
		}

		public override IPara Initial(string con, string v)
		{
			return new IntPara(string.Empty, int.Parse(v));
		}

		public override string ToString()
		{
			return list.ToString();
		}

		public override IPoolable Copy()
		{
			ParaListSet pls = new ParaListSet(this.order, this.capacity);
			pls.capacity = this.capacity;
			pls.name = this.name;
			pls.desc = this.desc;
			pls.order = this.order;
			pls.unique = this.unique;
			return pls;
		}

		private static ParaPool<IPara> pool = new ParaPool<IPara>(new ParaListSet());

		protected internal override ParaPool<IPara> GetPool()
		{
			return pool;
		}

        public IEnumerator<ParaList> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
