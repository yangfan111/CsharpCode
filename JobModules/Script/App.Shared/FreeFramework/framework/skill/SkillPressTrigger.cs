using Sharpen;

namespace com.wd.free.skill
{
	[System.Serializable]
	public class SkillPressTrigger : ISkillTrigger
	{
		private const long serialVersionUID = 4633526227781233320L;

		private int key;

		[System.NonSerialized]
		private bool pressed;

		public override ISkillTrigger.TriggerStatus Triggered(ISkillArgs args)
		{
			if (args.GetInput().IsPressed(key))
			{
				if (!pressed)
				{
					pressed = true;
					return ISkillTrigger.TriggerStatus.Success;
				}
				else
				{
					return ISkillTrigger.TriggerStatus.Interrupted;
				}
			}
			else
			{
				if (pressed)
				{
					pressed = false;
					return ISkillTrigger.TriggerStatus.Failed;
				}
				else
				{
					return ISkillTrigger.TriggerStatus.Interrupted;
				}
			}
		}

		public virtual int GetKey()
		{
			return key;
		}

		public virtual void SetKey(int key)
		{
			this.key = key;
		}
	}
}
