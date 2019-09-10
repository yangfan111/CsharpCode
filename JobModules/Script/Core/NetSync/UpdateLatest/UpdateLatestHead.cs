using System.IO;
using Core.EntityComponent;
using Core.Utils;

namespace Core.UpdateLatest
{
    public class UpdateLatestHead
    {
        public int LastAckUserCmdSeq;
        public int LastUserCmdSeq;
      
        public byte ComponentCount;
        public byte SerializeCount;
        public short BodyLength;
        public int LastSnapshotId;

        public UpdateLatestHead()
        {
            LastAckUserCmdSeq = -1;
            LastUserCmdSeq = 0;
            SerializeCount = 0;
            BodyLength = 0;
            LastSnapshotId = -1;
        }


        public void Serialize(MyBinaryWriter stream)
        {
            stream.Write(LastAckUserCmdSeq);
            stream.Write(LastUserCmdSeq);
            stream.Write(LastSnapshotId);
          
            stream.Write(ComponentCount);
            stream.Write(SerializeCount);
            stream.Write((short) 0);
        }

        public void Deserialize(BinaryReader binaryReader)
        {
            LastAckUserCmdSeq = binaryReader.ReadInt32();
            LastUserCmdSeq = binaryReader.ReadInt32();
            LastSnapshotId = binaryReader.ReadInt32();
          
            ComponentCount = binaryReader.ReadByte();
            SerializeCount = binaryReader.ReadByte();
            BodyLength = binaryReader.ReadInt16();
        }
        public void ReWriteComponentCountAndBodyLength(MyBinaryWriter stream, short bodyLenght, byte count)
        {
            SerializeCount = count;
            BodyLength = bodyLenght;
            stream.Seek(-bodyLenght - 3, SeekOrigin.Current);
            stream.Write(count);
            stream.Write(bodyLenght);
            stream.Seek(bodyLenght, SeekOrigin.Current);
        }
       

        public void ReInit()
        {
            LastAckUserCmdSeq = -1;
            LastUserCmdSeq = 0;
            SerializeCount = 0;
            BodyLength = 0;
            LastSnapshotId = -1;
        }


        bool Equals(UpdateLatestHead other)
        {
            return LastAckUserCmdSeq == other.LastAckUserCmdSeq && LastUserCmdSeq == other.LastUserCmdSeq &&
                   ComponentCount == other.ComponentCount &&
                   BodyLength == other.BodyLength && LastSnapshotId == other.LastSnapshotId && SerializeCount == other.SerializeCount;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UpdateLatestHead) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = LastAckUserCmdSeq;
                hashCode = (hashCode * 397) ^ LastUserCmdSeq;
                hashCode = (hashCode * 397) ^ ComponentCount.GetHashCode();
                hashCode = (hashCode * 397) ^ BodyLength.GetHashCode();
                hashCode = (hashCode * 397) ^ LastSnapshotId;
                hashCode = (hashCode * 397) ^ SerializeCount.GetHashCode();
                return hashCode;
            }
        }

        
    }
}