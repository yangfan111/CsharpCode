using System.Collections;
using System.IO;
using Core.Utils;

namespace Core.Animation
{
    public interface IPatchClass<T>
    {
        void RewindTo(T right);

        bool IsSimilar(T right);

       

        void Read(BinaryReader reader);

        void Write(T last, MyBinaryWriter writer);

        T Clone();

        void MergeFromPatch( T from);

        bool HasValue { get; set; }
        T CreateInstance();

    }
    
    public static class PatchClassAllocator<T> where T : class, IPatchClass<T>, new()
    {
        private static T _instance;

        static PatchClassAllocator()
        {
            _instance = new T();
        }

        public static T Create()
        {
            return _instance.CreateInstance();
        }
    }
}