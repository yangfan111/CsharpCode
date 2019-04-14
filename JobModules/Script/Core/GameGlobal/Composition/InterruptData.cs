namespace Core
{
    public struct InterruptData
    {
        //einterruptEffectType
        public byte cmdType;

        public bool hasValue;

        //einterruptstate
        public byte state;

        public void Reset()
        {
            state = (byte) EInterruptState.WaitInterrupt;
            hasValue = false;
        }

        public void CopyFrom(InterruptData remote)
        {
            cmdType  = remote.cmdType;
            hasValue = remote.hasValue;
            state    = remote.state;
        }

        //public void RewindTo(InterruptData right)
        //{
        //    CopyFrom(right);
        //}

        //public bool IsSimilar(InterruptData right)
        //{
        //    return (cmdType == right.cmdType || hasValue == right.hasValue);
        //}

        //public void Read(BinaryReader reader)
        //{
        //    cmdType= reader.ReadByte();
        //    hasValue = reader.ReadBoolean();
        //}

        //public void Write(InterruptData last, MyBinaryWriter writer)
        //{
        //    writer.Write(cmdType);
        //    writer.Write(hasValue);
        //}

        //public InterruptData Clone()
        //{
        //    InterruptData clone = new InterruptData();
        //    clone.cmdType = cmdType;
        //    clone.hasValue = hasValue;
        //    return clone;
        //}

        //public void MergeFromPatch(InterruptData from)
        //{
        //    CopyFrom(from);
        //}

        //public bool HasValue { get; set; }
        //public void Dispose()
        //{
        //}
    }
}