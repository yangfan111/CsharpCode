using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils.Utils
{
    public static class IEnumerableExt
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var v in collection)
            {
                action(v);
            }
        }
    }
}
