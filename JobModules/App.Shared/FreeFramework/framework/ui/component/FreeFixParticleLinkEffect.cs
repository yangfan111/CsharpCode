using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeFixParticleLinkEffect : IFreeEffect
	{
		private const long serialVersionUID = -6550441006104410613L;

		private string name;

		private string scale;

		private string adjust;

		private IPosSelector from;

		private IPosSelector to;

		private IList<IFreeUIAuto> autos;

		private const string SPLITER = "_$$$_";

		public override string GetStyle(IEventArgs args, string key)
		{
			UnitPosition fromUp = from.Select(args);
			UnitPosition toUp = to.Select(args);
			return fromUp.GetX() + "_" + fromUp.GetY() + "_" + fromUp.GetZ() + SPLITER + toUp.GetX() + "_" + toUp.GetY() + "_" + toUp.GetZ() + SPLITER + FreeUtil.ReplaceVar(name, args) + SPLITER + FreeUtil.ReplaceFloat(scale, args) + SPLITER + FreeUtil.
				ReplaceFloat(adjust, args);
		}

		public override int GetKey(IEventArgs args)
		{
			return 5;
		}

		public override string GetXyz(IEventArgs args, IPosSelector selector)
		{
			return string.Empty;
		}

		public override string GetScale(IEventArgs args)
		{
			return string.Empty;
		}

		public override string GetRotation(IEventArgs args, IPosSelector selector)
		{
			return string.Empty;
		}

		public virtual IList<IFreeUIAuto> GetAutos()
		{
			return autos;
		}

		public override IList<IFreeUIAuto> GetAutos(IEventArgs args)
		{
			return autos;
		}

		public virtual void SetAutos(IList<IFreeUIAuto> autos)
		{
			this.autos = autos;
		}
	}
}
