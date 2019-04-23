using System;
using System.Collections;
using Core.ObjectPool;

namespace Core.SnapshotReplication.Serialization.Serializer
{
    public class BitArrayWrapper : BaseRefCounter
    {
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(BitArrayWrapper))
            {
            }

            public override object MakeObject()
            {
                return new BitArrayWrapper();
            }

            public override int InitPoolSize
            {
                get { return 2048; }
            }
        }

        public static BitArrayWrapper Allocate(int length, bool init = false)
        {
            if (length >= 256) throw new ArgumentOutOfRangeException();
            var ret = ObjectAllocatorHolder<BitArrayWrapper>.Allocate();
            ret._length = length;

            ret.SetAll(init);

            return ret;
        }


        private BitArray _bitArray = new BitArray(256);
        private int _length = 0;

        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        protected override void OnCleanUp()
        {
            _length = 0;
            ObjectAllocatorHolder<BitArrayWrapper>.Free(this);
        }

        public bool Get(int index)
        {
            if (index >= _length) throw new ArgumentOutOfRangeException();
            return _bitArray.Get(index);
        }

        public void Set(int index, bool value)
        {
            if (index >= _length) throw new ArgumentOutOfRangeException();
            _bitArray.Set(index, value);
        }

        public void SetAll(bool value)
        {
            _bitArray.SetAll(value);
        }

        public bool this[int index]
        {
            get
            {
                if (index >= _length) throw new ArgumentOutOfRangeException();
                return _bitArray[index];
            }
            set
            {
                if (index >= _length) throw new ArgumentOutOfRangeException();
                _bitArray[index] = value;
            }
        }

        public void CopyTo(Array array, int index)
        {
            _bitArray.CopyTo(array, index);
        }
    }
}