using Sharpen;
using com.wd.free.@event;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoRotateValue : AbstractAutoValue
	{
		private const long serialVersionUID = 2613028647019236871L;

		private string angle;

		public override string ToConfig(IEventArgs args)
		{
			return "rotate|" + GetID(args) + "|" + angle;
		}

		public virtual string GetAngle()
		{
			return angle;
		}

		public virtual void SetAngle(string angle)
		{
			this.angle = angle;
		}
	}
}
