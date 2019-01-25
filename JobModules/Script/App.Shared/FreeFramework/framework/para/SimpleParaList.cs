using System.Collections.Generic;
using Sharpen;

namespace com.wd.free.para
{
    [System.Serializable]
    public class SimpleParaList : ParaList
    {
        private const long serialVersionUID = -1266253248791630602L;

        [System.NonSerialized]
        private IList<IFields> fieldList;

        public SimpleParaList()
            : base()
        {
            this.fieldList = new List<IFields>();
        }

        public virtual void AddFields(IFields fields)
        {
            if (fieldList == null)
            {
                fieldList = new List<IFields>();
            }
            this.fieldList.Add(fields);
        }

        public virtual void RemoveFields(IFields fields)
        {
            this.fieldList.Remove(fields);
        }

        public virtual IList<IFields> GetFieldList()
        {
            return fieldList;
        }

        public override bool HasPara(IPara para)
        {
            bool has = this.paras.ContainsKey(para.GetName());
            if (!has)
            {
                for(int i = 0; i < fieldList.Count; i++)
                {
                    IFields f = fieldList[i];
                    if (f.HasField(para.GetName()))
                    {
                        return true;
                    }
                }
            }
            return has;
        }

        public override bool HasPara(string para)
        {
            bool has = this.paras.ContainsKey(para);
            if (!has)
            {
                for (int i = 0; i < fieldList.Count; i++)
                {
                    IFields f = fieldList[i];
                    if (f.HasField(para))
                    {
                        return true;
                    }
                }
            }
            return has;
        }

        public override bool HasFeature(string feature)
        {
            return HasPara(feature);
        }

        public override object GetFeatureValue(string feature)
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

        public override string[] GetFields()
        {
            ICollection<string> set = new HashSet<string>();
            Sharpen.Collections.AddAll(set, paras.Keys);
            foreach (IFields f in fieldList)
            {
                Sharpen.Collections.AddAll(set, Arrays.AsList(f.GetFields()));
            }
            return Sharpen.Collections.ToArray(set, new string[0]);
        }

        public override IPara Get(string name)
        {
            IPara para = null;
            paras.TryGetValue(name, out para);
            if (para == null)
            {
                for (int i = 0; i < fieldList.Count; i++)
                {
                    IFields f = fieldList[i];
                    
                    if (f.HasField(name))
                    {
                        para = f.Get(name);
                        if (para != null)
                        {
                            return para;
                        }
                    }
                }
            }
            return para;
        }
    }
}
