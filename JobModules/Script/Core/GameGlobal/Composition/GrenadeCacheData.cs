using Core.Animation;
using Core.Utils;
using System.IO;
using System.Runtime.CompilerServices;
using Core.SnapshotReplication.Serialization.Serializer;

namespace Core
{
    public class GrenadeCacheData : IPatchClass<GrenadeCacheData>
    {
        public int grenadeId;
        public int grenadeCount;

        public bool HasValue { get; set; }
        public GrenadeCacheData CreateInstance()
        {
            return new GrenadeCacheData();
        }

        public string GetName()
        {
            return "GrenadeCacheData";
        }

        public BitArrayWrapper BitArray { get; set; }

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

        public void RewindTo(GrenadeCacheData right)
        {
            grenadeId = right.grenadeId;
            grenadeCount = right.grenadeCount;
        }
      
    }
  



}