using System;
using Sharpen;
using com.cpkf.yyjd.tools.util.math;
using Core.Free;

namespace com.wd.free.para
{
	[System.Serializable]
	public class BoolPara : AbstractPara, IRule
    {
		private const long serialVersionUID = -8824870098367160681L;

		public BoolPara()
		{
			this.value = false;
		}

		public BoolPara(string name)
			: base(name)
		{
			this.value = false;
		}

		public BoolPara(string name, bool v)
			: base(name)
		{
			this.value = v;
		}

		public override IPara Initial(string con, string v)
		{
			com.wd.free.para.BoolPara p = (com.wd.free.para.BoolPara)this.Copy();
			p.name = EMPTY_NAME;
			try
			{
                p.value = ParseUtility.QuickBoolParse(v);
            }
			catch (Exception)
			{
				p.value = false;
			}
			return p;
		}

		public override IPoolable Copy()
		{
            BoolPara para = pool.Spawn(true) as BoolPara;
            para.SetName(this.name);
            para.SetValue((bool)this.value);
            para.SetTemp(true);
            return /*new com.wd.free.para.BoolPara(this.name, (bool)this.value)*/para;
		}

		internal static ParaPool pool = new ParaPool(new com.wd.free.para.BoolPara(), 100);

		protected internal override ParaPool GetPool()
		{
			return pool;
		}

        public override void Recycle()
        {
            base.Recycle();
            this.SetName(default(string));
            this.SetValue(default(bool));
            this.SetTemp(false);
            pool.Recycle(this);
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.BoolPara;
        }
    }
}
