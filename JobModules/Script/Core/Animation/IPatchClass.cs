using System.Collections;
using System.IO;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;

namespace Core.Animation
{
    public interface IPatchClass<T>
    {
        void RewindTo(T right);
        bool IsSimilar(T right);
        T Clone();
        bool HasValue { get; set; }
        T CreateInstance();

        string GetName();

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