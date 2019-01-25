using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;
using Free.framework;

namespace gameplay.gamerule.free.ui
{
	[System.Serializable]
	public class FreeEffectDeleteAction : SendMessageAction
	{
		private const long serialVersionUID = -1188091456904593121L;

		private string key;

		public FreeEffectDeleteAction()
		{
		}

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void SetKey(string key)
		{
			this.key = key;
		}

		public override void DoAction(IEventArgs args)
		{
			this.scope = "4";
			base.DoAction(args);
		}

		public static void Build(SimpleProto builder, IEventArgs args, string key)
		{
			builder.Key = 65;
			builder.Ks.Add(1);
			builder.Ss.Add(key);
		}

	    protected override void BuildMessage(IEventArgs args)
	    {
	        Build(builder, args, FreeUtil.ReplaceVar(key, args));
        }

		public override string GetMessageDesc()
		{
			return "删除特效" + key;
		}
	}
}
