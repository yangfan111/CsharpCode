using System;
using Sharpen;

namespace com.wd.free.para
{
	[System.Serializable]
	public class XYZPara : AbstractPara
	{
		private const long serialVersionUID = -6806938851773973511L;

		public XYZPara()
		{
			this.value = new XYZPara.XYZ(this, 0, 0, 0);
		}

		public override IPoolable Copy()
		{
			com.wd.free.para.XYZPara para = new com.wd.free.para.XYZPara();
			para.name = name;
			para.value = value;
			para.desc = desc;
			return para;
		}

		public override object GetValue()
		{
			return (XYZPara.XYZ)value;
		}

        [Serializable]
		public class XYZ
		{
			private float x;

			private float y;

			private float z;

			public XYZ(XYZPara _enclosing, float x, float y, float z)
				: base()
			{
				this._enclosing = _enclosing;
				this.x = x;
				this.y = y;
				this.z = z;
			}

			public virtual float GetX()
			{
				return this.x;
			}

			public virtual void SetX(float x)
			{
				this.x = x;
			}

			public virtual float GetY()
			{
				return this.y;
			}

			public virtual void SetY(float y)
			{
				this.y = y;
			}

			public virtual float GetZ()
			{
				return this.z;
			}

			public virtual void SetZ(float z)
			{
				this.z = z;
			}

			public virtual float Distance(XYZPara.XYZ xyz)
			{
				float dx = this.x - xyz.x;
				float dy = this.y - xyz.y;
				float dz = this.z - xyz.z;
				return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
			}

			private readonly XYZPara _enclosing;
		}

		public override IPara Initial(string con, string v)
		{
			XYZPara para = (XYZPara)pool.BorrowObject();
			string[] ss = v.Split(",");
			if (ss.Length == 3)
			{
				para.value = new XYZPara.XYZ(this, float.Parse(ss[0].Trim()), float.Parse(ss[1].Trim()), float.Parse(ss[2].Trim()));
			}
			else
			{
				para.value = new XYZPara.XYZ(this, 0, 0, 0);
			}
			return para;
		}

		private static ParaPool<IPara> pool = new ParaPool<IPara>(new XYZPara());

		protected internal override ParaPool<IPara> GetPool()
		{
			return pool;
		}
	}
}
