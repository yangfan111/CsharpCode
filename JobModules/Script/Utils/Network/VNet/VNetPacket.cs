using Core.Network;
using Core.ObjectPool;
using Core.Utils;
using UnityEngine;

namespace VNet
{
    internal class VNetPacket : BaseRefCounter, IPacket
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(VNetPacket));
        private byte[] _data = new byte[100000];

        public static VNetPacket Allocate()
        {
            VNetPacket rc = ObjectAllocatorHolder<VNetPacket>.Allocate();

            return rc;
        }

        public int Length { get; set; }

        public int Capacity
        {
            get { return _data.Length; }
        }

        public byte[] Data
        {
            get { return _data; }
        }

        public void CopyTo(byte[] array, int arrayIndex, int count, int sourceIndex)
        {
            for (var i = arrayIndex; i < count; i++)
            {
                array[i] = _data[i];
            }
        }

        public void CopyFrom(byte[] array, int arrayIndex, int count)
        {
            if(count > _data.Length)
            {
                var newLength = 1;
                while(newLength < count)
                {
                    newLength <<= 1;
                }
                _data = new byte[newLength];
            }
            for (var i = 0; i < count; i++)
            {
                _data[i] = array[arrayIndex + i];
            }

            Length = count;
        }

        public void Dispose()
        {
            ReleaseReference();
        }

        protected override void OnCleanUp()
        {
            Length = 0;
            ObjectAllocatorHolder<VNetPacket>.Free(this);
        }
    }
}
