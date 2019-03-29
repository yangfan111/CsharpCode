using Core.Animation;
using Core.Utils;
using System.IO;

namespace Core
{
    public class GrenadeCacheData : IPatchClass<GrenadeCacheData>
    {
        public int grenadeId;
        public int grenadeCount;

        public bool HasValue { get; set; }

        public GrenadeCacheData Clone()
        {
            GrenadeCacheData clone = new GrenadeCacheData();
            clone.grenadeId= grenadeId;
            clone.grenadeCount = grenadeCount;
            return clone;
        }

        public bool IsSimilar(GrenadeCacheData right)
        {

            if (right == null) return false;
            return (grenadeId ==right.grenadeId &&grenadeCount ==right.grenadeCount);
          
        }

        public void MergeFromPatch(GrenadeCacheData from)
        {
            grenadeId = from.grenadeId;
            grenadeCount = from.grenadeCount;
          
        }

        public void Read(BinaryReader reader)
        {
            grenadeId = reader.ReadInt32();
            grenadeCount = reader.ReadInt32();
        }

        public void RewindTo(GrenadeCacheData right)
        {
            MergeFromPatch(right);
        }

        public void Write(GrenadeCacheData last, MyBinaryWriter writer)
        {
            writer.Write(grenadeId);
            writer.Write(grenadeCount);
        }
      
    }
  



}