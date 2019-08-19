using Core.Utils;
using UnityEngine;

namespace Core.Components
{
    public struct PrecisionsVector3
    {
        public float x;
        public float y;
        public float z;
        private string fstr;

        private string Fstr
        {
            get
            {
                if (string.IsNullOrEmpty(fstr))
                    fstr = string.Format("f{0}", precious);
                return fstr;
            }
        }

        public byte precious;
        public readonly static PrecisionsVector3 Zero = new PrecisionsVector3(0, 0, 0);
        public bool isRound;

        public PrecisionsVector3(float x, float y, float z, PrecisionsVector3 copyVal)
        {
            this.x   = x;
            this.y   = y;
            this.z   = z;
            precious = copyVal.precious;
            isRound  = copyVal.isRound;
            fstr     = copyVal.fstr;
            fstr = string.Empty;
        }

        public PrecisionsVector3(float x, float y, float z, byte precious = 2, bool isRound = false)
        {
            this.x        = isRound ? x.FloatRoundPrecision(precious) : x.FloatPrecision(precious);
            this.y        = isRound ? y.FloatRoundPrecision(precious) : y.FloatPrecision(precious);
            this.z        = isRound ? z.FloatRoundPrecision(precious) : z.FloatPrecision(precious);
            this.precious = precious;
            this.isRound  = isRound;
            fstr = string.Empty;
        }

        public static implicit operator Vector3(PrecisionsVector3 pv3)
        {
            return new Vector3(pv3.x, pv3.y, pv3.z);
        }

        public static implicit operator PrecisionsVector3(Vector3 pv3)
        {
            return new PrecisionsVector3(pv3.x, pv3.y, pv3.z);
        }

        public static PrecisionsVector3 MakePrecisionsV(Vector3 v3, byte precious)
        {
            return new PrecisionsVector3(v3.x, v3.y, v3.z, precious, false);
        }

        public static PrecisionsVector3 MakeRoundPrecisionsV(Vector3 v3, byte precious = 2)
        {
            return new PrecisionsVector3(v3.x, v3.y, v3.z, precious, true);
        }

        public PrecisionsVector3(Vector3 point, byte precious = 2) : this(point.x, point.y, point.z, precious)
        {
        }

        public PrecisionsVector3(float x, float y, float z, byte precious) : this(x, y, z, precious, false)
        {
        }

        public PrecisionsVector3(float x, float y, float z, bool isRound) : this(x, y, z, 2, isRound)
        {
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", x.ToString(Fstr), y.ToString(Fstr), z.ToString(Fstr));
        }

        public static PrecisionsVector3 operator +(PrecisionsVector3 a, PrecisionsVector3 b)
        {
            return new PrecisionsVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static PrecisionsVector3 operator -(PrecisionsVector3 a, PrecisionsVector3 b)
        {
            return new PrecisionsVector3(a.x - b.x, a.y - b.y, a.z - b.z, a);
        }

        public static PrecisionsVector3 operator -(PrecisionsVector3 a)
        {
            return new PrecisionsVector3(-a.x, -a.y, -a.z, a);
        }

        public static PrecisionsVector3 operator *(PrecisionsVector3 a, float d)
        {
            return new PrecisionsVector3(a.x * d, a.y * d, a.z * d, a.precious, a.isRound);
        }

        public static PrecisionsVector3 operator *(float d, PrecisionsVector3 a)
        {
            return new PrecisionsVector3(a.x * d, a.y * d, a.z * d, a.precious, a.isRound);
        }
    }
}