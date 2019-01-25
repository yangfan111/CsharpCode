using Sharpen;
using com.wd.free.@event;
using com.wd.free.util;

namespace gameplay.gamerule.free.ui.component
{
	[System.Serializable]
	public class AutoPositionValue : AbstractAutoValue
	{
		private const long serialVersionUID = 2613028647019236871L;

		private string distance;

		private string height;

		private string pitch;

		private string angle;

		public override string ToConfig(IEventArgs args)
		{
			if (distance == null)
			{
				distance = "0";
			}
			if (height == null)
			{
				height = "0";
			}
			if (pitch == null)
			{
				pitch = "0";
			}
			if (angle == null)
			{
				angle = "0";
			}
			return "position|" + GetID(args) + "|" + FreeUtil.ReplaceNumber(distance, args) + "|" + FreeUtil.ReplaceNumber(height, args) + "|" + FreeUtil.ReplaceNumber(pitch, args) + "|" + FreeUtil.ReplaceNumber(angle, args);
		}

		public virtual string GetDistance()
		{
			return distance;
		}

		public virtual void SetDistance(string distance)
		{
			this.distance = distance;
		}

		public virtual string GetHeight()
		{
			return height;
		}

		public virtual void SetHeight(string height)
		{
			this.height = height;
		}

		public virtual string GetPitch()
		{
			return pitch;
		}

		public virtual void SetPitch(string pitch)
		{
			this.pitch = pitch;
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
