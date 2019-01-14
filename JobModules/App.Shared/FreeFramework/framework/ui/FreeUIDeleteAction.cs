using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;

namespace gameplay.gamerule.free.ui
{
	[System.Serializable]
	public class FreeUIDeleteAction : SendMessageAction
	{
		private const long serialVersionUID = -1188091456904593121L;

		private string key;

		public virtual string GetKey()
		{
			return key;
		}

		public virtual void SetKey(string key)
		{
			this.key = key;
		}

		protected override void BuildMessage(IEventArgs args)
        {
		    builder.Key = 55;
            builder.Ss.Add(key);
        }

		public override string GetMessageDesc()
		{
			string d = string.Empty;
			if (!StringUtil.IsNullOrEmpty(desc))
			{
				d = "(" + desc + ")";
			}
			return "删除UI'" + key + d + "'";
		}
	}
}
