using Sharpen;
using com.cpkf.yyjd.tools.util;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class SkillKeyInterrupter : ISkillInterrupter
	{
		private const long serialVersionUID = -3224470478507787105L;

		private string press;

		private string release;

		[System.NonSerialized]
		private int[] ps;

		[System.NonSerialized]
		private int[] rs;

		public SkillKeyInterrupter()
			: base()
		{
		}

		public SkillKeyInterrupter(string press)
			: base()
		{
			this.press = press;
		}

		public SkillKeyInterrupter(string press, string release)
			: base()
		{
			this.press = press;
			this.release = release;
		}

		public virtual bool IsInterrupted(ISkillArgs args)
		{
			if (ps == null)
			{
				if (!StringUtil.IsNullOrEmpty(press))
				{
					string[] ss = StringUtil.Split(press, ",");
					ps = new int[ss.Length];
					for (int i = 0; i < ss.Length; i++)
					{
						ps[i] = int.Parse(ss[i].Trim());
					}
				}
				else
				{
					ps = new int[0];
				}
				if (!StringUtil.IsNullOrEmpty(release))
				{
					string[] ss = StringUtil.Split(release, ",");
					rs = new int[ss.Length];
					for (int i = 0; i < ss.Length; i++)
					{
						rs[i] = int.Parse(ss[i].Trim());
					}
				}
				else
				{
					rs = new int[0];
				}
			}
			foreach (int p in ps)
			{
				if (args.GetInput().IsPressed(p))
				{
					return true;
				}
			}
			foreach (int r in rs)
			{
				if (!args.GetInput().IsPressed(r))
				{
					return true;
				}
			}
			return false;
		}

		public virtual string GetPress()
		{
			return press;
		}

		public virtual void SetPress(string press)
		{
			this.press = press;
		}

		public virtual string GetRelease()
		{
			return release;
		}

		public virtual void SetRelease(string release)
		{
			this.release = release;
		}
	}
}
