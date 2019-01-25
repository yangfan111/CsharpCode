using Sharpen;
using com.wd.free.@event;
using Free.framework;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeRaderComponet : AbstractFreeComponent
	{
		private const long serialVersionUID = -7699020163820046853L;

		public override int GetKey(IEventArgs args)
		{
			return RADER;
		}

		public override string GetStyle(IEventArgs args)
		{
			return string.Empty;
		}

		public override IFreeUIValue GetValue()
		{
			return null;
		}

		public override SimpleProto CreateChildren(IEventArgs args)
		{
		    SimpleProto b = FreePool.Allocate();
		    b.Key = 1;

			return b;
		}
	}
}
