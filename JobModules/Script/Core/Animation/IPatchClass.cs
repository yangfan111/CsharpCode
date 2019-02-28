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
       
        
    }
}