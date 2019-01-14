using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using gameplay.gamerule.free.ui.component;
using com.wd.free.map.position;

namespace gameplay.gamerule.free.component
{
	[System.Serializable]
	public class ComponentFreeEffect : IFreeEffect, IComponentable
	{
		private const long serialVersionUID = 3147927303245355665L;

		private string name;

		private string title;

		private IFreeEffect defaultEffect;

		public virtual string GetTitle()
		{
			return title;
		}

		public virtual void SetTitle(string title)
		{
			this.title = title;
		}

		public virtual IFreeEffect GetDefaultEffect()
		{
			return defaultEffect;
		}

		public virtual void SetDefaultEffect(IFreeEffect defaultEffect)
		{
			this.defaultEffect = defaultEffect;
		}

		public virtual void SetName(string name)
		{
			this.name = name;
		}

		public virtual string GetName()
		{
			return name;
		}

		private IFreeEffect GetCurrentEffect(IEventArgs args)
		{
			IFreeEffect fc = args.ComponentMap.GetEffect(name);
			if (fc != null)
			{
				return fc;
			}
			return defaultEffect;
		}

		public override int GetKey(IEventArgs args)
		{
			return GetCurrentEffect(args).GetKey(args);
		}

		public override string GetXyz(IEventArgs args, IPosSelector selector)
		{
			return GetCurrentEffect(args).GetXyz(args, selector);
		}

		public override string GetScale(IEventArgs args)
		{
			return GetCurrentEffect(args).GetScale(args);
		}

		public override string GetRotation(IEventArgs args, IPosSelector selector)
		{
			return GetCurrentEffect(args).GetRotation(args, selector);
		}

		public override string GetStyle(IEventArgs args, string key)
		{
			return GetCurrentEffect(args).GetStyle(args, key);
		}

		public override IList<IFreeUIAuto> GetAutos(IEventArgs args)
		{
			return GetCurrentEffect(args).GetAutos(args);
		}
	}
}
