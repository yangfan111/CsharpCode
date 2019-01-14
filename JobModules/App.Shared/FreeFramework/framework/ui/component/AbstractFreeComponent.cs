using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using Free.framework;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public abstract class AbstractFreeComponent : IFreeComponent
	{
		private const long serialVersionUID = 3319858323569148620L;

	    public const int IMAGE = 1;

	    public const int TEXT = 2;

	    public const int NUMBER = 3;

	    public const int LIST = 4;

	    public const int RADER = 5;

	    public const int EXP = 6;

	    public const int SMAP = 7;

	    public const int RIMAGE = 8;

	    public const int Prefab = 9;

        protected internal string desc;

		protected internal string width;

		protected internal string height;

		protected internal string x;

		protected internal string y;

		protected internal string relative;

		protected internal string parent;

		protected internal string @event;

		private IList<IFreeUIAuto> autos;

		public virtual string GetWidth(IEventArgs args)
		{
			return width;
		}

		public virtual void SetWidth(string width)
		{
			this.width = width;
		}

		public virtual string GetDesc()
		{
			return desc;
		}

		public virtual void SetDesc(string desc)
		{
			this.desc = desc;
		}

		public virtual string GetHeight(IEventArgs args)
		{
			return height;
		}

		public virtual void SetHeight(string height)
		{
			this.height = height;
		}

		public virtual string GetX(IEventArgs args)
		{
			return x;
		}

		public virtual void SetX(string x)
		{
			this.x = x;
		}

		public virtual string GetY(IEventArgs args)
		{
			return y;
		}

		public virtual void SetY(string y)
		{
			this.y = y;
		}

		public virtual string GetEvent(IEventArgs args)
		{
			return @event;
		}

		public virtual void SetEvent(string @event)
		{
			this.@event = @event;
		}

		public virtual string GetRelative(IEventArgs args)
		{
			return relative;
		}

		public virtual void SetRelative(string relative)
		{
			this.relative = relative;
		}

		public virtual string GetParent(IEventArgs args)
		{
			return parent;
		}

		public virtual void SetParent(string parent)
		{
			this.parent = parent;
		}

		public virtual IList<IFreeUIAuto> GetAutos(IEventArgs args)
		{
			return autos;
		}

		public virtual void SetAutos(IList<IFreeUIAuto> autos)
		{
			this.autos = autos;
		}

		public abstract SimpleProto CreateChildren(IEventArgs arg1);

		public abstract int GetKey(IEventArgs arg1);

		public abstract string GetStyle(IEventArgs arg1);

		public abstract IFreeUIValue GetValue();
	}
}
