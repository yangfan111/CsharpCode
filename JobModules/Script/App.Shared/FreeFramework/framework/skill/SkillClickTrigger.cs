using Sharpen;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class SkillClickTrigger : AbstractSkillTrigger
	{
		private const long serialVersionUID = -1164578898529834201L;

		private int key;

		[System.NonSerialized]
		private bool release;

		public override ISkillTrigger.TriggerStatus Triggered(ISkillArgs args)
		{
			if (!args.GetInput().IsPressed(key))
			{
				release = true;
			}
			if (release)
			{
				if (args.GetInput().IsPressed(key))
				{
					release = false;
					return IsInter(args);
				}
			}
			return ISkillTrigger.TriggerStatus.Failed;
		}

		public virtual int GetKey()
		{
			return key;
		}

		public virtual void SetKey(int key)
		{
			this.key = key;
		}

        public override string ToString()
        {
            return string.Format("°´{0}½¨´¥·¢", key);
        }
    }
}
