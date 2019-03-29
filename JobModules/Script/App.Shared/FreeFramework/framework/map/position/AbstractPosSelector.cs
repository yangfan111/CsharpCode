using Sharpen;
using com.wd.free.@event;
using com.wd.free.unit;

namespace com.wd.free.map.position
{
	[System.Serializable]
	public abstract class AbstractPosSelector : IPosSelector
	{
		private const long serialVersionUID = 2715840940587294356L;

		public abstract UnitPosition Select(IEventArgs args);

		public virtual UnitPosition[] Select(IEventArgs args, int count)
		{
			return new UnitPosition[] { Select(args) };
		}
	}
}
