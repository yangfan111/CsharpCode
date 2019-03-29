using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.util;
using gameplay.gamerule.free.ui.component;

namespace gameplay.gamerule.free.ui
{
	[System.Serializable]
	public class FreeUIShowAction : SendMessageAction
	{
		private const long serialVersionUID = -1188091456904593121L;

		public const string ALWAYS = "0";

		public const string HIDE = "-1";

		private string key;

		private string time;

		private IFreeUIShow show;

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void SetKey(string key)
		{
			this.key = key;
		}

		public virtual string GetTime()
		{
			return time;
		}

		public virtual void SetTime(string time)
		{
			this.time = time;
		}

		protected override void BuildMessage(IEventArgs args)
		{
			if (show == null)
			{
				show = new FreeUISimpleShow();
			}
			
		    builder.Key = 51;
		    builder.Ks.Add(0);
		    builder.Ss.Add(FreeUtil.ReplaceNumber(key, args));
		    builder.Ks.Add(show.GetKey());
		    builder.Ks.Add(FreeUtil.ReplaceInt(time, args));
			show.Handle(args, builder);
		}

		public override string GetMessageDesc()
		{
			string d = string.Empty;
			if (!StringUtil.IsNullOrEmpty(desc))
			{
				d = "(" + desc + ")";
			}
			return "展示UI'" + key + d + "'" + "'\n" + builder.ToString();
		}
	}
}
