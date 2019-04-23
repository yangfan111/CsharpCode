//#define Test
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Utils.Utils
{
#if Test
    public class CommonIntEnumEqualityComparer<T> : IEqualityComparer<T>  where T : struct
    {
        public bool Equals(T x, T y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        public static readonly CommonIntEnumEqualityComparer<T> Instance = new CommonIntEnumEqualityComparer<T>();
    }
#else
    public class CommonIntEnumEqualityComparer<T> : IEqualityComparer<T>  where T : struct
    {
        [StructLayout(LayoutKind.Explicit)]
        struct EnumUnion32
        {
            [FieldOffset(0)]
            public T Enum;
            [FieldOffset(0)]
            public int Int;
        }

        public static int Enum32ToInt(T e)
        {
            var u = default(EnumUnion32);
            u.Enum = e;
            return u.Int;
        }

        public bool Equals(T x, T y)
        {
            var a = Enum32ToInt(x);
            var b = Enum32ToInt(y);
            return a == b;
        }

        public int GetHashCode(T obj)
        {
            return Enum32ToInt(obj);
        }

        public static readonly CommonIntEnumEqualityComparer<T> Instance = new CommonIntEnumEqualityComparer<T>();
    }
#endif
}
