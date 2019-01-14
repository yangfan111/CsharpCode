using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public abstract class AbstractAutoValue : IFreeUIAuto
	{
		private const long serialVersionUID = -2398000914835179241L;

	    public const string SPLITER = "_$$$_";

        protected internal string field;

		protected internal bool all;

		protected internal string id;

		public virtual string GetField()
		{
			return field;
		}

		protected internal virtual int GetID(IEventArgs args)
		{
			int id = 0;
			if (!StringUtil.IsNullOrEmpty(this.id))
			{
				id = FreeUtil.ReplaceInt(this.id, args);
			}
			return id;
		}

		public virtual bool IsAll()
		{
			return all;
		}

		public virtual void SetAll(bool all)
		{
			this.all = all;
		}

		public virtual void SetField(string field)
		{
			this.field = field;
		}

		public virtual string GetId()
		{
			return id;
		}

		public virtual void SetId(string id)
		{
			this.id = id;
		}

		public abstract string ToConfig(IEventArgs arg1);
	}
}
