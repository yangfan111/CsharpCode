using System.IO;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    interface IFieldSerializer<T>
    {
        void Write(T data,Core.Utils.MyBinaryWriter writer);
        T Read(BinaryReader reader);
    }
    
    interface IFieldDeltaSerializer<T>
    {
        void Write(T last, T data,Core.Utils.MyBinaryWriter writer);
        T Read(T last, BinaryReader reader);
    }
}
