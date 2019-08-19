using System.Collections.Generic;
using System.IO;
using Core.Animation;
using Core.Utils;

namespace App.Shared.Components.Serializer
{

    public abstract class AbstractPatchSerializer
    {
        public abstract string GetType();
        public abstract bool CheckType(object t);
    }

    interface IPatchSerializer<T>where T :  IPatchClass<T>, new()
    {
        void Write(T cur, T last, MyBinaryWriter writer);
        void Read(T cur, BinaryReader reader);
        void Merge(T cur, T last);
    }
    
    public static partial class PatchPropertySerializer
    {

        private static Dictionary<string, AbstractPatchSerializer> _patchSerializers = new Dictionary<string, AbstractPatchSerializer>();

        public static void RegistSerialzier(AbstractPatchSerializer serializer)
        {
           
            if (!_patchSerializers.ContainsKey(serializer.GetType()))
                _patchSerializers.Add(serializer.GetType(), serializer);
        }
        
        public static void Write<T>(T cur, T last, MyBinaryWriter writer)where T :  IPatchClass<T>, new()
        {
            var serializer = _patchSerializers[cur.GetName()];
            if (serializer is IPatchSerializer<T>)
                (serializer as IPatchSerializer<T>).Write(cur, last, writer);
        }

        public static void Read<T>(T cur, BinaryReader reader)where T :  IPatchClass<T>, new()
        {
            var serializer = _patchSerializers[cur.GetName()];
            if (serializer is IPatchSerializer<T>)
                (serializer as IPatchSerializer<T>).Read(cur, reader);
        }

        public static void MergeFromPatch<T>(T cur, T last)where T :  IPatchClass<T>, new()
        {
            var serializer = _patchSerializers[cur.GetName()];
            if (serializer is IPatchSerializer<T>)
                (serializer as IPatchSerializer<T>).Merge(cur, last);
        }
    }
}