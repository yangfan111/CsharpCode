using Sharpen;

namespace com.cpkf.yyjd.tools.data.sort
{
	public class MarkedData
	{
		private IFeaturable fe;

		private int index;

		public MarkedData(IFeaturable fe, int index)
			: base()
		{
			this.fe = fe;
			this.index = index;
		}

		public virtual IFeaturable GetFe()
		{
			return fe;
		}

		public virtual void SetFe(IFeaturable fe)
		{
			this.fe = fe;
		}

		public virtual int GetIndex()
		{
			return index;
		}

		public virtual void SetIndex(int index)
		{
			this.index = index;
		}

		public override string ToString()
		{
			return index + ":" + fe.ToString();
		}
	}
}
