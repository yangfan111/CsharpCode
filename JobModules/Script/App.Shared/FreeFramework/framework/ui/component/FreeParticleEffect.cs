using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeParticleEffect : IFreeEffect
	{
		private const long serialVersionUID = -6550441006104410613L;

		private string name;

		private string xyz;

		private string scale;

		private string rotation;

		private IList<IFreeUIAuto> autos;

		public override string GetStyle(IEventArgs args, string key)
		{
			return FreeUtil.ReplaceVar(name, args);
		}

		public override IList<IFreeUIAuto> GetAutos(IEventArgs args)
		{
			return autos;
		}

		public override int GetKey(IEventArgs args)
		{
			return 3;
		}

		public virtual void SetAutos(IList<IFreeUIAuto> autos)
		{
			this.autos = autos;
		}

		public virtual void SetXyz(string xyz)
		{
			this.xyz = xyz;
		}

		public virtual void SetScale(string scale)
		{
			this.scale = scale;
		}

		public virtual void SetRotation(string rotation)
		{
			this.rotation = rotation;
		}

		public override string GetXyz(IEventArgs args, IPosSelector selector)
		{
			return FreeUtil.ReplaceVar(xyz, args);
		}

		public override string GetScale(IEventArgs args)
		{
			return FreeUtil.ReplaceVar(scale, args);
		}

		public override string GetRotation(IEventArgs args, IPosSelector selector)
		{
			return FreeUtil.ReplaceVar(rotation, args);
		}
	}
}
