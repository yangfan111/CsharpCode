using Sharpen;
using com.wd.free.@event;
using com.wd.free.unit;
using com.wd.free.util;
using Core.Free;

namespace com.wd.free.map.position
{
	[System.Serializable]
	public class PosAssignSelector : AbstractPosSelector, IRule
    {
		private const long serialVersionUID = 310862848114343769L;

		private string x;

		private string y;

		private string z;

		private string yaw;

		private string pitch;

        private string invalid;

        private string randomindex;


        public PosAssignSelector()
			: base()
		{
		}

		public PosAssignSelector(string x, string y, string z)
			: base()
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public PosAssignSelector(string x, string y, string z, string yaw, string pitch)
			: base()
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.yaw = yaw;
			this.pitch = pitch;
		}

		public virtual string GetYaw()
		{
			return yaw;
		}

		public virtual void SetYaw(string yaw)
		{
			this.yaw = yaw;
		}

		public virtual string GetPitch()
		{
			return pitch;
		}

		public virtual void SetPitch(string pitch)
		{
			this.pitch = pitch;
		}

		public virtual string GetX()
		{
			return x;
		}

		public virtual void SetX(string x)
		{
			this.x = x;
		}

		public virtual string GetY()
		{
			return y;
		}

		public virtual void SetY(string y)
		{
			this.y = y;
		}

		public virtual string GetZ()
		{
			return z;
		}

		public virtual void SetZ(string z)
		{
			this.z = z;
		}

        public virtual void SetInvalid(string invalid)
        {
            this.invalid = invalid;
        }

        public virtual void SetRandomindex(string randomindex)
        {
            this.randomindex = randomindex;
        }

        public override UnitPosition Select(IEventArgs args)
		{
			UnitPosition up = new UnitPosition();
			up.SetX(FreeUtil.ReplaceFloat(x, args));
			up.SetY(FreeUtil.ReplaceFloat(y, args));
			up.SetZ(FreeUtil.ReplaceFloat(z, args));
			up.SetYaw(FreeUtil.ReplaceFloat(yaw, args));
			up.SetPitch(FreeUtil.ReplaceFloat(pitch, args));
            up.SetInvalid(FreeUtil.ReplaceBool(invalid, args));
            up.SetRandomindex(FreeUtil.ReplaceInt(randomindex, args));
            return up;
		}

		public override string ToString()
		{
			return x + "," + y + "," + z;
		}

        public int GetRuleID()
        {
            return (int)ERuleIds.PosAssignSelector;
        }
    }
}
