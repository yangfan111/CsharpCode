using Sharpen;
using com.wd.free.@event;
using com.wd.free.unit;
using com.wd.free.map.position;

namespace gameplay.gamerule.free.component
{
	[System.Serializable]
	public class ComponentPosSelector : IPosSelector, IComponentable
	{
		private const long serialVersionUID = 3147927303245355665L;

		private string name;

		private string title;

		private IPosSelector defaultPos;

		public virtual string GetTitle()
		{
			return title;
		}

		public virtual void SetTitle(string title)
		{
			this.title = title;
		}

		public virtual IPosSelector GetDefaultPos()
		{
			return defaultPos;
		}

		public virtual void SetDefaultPos(IPosSelector defaultPos)
		{
			this.defaultPos = defaultPos;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual string GetName()
		{
			return name;
		}

		public virtual UnitPosition Select(IEventArgs args)
		{
			IPosSelector pos = args.ComponentMap.GetPos(name);
			if (pos != null)
			{
				return pos.Select(args);
			}
			else
			{
				if (defaultPos != null)
				{
					return defaultPos.Select(args);
				}
			}
			return null;
		}

		public virtual UnitPosition[] Select(IEventArgs args, int count)
		{
			if (defaultPos != null)
			{
				return defaultPos.Select(args, count);
			}
			return null;
		}
	}
}
