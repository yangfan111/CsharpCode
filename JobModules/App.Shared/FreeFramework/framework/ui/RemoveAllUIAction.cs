using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui
{
	[System.Serializable]
	public class RemoveAllUIAction : SendMessageAction
	{
		private const long serialVersionUID = 1550106371544011244L;

		private string recovery;

		protected  override void BuildMessage(IEventArgs args)
		{
			builder.Key = 56;
			builder.Bs.Add(FreeUtil.ReplaceBool(recovery, args));
		}

		public override string GetMessageDesc()
		{
			return "移除所有UI " + recovery;
		}
	}
}
