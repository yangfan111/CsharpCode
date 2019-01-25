using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui
{
	[System.Serializable]
	public class FreeEffectShowAction : SendMessageAction
	{
		private const long serialVersionUID = -1188091456904593121L;

		private string key;

		private string time;

		private IPosSelector pos;

		public virtual IPosSelector GetPos()
		{
			return pos;
		}

		public virtual void SetPos(IPosSelector pos)
		{
			this.pos = pos;
		}

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

		public virtual void SetTime(int time)
		{
			this.time = time.ToString();
		}

		protected override void BuildMessage(IEventArgs args)
		{
		    builder.Key = 61;
			if (StringUtil.IsNullOrEmpty(key))
			{
			    builder.Ss.Add(string.Empty);
			}
			else
			{
			    builder.Ss.Add(FreeUtil.ReplaceVar(key, args));
			}
		    builder.Ks.Add(FreeUtil.ReplaceInt(time.ToString(), args));
			if (pos != null)
			{
				UnitPosition up = pos.Select(args);
				if (up != null)
				{
				    builder.Fs.Add(up.GetX());
				    builder.Fs.Add(up.GetY());
				    builder.Fs.Add(up.GetZ());
				}
			}
		}

		public override string GetMessageDesc()
		{
			return "展示特效" + key;
		}
	}
}
