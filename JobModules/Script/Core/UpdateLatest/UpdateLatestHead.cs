using System.IO;
using Core.EntityComponent;
using Core.Utils;

namespace Core.UpdateLatest
{
    public class UpdateLatestHead
    {
        public int BaseUserCmdSeq;
        public int UserCmdSeq;
      
        public byte ComponentCount;
        public short BodyLength;
        public int LastSnapshotId;

        public UpdateLatestHead()
        {
            BaseUserCmdSeq = -1;
            UserCmdSeq = 0;
            
            BodyLength = 0;
            LastSnapshotId = -1;
        }


        public void Serialize(MyBinaryWriter stream)
        {
            stream.Write(BaseUserCmdSeq);
            stream.Write(UserCmdSeq);
            stream.Write(LastSnapshotId);
          
            stream.Write(ComponentCount);
            stream.Write((short) 0);
        }

        public void Deserialize(BinaryReader binaryReader)
        {
            BaseUserCmdSeq = binaryReader.ReadInt32();
            UserCmdSeq = binaryReader.ReadInt32();
            LastSnapshotId = binaryReader.ReadInt32();
          
            ComponentCount = binaryReader.ReadByte();
            BodyLength = binaryReader.ReadInt16();
        }

        public void ReWriteBodyLength(MyBinaryWriter stream, short bodyLenght)
        {
            BodyLength = bodyLenght;
            stream.Seek(-bodyLenght - 2, SeekOrigin.Current);
            stream.Write(bodyLenght);
            stream.Seek(bodyLenght, SeekOrigin.Current);
        }

        public void ReInit()
        {
            BaseUserCmdSeq = -1;
            UserCmdSeq = 0;
          
            BodyLength = 0;
            LastSnapshotId = -1;
        }


        bool Equals(UpdateLatestHead other)
        {
            return BaseUserCmdSeq == other.BaseUserCmdSeq && UserCmdSeq == other.UserCmdSeq &&
                   ComponentCount == other.ComponentCount &&
                   BodyLength == other.BodyLength && LastSnapshotId == other.LastSnapshotId;
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
                var hashCode = BaseUserCmdSeq;
                hashCode = (hashCode * 397) ^ UserCmdSeq;
                hashCode = (hashCode * 397) ^ ComponentCount.GetHashCode();
                hashCode = (hashCode * 397) ^ BodyLength.GetHashCode();
                hashCode = (hashCode * 397) ^ LastSnapshotId;
                return hashCode;
            }
        }
    }
}