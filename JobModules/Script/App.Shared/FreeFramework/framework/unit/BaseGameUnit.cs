using System;
using Sharpen;
using com.cpkf.yyjd.tools.data.sort;
using com.wd.free.para;
using com.wd.free.skill;

namespace com.wd.free.unit
{
    [Serializable]
	public class BaseGameUnit : IGameUnit, IFeaturable
	{
		protected string key;

        [NonSerialized]
		protected UnitSkill skill;

	    [NonSerialized]
        protected ParaList paras;

		protected long id;

	    public const string POSITION = "position_xyz";

		public BaseGameUnit()
		{
			this.paras = new ParaList();
			this.skill = new UnitSkill(this);
			XYZPara xyz = new XYZPara();
			xyz.SetName(POSITION);
			this.paras.AddPara(xyz);
		}

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void SetKey(string key)
		{
			this.key = key;
		}

		public virtual ParaList GetParameters()
		{
			return paras;
		}

		public virtual long GetID()
		{
			return id;
		}

		public virtual UnitSkill GetUnitSkill()
		{
			return skill;
		}

		public virtual XYZPara GetXYZ()
		{
			return (XYZPara)paras.Get(POSITION);
		}

		public virtual bool HasFeature(string feature)
		{
			return this.paras.HasFeature(feature);
		}

		public virtual object GetFeatureValue(string feature)
		{
			return paras.GetFeatureValue(feature);
		}
	}
}
