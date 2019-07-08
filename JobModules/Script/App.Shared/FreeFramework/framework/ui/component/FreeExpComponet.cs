using Sharpen;
using com.wd.free.@event;
using Free.framework;
using Core.Free;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class FreeExpComponet : AbstractFreeComponent, IRule
    {
		private const long serialVersionUID = -7699020163820046853L;

		public override int GetKey(IEventArgs args)
		{
			return EXP;
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
		    SimpleProto proto = FreePool.Allocate();
		    proto.Key = 1;

			return proto;
		}

        public int GetRuleID()
        {
            return (int)ERuleIds.FreeExpComponet;
        }
    }
}
