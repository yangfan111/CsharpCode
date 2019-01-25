using System.Collections.Generic;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.map.position;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public abstract class IFreeEffect
	{
		public const string SPLITER = "_$$$_";

		public abstract int GetKey(IEventArgs args);

		public abstract string GetXyz(IEventArgs args, IPosSelector selector);

		public abstract string GetScale(IEventArgs args);

		public abstract string GetRotation(IEventArgs args, IPosSelector selector);

		public abstract string GetStyle(IEventArgs args, string key);

		public abstract IList<IFreeUIAuto> GetAutos(IEventArgs args);
	}

	public static class IFreeEffectConstants
	{
	}
}
