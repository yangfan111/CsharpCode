using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using Free.framework;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeRotationImageComponet : AbstractFreeComponent
	{
		private const long serialVersionUID = -7699020163820046853L;

		private bool reverse;

		private string url;

		public override int GetKey(IEventArgs args)
		{
			return RIMAGE;
		}

		public override string GetStyle(IEventArgs args)
		{
			return FreeUtil.ReplaceVar(url, args) + "_$$$_" + reverse;
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
