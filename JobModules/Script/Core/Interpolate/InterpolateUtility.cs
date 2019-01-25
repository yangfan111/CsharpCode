using Core.Playback;
using UnityEngine;

namespace Core.Interpolate
{
    public class InterpolateUtility
    {
        public static int Interpolate(int lf, int rh, IInterpolationInfo interpolationInfo)
        {
            return (int) (lf + (rh - lf) * interpolationInfo.Ratio);
        }

        public double Interpolate(double lf, double rh, IInterpolationInfo interpolationInfo)
        {
            return lf + (rh - lf) * interpolationInfo.Ratio;
        }

        public static float Interpolate(float lf, float rh, IInterpolationInfo interpolationInfo)
        {
            return (lf + (rh - lf) * interpolationInfo.Ratio);
        }

        public static Vector3 Interpolate(Vector3 l, Vector3 r, IInterpolationInfo interpolationInfo)
        {
            var ratio = interpolationInfo.Ratio;
            return new Vector3(
                l.x + (r.x - l.x) * ratio,
                l.y + (r.y - l.y) * ratio,
                l.z + (r.z - l.z) * ratio);
            

        }

        public static Quaternion Interpolate(Quaternion l, Quaternion r, IInterpolationInfo interpolationInfo)
        {
	        return Quaternion.Slerp(l, r, interpolationInfo.Ratio);
        }

        public static int Interpolate(int lf, int rh, float ratio)
        {
            return (int) (lf + (rh - lf) * ratio);
        }

        public double Interpolate(double lf, double rh, float ratio)
        {
            return lf + (rh - lf) * ratio;
        }

        public static float Interpolate(float lf, float rh, float ratio)
        {
            return (lf + (rh - lf) * ratio);
        }

        public static Vector3 Interpolate(Vector3 l, Vector3 r, float ratio)
        {
            return new Vector3(
                l.x + (r.x - l.x) * ratio,
                l.y + (r.y - l.y) * ratio,
                l.z + (r.z - l.z) * ratio
            );

        }  public static Vector2 Interpolate(Vector2 l, Vector2 r, float ratio)
        {
            return new Vector2(
                l.x + (r.x - l.x) * ratio,
                l.y + (r.y - l.y) * ratio
            );

        }

        public static Quaternion Interpolate(Quaternion l, Quaternion r, float ratio)
        {
	        return Quaternion.Slerp(l, r, ratio);
        }
    }
}