using Sharpen;
using com.wd.free.@event;
using com.wd.free.para.exp;

namespace gameplay.gamerule.free.component
{
	[System.Serializable]
	public class ComponentParaCondition : IParaCondition, IComponentable
	{
		private const long serialVersionUID = -3096668947524226601L;

		private string name;

		private string title;

		private string desc;

		private IParaCondition defaultCondition;

		public virtual bool Meet(IEventArgs args)
		{
			IParaCondition con = args.ComponentMap.GetCondition(name);
			if (con != null)
			{
				return con.Meet(args);
			}
			else
			{
				if (defaultCondition != null)
				{
					return defaultCondition.Meet(args);
				}
			}
			return false;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual string GetTitle()
		{
			return title;
		}

		public virtual void SetTitle(string title)
		{
			this.title = title;
		}

		public virtual string GetDesc()
		{
			return desc;
		}

		public virtual void SetDesc(string desc)
		{
			this.desc = desc;
		}

		public virtual IParaCondition GetDefaultCondition()
		{
			return defaultCondition;
		}
	}
}
