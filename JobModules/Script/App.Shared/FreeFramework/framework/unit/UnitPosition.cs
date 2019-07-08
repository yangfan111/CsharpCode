using System;
using UnityEngine;

namespace com.wd.free.unit
{
    [Serializable]
	public class UnitPosition
	{
	    private float x;
		private float y;
		private float z;

		private float yaw;
		private float pitch;

        private float cylinderVolR;
        private float cylinderVolH;

        private bool invalid;
        private int randomindex;


        public UnitPosition() : base()
		{
		}

		public UnitPosition(float x, float y, float z, float yaw, float pitch, float cylinderVolR, float cylinderVolH) : base()
		{
			this.x = x;
			this.y = y;
			this.z = z;
            this.yaw = yaw;
            this.pitch = pitch;
            this.cylinderVolR = cylinderVolR;
            this.cylinderVolH = cylinderVolH;
        }

		public virtual float Distance(UnitPosition another)
		{
			float dx = x - another.x;
			float dy = y - another.y;
			float dz = z - another.z;
			return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
		}

		public virtual float GetX()
		{
			return x;
		}
		public virtual void SetX(float x)
		{
			this.x = x;
		}

		public virtual float GetY()
		{
			return y;
		}
		public virtual void SetY(float y)
		{
			this.y = y;
		}

		public virtual float GetZ()
		{
			return z;
		}
		public virtual void SetZ(float z)
		{
			this.z = z;
		}

		public virtual float GetYaw()
		{
			return yaw;
		}
		public virtual void SetYaw(float yaw)
		{
			this.yaw = yaw;
		}

		public virtual float GetPitch()
		{
			return pitch;
		}
		public virtual void SetPitch(float pitch)
		{
			this.pitch = pitch;
		}

        public virtual float GetCylinderVolR()
        {
            return cylinderVolR;
        }
        public virtual void SetCylinderVolR(float cylinderVolR)
        {
            this.cylinderVolR = cylinderVolR;
        }

        public virtual float GetCylinderVolH()
        {
            return cylinderVolH;
        }
        public virtual void SetCylinderVolH(float cylinderVolH)
        {
            this.cylinderVolH = cylinderVolH;
        }

        public Vector3 Vector3
        {
            get { return new Vector3(x, y, z); }
        }

        public void SetInvalid(bool invalid)
        {
            this.invalid = invalid;
        }

        public bool GetInvalid()
        {
            return invalid;
        }

        public void SetRandomindex(int randomindex)
        {
            this.randomindex = randomindex;
        }

        public int GetRandomindex()
        {
            return randomindex;
        }

        protected bool Equals(UnitPosition other)
	    {
	        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && yaw.Equals(other.yaw) && pitch.Equals(other.pitch)
	               && cylinderVolR.Equals(other.cylinderVolR) && cylinderVolH.Equals(other.cylinderVolH);
	    }

	    public override bool Equals(object obj)
	    {
	        if (ReferenceEquals(null, obj)) return false;
	        if (ReferenceEquals(this, obj)) return true;
	        if (obj.GetType() != this.GetType()) return false;
	        return Equals((UnitPosition)obj);
	    }

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            var hashCode = x.GetHashCode();
	            hashCode = (hashCode * 397) ^ y.GetHashCode();
	            hashCode = (hashCode * 397) ^ z.GetHashCode();
	            hashCode = (hashCode * 397) ^ yaw.GetHashCode();
	            hashCode = (hashCode * 397) ^ pitch.GetHashCode();
                hashCode = (hashCode * 397) ^ cylinderVolR.GetHashCode();
                hashCode = (hashCode * 397) ^ cylinderVolH.GetHashCode();
	            return hashCode;
	        }
	    }

        public override string ToString()
		{
			return x + "," + y + "," + z + "," + yaw + "," + pitch + "," + cylinderVolR + "," + cylinderVolH;
		}
	}
}
