using System;

namespace Core.Utils
{
    public static class ArrayUtility
    {
        public static void SafeSet<T>(ref T[] array, int index, T elem, T initValue=default(T))
        {
            if (array.Length <= index)
            {
                int initLen = array.Length;
                Array.Resize(ref array, index+1);
                for (int i = initLen; i < array.Length; i++)
                {
                    array[i] = initValue;
                }
            }
            array[index] = elem;
        }

        public static T SafeGet<T>(T[] array, int index)
        {
            if (array.Length <= index)
            {
                return default(T);
            }
            return array[index];
        }
    }
}