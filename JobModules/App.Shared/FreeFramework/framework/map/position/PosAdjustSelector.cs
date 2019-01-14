using Sharpen;
using com.cpkf.yyjd.tools.util;
using com.wd.free.@event;
using com.wd.free.unit;
using com.wd.free.util;

namespace com.wd.free.map.position
{
	[System.Serializable]
	public class PosAdjustSelector : IPosSelector
	{
		private const long serialVersionUID = -7346159163343137078L;

		private string x;

		private string y;

		private string z;

		private string pitch;

		private string yaw;

		private IPosSelector pos;

		public virtual UnitPosition Select(IEventArgs args)
		{
			UnitPosition up = pos.Select(args);
			if (up != null)
			{
				Adjust(up, args);
			}
			return up;
		}

		private void Adjust(UnitPosition up, IEventArgs args)
		{
			up.SetX(up.GetX() + FreeUtil.ReplaceInt(x, args));
			up.SetY(up.GetY() + FreeUtil.ReplaceInt(y, args));
			up.SetZ(up.GetZ() + FreeUtil.ReplaceInt(z, args));
			if (!StringUtil.IsNullOrEmpty(pitch))
			{
				up.SetPitch(FreeUtil.ReplaceFloat(pitch, args));
			}
			if (!StringUtil.IsNullOrEmpty(yaw))
			{
				up.SetYaw(FreeUtil.ReplaceFloat(yaw, args));
			}
		}

		public virtual UnitPosition[] Select(IEventArgs args, int count)
		{
			UnitPosition[] ups = pos.Select(args, count);
			if (ups != null)
			{
				foreach (UnitPosition up in ups)
				{
					Adjust(up, args);
				}
			}
			return ups;
		}
	}
}
