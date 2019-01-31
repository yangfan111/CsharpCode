using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using App.Shared.Components.Bag;
using App.Shared.Components.Player;
using App.Shared.Components.Serializer.FieldSerializer;
using Core.Animation;
using Core.CameraControl;
using Core.CharacterState;
using Core.CharacterState.Posture;
using Core.EntityComponent;
using Core.Event;
using Core.SnapshotReplication.Serialization.Serializer;
using Core.Utils;
using UnityEngine;
using Utils.Utils.Buildin;


namespace App.Shared.Components.Serializer
{
    public class FieldSerializeUtil
    {
        private static LoggerAdapter _serializeLogger = new LoggerAdapter(typeof(FieldSerializeUtil));

        private static DoubleSerializer _doubleSerializer = new DoubleSerializer();
        private static IntSerializer _intSerializer = new IntSerializer();
        private static UintSerializer _uintSerializer = new UintSerializer();
        private static Uint16Serializer _uint16Serializer = new Uint16Serializer();
        private static ByteSerializer _byteSerializer = new ByteSerializer();
        private static ShortSerializer _shortSerializer = new ShortSerializer();
        private static LongSerializer _longSerializer = new LongSerializer();
        private static FloatSerializer _floatSerializer = new FloatSerializer();
        private static BoolSerializer _boolSerializer = new BoolSerializer();
        private static Vector2Serializer _vector2Serializer = new Vector2Serializer();
        private static Vector3Serializer _vector3Serializer = new Vector3Serializer();
        private static QuaternionSerializer _quaternionSerializer = new QuaternionSerializer();
        private static EntityKeySerializer _entityKeySerializer = new EntityKeySerializer();
        private static StringSerializer _stringSerializer = new StringSerializer();

        private static StateInterCommandsSerializer _stateInterCommandsSerializer = new StateInterCommandsSerializer();
        private static UnityAnimationEventCommandsSerializer _unityAnimationEventCommandsSerializer = new UnityAnimationEventCommandsSerializer();
        private static EventsSerializer _eventsSerializer = new EventsSerializer();

        private static void SendCompressedData(int sendTime, uint toSend, Core.Utils.MyBinaryWriter writer)
        {
            while (sendTime > 0)
            {
                _byteSerializer.Write((Byte) (toSend >> ((sendTime - 1) * 8)), writer);
                sendTime--;
            }
        }

        private static uint GetCompressData(int receiveTimes, BinaryReader reader)
        {
            uint result = 0;
            while (receiveTimes > 0)
            {
                result = result << 8;
                result |= _byteSerializer.Read(reader);
                receiveTimes--;
            }

            return result;
        }

        public static void Serialize(int data, Core.Utils.MyBinaryWriter writer, int last = default(int),
            bool weiteAll = false)
        {
            _intSerializer.Write(data, writer);
        }

        public static void Serialize(int data, float max, float min, float ratio, int sendTimes, bool DoCompress,
            Core.Utils.MyBinaryWriter writer, int last = default(int), bool weiteAll = false)
        {
            if (!DoCompress)
            {
                Serialize(data, writer, last, weiteAll);
                return;
            }

            if (data > max || data < min)
            {
                _serializeLogger.DebugFormat("Serialize fault : out of range!  data {0},max {1},min {2}", data, max,
                    min);
            }

            uint toSend = (uint) (data - min);
            SendCompressedData(sendTimes, toSend, writer);
        }

        public static void Serialize(uint data, Core.Utils.MyBinaryWriter writer, uint last = default(uint),
            bool weiteAll = false)
        {
            _uintSerializer.Write(data, writer);
        }

        public static void Serialize(uint data, float max, float min, float ratio, int sendTimes, bool DoCompress,
            Core.Utils.MyBinaryWriter writer, uint last = default(uint), bool weiteAll = false)
        {
            if (!DoCompress)
            {
                Serialize(data, writer, last, weiteAll);
                return;
            }

            if (data > max || data < min)
            {
                _serializeLogger.DebugFormat("Serialize fault : out of range!  data {0},max {1},min {2}", data, max,
                    min);
            }

            uint toSend = (uint) (data - min);
            SendCompressedData(sendTimes, toSend, writer);
        }

        public static void Serialize(byte data, Core.Utils.MyBinaryWriter writer, byte last = default(byte),
            bool weiteAll = false)
        {
            _byteSerializer.Write(data, writer);
        }

        public static void Serialize(short data, Core.Utils.MyBinaryWriter writer, short last = default(short),
            bool weiteAll = false)
        {
            _shortSerializer.Write(data, writer);
        }

        public static void Serialize(short data, float max, float min, float ratio, int sendTimes, bool DoCompress,
            Core.Utils.MyBinaryWriter writer, short last = default(short), bool weiteAll = false)
        {
            if (!DoCompress)
            {
                Serialize(data, writer, last, weiteAll);
                return;
            }

            if (data > max || data < min)
            {
                _serializeLogger.DebugFormat("Serialize fault : out of range!  data {0},max {1},min {2}", data, max,
                    min);
            }

            uint toSend = (uint) (data - min);
            SendCompressedData(sendTimes, toSend, writer);
        }

        public static void Serialize(UInt16 data, Core.Utils.MyBinaryWriter writer, ushort last = default(ushort),
            bool weiteAll = false)
        {
            _uint16Serializer.Write(data, writer);
        }

        public static void Serialize(UInt16 data, float max, float min, float ratio, int sendTimes, bool DoCompress,
            Core.Utils.MyBinaryWriter writer, UInt16 last = default(UInt16), bool weiteAll = false)
        {
            if (!DoCompress)
            {
                Serialize(data, writer, last, weiteAll);
                return;
            }

            if (data > max || data < min)
            {
                _serializeLogger.DebugFormat("Serialize fault : out of range!  data {0},max {1},min {2}", data, max,
                    min);
            }

            uint toSend = (uint) (data - min);
            SendCompressedData(sendTimes, toSend, writer);
        }

        public static void Serialize(long data, Core.Utils.MyBinaryWriter writer, long last = default(long),
            bool weiteAll = false)
        {
            _longSerializer.Write(data, writer);
        }

        public static void Serialize(long data, float max, float min, float ratio, int sendTimes, bool DoCompress,
            Core.Utils.MyBinaryWriter writer, long last = default(long), bool weiteAll = false)
        {
            if (!DoCompress)
            {
                Serialize(data, writer, last, weiteAll);
                return;
            }

            if (data > max || data < min)
            {
                _serializeLogger.DebugFormat("Serialize fault : out of range!  data {0},max {1},min {2}", data, max,
                    min);
            }

            uint toSend = (uint) ((data - min) * ratio);
            SendCompressedData(sendTimes, toSend, writer);
        }

        public static void Serialize(float data, Core.Utils.MyBinaryWriter writer, float last = default(float),
            bool weiteAll = false)
        {
            _floatSerializer.Write(data, writer);
        }

        public static void Serialize(float data, float max, float min, float ratio, int sendTimes, bool DoCompress,
            Core.Utils.MyBinaryWriter writer,
            float last = default(float), bool weiteAll = false)
        {
            if (!DoCompress)
            {
                Serialize(data, writer, last, weiteAll);
                return;
            }

            if (data > max || data < min)
            {
                _serializeLogger.DebugFormat("Serialize fault : out of range!  data {0},max {1},min {2}", data, max,
                    min);
            }

            uint toSend = (uint) ((data - min) * ratio);
            SendCompressedData(sendTimes, toSend, writer);
        }

        public static void Serialize(string data, Core.Utils.MyBinaryWriter writer, string last = default(string),
            bool weiteAll = false)
        {
            _stringSerializer.Write(data, writer);
        }

        public static void Serialize(double data, Core.Utils.MyBinaryWriter writer, double last = default(double),
            bool weiteAll = false)
        {
            _doubleSerializer.Write(data, writer);
        }

        public static void Serialize(double data, float max, float min, float ratio, int sendTimes, bool DoCompress,
            Core.Utils.MyBinaryWriter writer, double last = default(double), bool weiteAll = false)
        {
            if (!DoCompress)
            {
                Serialize(data, writer, last, weiteAll);
                return;
            }

            if (data > max || data < min)
            {
                _serializeLogger.DebugFormat("Serialize fault : out of range!  data {0},max {1},min {2}", data, max,
                    min);
            }

            uint toSend = (uint) ((data - min) * ratio);
            SendCompressedData(sendTimes, toSend, writer);
        }

        public static void Serialize(bool data, Core.Utils.MyBinaryWriter writer, bool last = default(bool),
            bool weiteAll = false)
        {
            _boolSerializer.Write(data, writer);
        }

        public static void Serialize(Vector2 data, Core.Utils.MyBinaryWriter writer, Vector2 last = default(Vector2),
            bool weiteAll = false)
        {
            _vector2Serializer.Write(data, writer);
        }

        public static void Serialize(Vector2 data, float max, float min, float ratio, int sendTimes, bool DoCompress,
            Core.Utils.MyBinaryWriter writer,
            Vector2 last = default(Vector2), bool weiteAll = false)
        {
            if (!DoCompress)
            {
                Serialize(data, writer, last, weiteAll);
                return;
            }

            Serialize(data.x, max, min, ratio, sendTimes, DoCompress, writer, last.x, weiteAll);
            Serialize(data.y, max, min, ratio, sendTimes, DoCompress, writer, last.y, weiteAll);
        }

        public static void Serialize(Vector3 data, Core.Utils.MyBinaryWriter writer, Vector3 last = default(Vector3),
            bool weiteAll = false)
        {
            _vector3Serializer.Write(data, writer);
        }

        public static void Serialize(Vector3 data, float max, float min, float ratio, int sendTimes, bool DoCompress,
            Core.Utils.MyBinaryWriter writer, Vector3 last = default(Vector3), bool weiteAll = false)
        {
            if (!DoCompress)
            {
                Serialize(data, writer, last, weiteAll);
                return;
            }

            Serialize(data.x, max, min, ratio, sendTimes, DoCompress, writer, last.x, weiteAll);
            Serialize(data.y, max, min, ratio, sendTimes, DoCompress, writer, last.y, weiteAll);
            Serialize(data.z, max, min, ratio, sendTimes, DoCompress, writer, last.z, weiteAll);
        }

        public static void Serialize(Quaternion data, Core.Utils.MyBinaryWriter writer,
            Quaternion last = default(Quaternion), bool weiteAll = false)
        {
            _quaternionSerializer.Write(data, writer);
        }

        public static void Serialize(Quaternion data, float max, float min, float ratio, int sendTimes, bool DoCompress,
            Core.Utils.MyBinaryWriter writer, Quaternion last = default(Quaternion), bool weiteAll = false)
        {
            if (!DoCompress)
            {
                Serialize(data, writer, last, weiteAll);
                return;
            }

            float[] list = {data.x, data.y, data.z, data.w};
            float[] lastlist = {last.x, last.y, last.z, last.w};
            float biggest = data.x;
            int Site = 0;
            for (int i = 1; i < list.Length; i++)
            {
                if (Mathf.Abs(list[i]) > Mathf.Abs(biggest))
                {
                    biggest = list[i];
                    Site = i;
                }
            }

            float sign = list[Site] > 0 ? 1 : -1;
            _byteSerializer.Write((Byte) Site, writer);
            for (int i = 0; i < list.Length; i++)
            {
                if (i != Site)
                    Serialize(list[i] * sign, max, min, ratio, sendTimes, DoCompress, writer, lastlist[i] * sign,
                        weiteAll);
            }
        }

        public static void Serialize(StateInterCommands data, Core.Utils.MyBinaryWriter writer,
            StateInterCommands last = default(StateInterCommands), bool weiteAll = false)
        {
            _stateInterCommandsSerializer.Write(data, writer);
        }
        
        public static void Serialize(UnityAnimationEventCommands data, Core.Utils.MyBinaryWriter writer,
            UnityAnimationEventCommands last = default(UnityAnimationEventCommands), bool weiteAll = false)
        {
            _unityAnimationEventCommandsSerializer.Write(data, writer);
        }

        public static void Serialize(EntityKey data, Core.Utils.MyBinaryWriter writer,
            EntityKey last = default(EntityKey), bool weiteAll = false)
        {
            _entityKeySerializer.Write(data, writer);
        }

        public static void Serialize(PlayerEvents data, Core.Utils.MyBinaryWriter writer, PlayerEvents last = null,
            bool weiteAll = false)
        {
            _eventsSerializer.Write(data, writer);
        }

        private static BitArrayWrapper GetDiffBitArray<T>(List<T> last, List<T> data, bool writeAll)
            where T : IPatchClass<T>, new()
        {
            var count = data == null ? 0 : data.Count;
            var lastCount = last == null ? 0 : last.Count;
            if (writeAll) return BitArrayWrapper.Allocate(count, true);
            BitArrayWrapper ret = BitArrayWrapper.Allocate(count, false);

            for (int i = 0; i < count; i++)
            {
                ret[i] = i >= lastCount || !data[i].NeedPatch(
                             last[i]);
            }

            return ret;
        }

        public static void Serialize(List<int> list, Core.Utils.MyBinaryWriter writer, List<int> lastList = null,
            bool writeAll = false)
        {
            if (list == null || list.Count == 0)
            {
                writer.Write((short) 0);
            }
            else
            {
                writer.Write((short) list.Count);
                foreach (var i in list)
                {
                    writer.Write(i);
                }
            }
        }

        public static void Serialize<T>(List<T> list, Core.Utils.MyBinaryWriter writer, List<T> lastList = null,
            bool writeAll = false) where T : IPatchClass<T>, new()
        {
            BitArrayWrapper bitArray = GetDiffBitArray(lastList, list, writeAll);
            var count = list == null ? 0 : list.Count;
            var lastCount = lastList == null ? 0 : lastList.Count;
            writer.Write(bitArray);

            for (int i = 0; i < count; i++)
            {
                if (bitArray[i] == false) continue;
                var last = i >= lastCount || writeAll ? default(T) : lastList[i];
                list[i].Write(last, writer);
            }

            bitArray.ReleaseReference();
        }

        public static List<T> Resize<T>(List<T> list, int length) where T : class, IPatchClass<T>, new()
        {
            if (list == null)
            {
                list = new List<T>(length);
            }

            if (list.Count > length)
            {
                list.RemoveRange(length, list.Count - length);
            }
            else
            {
                var add = length - list.Count;
                for (int i = 0; i < add; i++)
                    list.Add(new T());
            }

            for (int i = 0; i < length; i++)
            {
                if (list[i] == null) list[i] = new T();
            }

            return list;
        }

        public static List<int> Deserialize(List<int> list, BinaryReader reader)
        {
            int count = reader.ReadInt16();

            list.Clear();
            for (int i = 0; i < count; i++)
            {
                list.Add(reader.ReadInt32());
            }

            return list;
        }

        public static List<T> Deserialize<T>(List<T> list, BinaryReader reader) where T : class, IPatchClass<T>, new()
        {
            BitArrayWrapper bitArray = reader.ReadBitArray();
            int length = bitArray.Length;
            list = Resize(list, length);

            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray[i])
                {
                    Deserialize(list[i], reader);
                    list[i].HasValue = true;
                }
                else
                {
                    list[i].HasValue = false;
                }
            }

            bitArray.ReleaseReference();
            return list;
        }

        private static T CloneObject<T>(T p) where T : class, IPatchClass<T>, new()
        {
            return p.Clone();
        }

        private static T Deserialize<T>(T last, BinaryReader reader) where T : class, IPatchClass<T>, new()
        {
            last.Read(reader);
            return last;
        }

        public static int Deserialize(int typeTag, BinaryReader reader)
        {
            return _intSerializer.Read(reader);
        }

        public static int Deserialize(int typeTag, float max, float min, float ratio, int receiveTimes, bool DoCompress,
            BinaryReader reader)
        {
            if (!DoCompress) return Deserialize(typeTag, reader);
            uint result = GetCompressData(receiveTimes, reader);
            return (int) (result + min);
        }

        public static uint Deserialize(uint typeTag, BinaryReader reader)
        {
            return _uintSerializer.Read(reader);
        }

        public static uint Deserialize(uint typeTag, float max, float min, float ratio, int receiveTimes,
            bool DoCompress, BinaryReader reader)
        {
            if (!DoCompress) return Deserialize(typeTag, reader);
            uint result = GetCompressData(receiveTimes, reader);
            return (uint) (result + min);
        }

        public static byte Deserialize(byte typeTag, BinaryReader reader)
        {
            return _byteSerializer.Read(reader);
        }

        public static short Deserialize(short typeTag, BinaryReader reader)
        {
            return _shortSerializer.Read(reader);
        }

        public static short Deserialize(short typeTag, float max, float min, float ratio, int receiveTimes,
            bool DoCompress, BinaryReader reader)
        {
            if (!DoCompress) return Deserialize(typeTag, reader);
            uint result = GetCompressData(receiveTimes, reader);
            return (short) (result + min);
        }

        public static long Deserialize(long typeTag, BinaryReader reader)
        {
            return _longSerializer.Read(reader);
        }

        public static long Deserialize(long typeTag, float max, float min, float ratio, int receiveTimes,
            bool DoCompress, BinaryReader reader)
        {
            if (!DoCompress) return Deserialize(typeTag, reader);
            uint result = GetCompressData(receiveTimes, reader);
            return (long) (result / ratio + min);
        }

        public static string Deserialize(string typeTag, BinaryReader reader)
        {
            return _stringSerializer.Read(reader);
        }

        public static float Deserialize(float typeTag, BinaryReader reader)
        {
            return _floatSerializer.Read(reader);
        }

        public static float Deserialize(float typeTag, float max, float min, float ratio, int receiveTimes,
            bool DoCompress, BinaryReader reader)
        {
            if (!DoCompress) return Deserialize(typeTag, reader);
            uint result = (uint) GetCompressData(receiveTimes, reader);
            return (float) (result / ratio + min);
        }

        public static double Deserialize(double typeTag, BinaryReader reader)
        {
            return _doubleSerializer.Read(reader);
        }

        public static double Deserialize(double typeTag, float max, float min, float ratio, int receiveTimes,
            bool DoCompress, BinaryReader reader)
        {
            if (!DoCompress) return Deserialize(typeTag, reader);
            uint result = GetCompressData(receiveTimes, reader);
            return (double) (result / ratio + min);
        }

        public static bool Deserialize(bool typeTag, BinaryReader reader)
        {
            return _boolSerializer.Read(reader);
        }

        public static Vector2 Deserialize(Vector2 typeTag, BinaryReader reader)
        {
            return _vector2Serializer.Read(reader);
        }

        public static Vector2 Deserialize(Vector2 typeTag, float max, float min, float ratio, int receiveTimes,
            bool DoCompress, BinaryReader reader)
        {
            if (!DoCompress) return Deserialize(typeTag, reader);
            float x = Deserialize(typeTag.x, max, min, ratio, receiveTimes, DoCompress, reader);
            float y = Deserialize(typeTag.x, max, min, ratio, receiveTimes, DoCompress, reader);
            return new Vector2(x, y);
        }

        public static Vector3 Deserialize(Vector3 typeTag, BinaryReader reader)
        {
            return _vector3Serializer.Read(reader);
        }

        public static Vector3 Deserialize(Vector3 typeTag, float max, float min, float ratio, int receiveTimes,
            bool DoCompress, BinaryReader reader)
        {
            if (!DoCompress) return Deserialize(typeTag, reader);
            float x = Deserialize(typeTag.x, max, min, ratio, receiveTimes, DoCompress, reader);
            float y = Deserialize(typeTag.y, max, min, ratio, receiveTimes, DoCompress, reader);
            float z = Deserialize(typeTag.z, max, min, ratio, receiveTimes, DoCompress, reader);
            return new Vector3(x, y, z);
        }

        public static Quaternion Deserialize(Quaternion typeTag, BinaryReader reader)
        {
            return _quaternionSerializer.Read(reader);
        }

        public static Quaternion Deserialize(Quaternion typeTag, float max, float min, float ratio, int receiveTimes,
            bool DoCompress,
            BinaryReader reader)
        {
            if (!DoCompress) return Deserialize(typeTag, reader);
            float[] list = {0f, 0f, 0f, 0f};
            int Site = _byteSerializer.Read(reader);
            list[Site] = 1f;
            for (int i = 0; i < list.Length; i++)
            {
                if (i != Site)
                {
                    list[i] = Deserialize(typeTag.x, max, min, ratio, receiveTimes, DoCompress, reader);
                    list[Site] -= list[i] * list[i];
                }
            }

            list[Site] = Mathf.Sqrt(list[Site]);
            return new Quaternion(list[0], list[1], list[2], list[3]);
        }

        public static StateInterCommands Deserialize(StateInterCommands typeTag, BinaryReader reader)
        {
            return _stateInterCommandsSerializer.Read(reader);
        }
        
        public static UnityAnimationEventCommands Deserialize(UnityAnimationEventCommands typeTag, BinaryReader reader)
        {
            return _unityAnimationEventCommandsSerializer.Read(reader);
        }

        public static EntityKey Deserialize(EntityKey typeTag, BinaryReader reader)
        {
            return _entityKeySerializer.Read(reader);
        }

        public static PlayerEvents Deserialize(PlayerEvents typeTag, BinaryReader reader)
        {
            return _eventsSerializer.Read(reader, typeTag);
        }
    }
}