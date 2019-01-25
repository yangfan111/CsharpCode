using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.util;

namespace com.wd.free.action
{
	[System.Serializable]
	public class StringMultiAction : AbstractGameAction
	{
		private const long serialVersionUID = -5592323358066894668L;

		private string keys;

		private IGameAction action;

		public const string RECORD_SPLITER = "@@@";

		public const string FIELD_SPLITER = "***";

		public override void DoAction(IEventArgs args)
		{
			string[] ks = StringUtil.Split(FreeUtil.ReplaceVar(keys, args), new string[] { RECORD_SPLITER });
			for (int i = 0; i < ks.Length; i++)
			{
				string[] vs = StringUtil.Split(ks[i], new string[] { FIELD_SPLITER });
				HandleOne(ks.Length, i + 1, vs, args);
			}
		}

		private void HandleOne(int all, int index, string[] ks, IEventArgs args)
		{
			args.GetDefault().GetParameters().TempUse(new IntPara("index", index));
			args.GetDefault().GetParameters().TempUse(new IntPara("count", all));
			for (int i = 0; i < ks.Length; i++)
			{
				args.GetDefault().GetParameters().TempUse(new StringPara("r" + (i + 1), ks[i].Trim()));
			}
			if (action != null)
			{
				action.Act(args);
			}
			args.GetDefault().GetParameters().Resume("count");
			args.GetDefault().GetParameters().Resume("index");
			for (int i_1 = 0; i_1 < ks.Length; i_1++)
			{
				args.GetDefault().GetParameters().Resume("r" + (i_1 + 1));
			}
		}

		public virtual string GetKeys()
		{
			return keys;
		}

		public virtual void SetKeys(string keys)
		{
			this.keys = keys;
		}
	}
}
