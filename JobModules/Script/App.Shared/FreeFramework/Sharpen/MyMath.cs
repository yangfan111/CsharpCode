using System;

namespace Sharpen
{
    public class MyMath 
    {
        static Random rnd = new Random();
        public const double E = Math.E;
        public const double PI = Math.PI;


        public static double Random()
        {
            return rnd.NextDouble();
        }


        public static double Cos(double rotationX)
        {
            return Math.Cos(rotationX);
        }

        public static double Sin(double rotationX)
        {
            return Math.Sin(rotationX);
        }

        public static double Round(double d)
        {
            return Math.Round(d);
        }

        public static double Log(double d)
        {
            return Math.Log(d);
        }

        public static double Sqrt(double p0)
        {
            return Math.Sqrt(p0);
        }

        public static double Atan2(double p0, double p1)
        {
            return Math.Atan2(p0, p1);
        }

        public static double Asin(double d)
        {
            return Math.Asin(d);
        }

        public static double Abs(double p0)
        {
            return Math.Abs(p0);
        }

        public static float Abs(float p0)
        {
            return Math.Abs(p0);
        }

        public static float Pow(float ratio, float frametimeRatio)
        {
            return (float)Math.Pow(ratio, frametimeRatio);
        }

        public static double Pow(double ratio, double frametimeRatio)
        {
            return Math.Pow(ratio, frametimeRatio);
        }

        public static double Tan(double d)
        {
            return Math.Tan(d);
        }

        public static double Max(double p0, double vX)
        {
            return Math.Max(p0, vX);
        }

        public static double Min(double p0, double vX)
        {
            return Math.Min(p0, vX);
        }

        public static double Floor(double f)
        {
            return Math.Floor(f);
        }

        public static double Atan(double d)
        {
            return Math.Atan(d);
        }

        public static double Ceil(double d)
        {
            return Math.Ceiling(d);
        }

        public static double Exp(double d)
        {
            return Math.Exp(d);
        }

        public static double Acos(double angle)
        {
            return Math.Acos(angle);
        }
    }
}