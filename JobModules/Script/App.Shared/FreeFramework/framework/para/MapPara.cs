using System;
using System.Collections.Generic;
using Sharpen;

namespace com.wd.free.para
{
	[System.Serializable]
	public class MapPara : AbstractPara
	{
		private const long serialVersionUID = -4439935994846733590L;

		private MyDictionary<string, IPara> map;

		private bool number;

		public MapPara()
		{
			this.map = new MyDictionary<string, IPara>();
		}

		public MapPara(string name)
		{
			this.name = name;
			this.map = new MyDictionary<string, IPara>();
		}

		public MapPara(string name, bool number)
		{
			this.name = name;
			this.number = number;
			this.map = new MyDictionary<string, IPara>();
		}

		public virtual int Size()
		{
			return map.Count;
		}

		public override IPara Initial(string con, string v)
		{
			if ("==".Equals(con))
			{
				try
				{
					return new IntPara(EMPTY_NAME, int.Parse(v));
				}
				catch (Exception)
				{
					return new IntPara(EMPTY_NAME, 0);
				}
			}
			else
			{
				if (CON_NOT_EQUAL.Equals(con))
				{
					return new StringPara(EMPTY_NAME, v);
				}
			}
			return new com.wd.free.para.MapPara();
		}

		public virtual IPara GetValue(string key)
		{
			if (!map.ContainsKey(key))
			{
				if (!number)
				{
					map[key] = new StringPara(key, "null");
				}
				else
				{
					map[key] = new IntPara(key, 0);
				}
			}
			return map[key];
		}

		public override string[] GetConditions()
		{
			return new string[] { "==", CON_NOT_EQUAL };
		}

		public override bool Meet(string con, IPara v)
		{
			if (v == null)
			{
				return false;
			}
			if ("==".Equals(con))
			{
				if (v is IntPara)
				{
					return map.Count == (int)v.GetValue();
				}
			}
			else
			{
				if (CON_NOT_EQUAL.Equals(con))
				{
					if (v is StringPara)
					{
						return map.ContainsKey((string)v.GetValue());
					}
				}
			}
			return false;
		}

		public override IPoolable Copy()
		{
			com.wd.free.para.MapPara clone = new com.wd.free.para.MapPara();
			clone.name = name;
			clone.number = number;
			clone.map.PutAll(this.map);
			return clone;
		}

		[System.NonSerialized]
		internal static ParaPool<IPara> pool = new ParaPool<IPara>(new com.wd.free.para.MapPara());

		protected internal override ParaPool<IPara> GetPool()
		{
			return pool;
		}

		public override string ToString()
		{
			return name + "=" + map.ToString();
		}
	}
}
