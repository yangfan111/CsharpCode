using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.Network
{
    public class Vector3Converter
    {
        public static App.Protobuf.Vector3 UnityToProtobufVector3(UnityEngine.Vector3 source)
        {
            App.Protobuf.Vector3 dest = App.Protobuf.Vector3.Allocate();
            dest.X = source.x;
            dest.Y = source.y;
            dest.Z = source.z;

            return dest;
        }

        public static App.Protobuf.Quaternion UnityToProtobufQuaternion(UnityEngine.Quaternion source)
        {
            App.Protobuf.Quaternion dest = App.Protobuf.Quaternion.Allocate();
            dest.X = source.x;
            dest.Y = source.y;
            dest.Z = source.z;
            dest.W = source.w;

            return dest;
        }

        public static UnityEngine.Vector3 ProtobufToUnityVector3(App.Protobuf.Vector3 source)
        {
            return new UnityEngine.Vector3(source.X, source.Y, source.Z);
        }

        public static UnityEngine.Quaternion ProtobufToUnityQuaternion(App.Protobuf.Quaternion source)
        {
            return new UnityEngine.Quaternion(source.X, source.Y, source.Z, source.W);
        }
    }
}
