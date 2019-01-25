using System.Collections.Generic;
using Sharpen;
using com.cpkf.yyjd.tools.data.sort;
using com.cpkf.yyjd.tools.util;

namespace com.wd.free.para
{
	[System.Serializable]
	public class ParaList : IFeaturable
	{
		private const long serialVersionUID = -8515650288168248280L;

		protected internal MyDictionary<string, IPara> paras;

		[System.NonSerialized]
		private MyDictionary<string, Stack<IPara>> temp;

		public ParaList()
		{
			this.paras = new MyDictionary<string, IPara>();
		}

		public virtual void AddPara(IPara para)
		{
			this.paras[para.GetName()] = para;
		}

		public virtual bool HasPara(IPara para)
		{
			return this.paras.ContainsKey(para.GetName());
		}

		public virtual bool HasPara(string para)
		{
			return this.paras.ContainsKey(para);
		}

		public virtual void TempUse(IPara para)
		{
			if (temp == null)
			{
				temp = new MyDictionary<string, Stack<IPara>>();
			}
			if (!temp.ContainsKey(para.GetName()))
			{
				temp[para.GetName()] = new Stack<IPara>();
			}
			temp[para.GetName()].Push(paras[para.GetName()]);
			this.paras[para.GetName()] = para;
		}

		public virtual void Resume(string key)
		{
			IPara old = temp[key].Pop();
			if (old != null)
			{
				this.paras[key] = old;
			}
			else
			{
				this.paras.Remove(key);
			}
		}

		public virtual void RemovePara(IPara para)
		{
			this.paras.Remove(para.GetName());
		}

		public virtual void RemovePara(string name)
		{
			this.paras.Remove(name);
		}

		public virtual string[] GetFields()
		{
			return Sharpen.Collections.ToArray(paras.Keys, new string[0]);
		}

		public virtual void Merge(com.wd.free.para.ParaList list)
		{
			foreach (IPara p in list.paras.Values)
			{
				if (!this.paras.ContainsKey(p.GetName()))
				{
					this.paras[p.GetName()] = p;
				}
			}
		}

		public virtual int Count()
		{
			return this.paras.Count;
		}

		public virtual IPara Get(string name)
		{
			return paras[name];
		}

		public override string ToString()
		{
			IList<string> list = new List<string>();
			foreach (IPara pa in paras.Values)
			{
				list.Add(pa.ToString());
			}
			return "{" + StringUtil.GetStringFromStrings(list, ", ") + "}";
		}

		public virtual bool Meet(string exp)
		{
			return new SelectMethod(exp).Meet(this);
		}

		public virtual bool HasFeature(string feature)
		{
			return HasPara(feature);
		}

		public virtual com.wd.free.para.ParaList Clone()
		{
			com.wd.free.para.ParaList pl = new com.wd.free.para.ParaList();
			pl.paras.PutAll(this.paras);
			return pl;
		}

		public virtual MyDictionary<string, IPara> GetMapPara()
		{
			return this.paras;
		}

		public virtual object GetFeatureValue(string feature)
		{
			if (HasFeature(feature))
			{
				object v = Get(feature).GetValue();
				return v;
			}
			else
			{
				return null;
			}
		}
	}
}
