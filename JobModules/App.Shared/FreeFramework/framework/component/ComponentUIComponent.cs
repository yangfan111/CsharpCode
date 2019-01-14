using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using gameplay.gamerule.free.ui.component;
using Free.framework;

namespace gameplay.gamerule.free.component
{
	[System.Serializable]
	public class ComponentUIComponent : IFreeComponent, IComponentable
	{
		private const long serialVersionUID = 5611403387621130161L;

		private string name;

		private string title;

		private string desc;

		private IFreeComponent component;

		public virtual string GetName()
		{
			return name;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
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

		private IFreeComponent GetCurrentComponent(IEventArgs args)
		{
			IFreeComponent fc = args.ComponentMap.GetUI(name);
			if (fc != null)
			{
				return fc;
			}
			return component;
		}

		public virtual IFreeComponent GetComponent()
		{
			return component;
		}

		public virtual string GetWidth(IEventArgs args)
		{
			return GetCurrentComponent(args).GetWidth(args);
		}

		public virtual string GetHeight(IEventArgs args)
		{
			return GetCurrentComponent(args).GetHeight(args);
		}

		public virtual string GetX(IEventArgs args)
		{
			return GetCurrentComponent(args).GetX(args);
		}

		public virtual string GetY(IEventArgs args)
		{
			return GetCurrentComponent(args).GetY(args);
		}

		public virtual string GetRelative(IEventArgs args)
		{
			return GetCurrentComponent(args).GetRelative(args);
		}

		public virtual string GetParent(IEventArgs args)
		{
			return GetCurrentComponent(args).GetParent(args);
		}

		public virtual string GetEvent(IEventArgs args)
		{
			return GetCurrentComponent(args).GetEvent(args);
		}

		public virtual IList<IFreeUIAuto> GetAutos(IEventArgs args)
		{
			return GetCurrentComponent(args).GetAutos(args);
		}

		public virtual int GetKey(IEventArgs args)
		{
			return GetCurrentComponent(args).GetKey(args);
		}

		public virtual IFreeUIValue GetValue()
		{
			return component.GetValue();
		}

		public virtual string GetStyle(IEventArgs args)
		{
			return GetCurrentComponent(args).GetStyle(args);
		}

		public virtual SimpleProto CreateChildren(IEventArgs args)
		{
			return GetCurrentComponent(args).CreateChildren(args);
		}
	}
}
