using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para.exp;
using com.wd.free.unit;
using com.wd.free.util;
using gameplay.gamerule.free.ui;

namespace com.wd.free.map.position
{
	[System.Serializable]
	public abstract class AbstractMapRegion : IMapRegion
	{
		private const long serialVersionUID = 4099877136835542244L;

		protected internal string con;

	    protected bool useOut;

		[System.NonSerialized]
		protected internal IParaCondition condition;

		public virtual bool In(IEventArgs args, UnitPosition entity)
		{
			if (!StringUtil.IsNullOrEmpty(con))
			{
				if (condition == null || (con.Contains(FreeUtil.VAR_START) && con.Contains(FreeUtil.VAR_END)))
				{
					condition = new ExpParaCondition(con);
				}
			}
			bool @in = IsIn(args, entity);
			if (@in)
			{
				return condition == null || condition.Meet(args);
			}
			return false;
		}

		public abstract bool IsIn(IEventArgs args, UnitPosition entity);

		public abstract UnitPosition GetCenter(IEventArgs arg1);

		public abstract bool InRectange(FreeUIUtil.Rectangle arg1, IEventArgs arg2);

		public abstract bool IsDynamic();
	}
}
