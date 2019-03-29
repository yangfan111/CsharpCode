using Core.EntityComponent;
using Core.SnapshotReplication.Serialization.Serializer;

namespace Core
{



    /// <summary>
    /// 武器槽位数据结构
    /// </summary>
    public class WeaponBagSlotData
    {


        public EntityKey WeaponKey { get; private set; }
    
        /// <summary>
        /// 是否赋值
        /// </summary>
        public WeaponBagSlotData()
        {
            WeaponKey = EntityKey.Default;

        }
        public WeaponBagSlotData Clone()
        {
            WeaponBagSlotData clone = new WeaponBagSlotData();
            clone.WeaponKey = WeaponKey;
            return clone;
        }
        //public void Arm(int keyId)
        //{
        //    Sync(new EntityKey(keyId, GameGlobalConst.WeaponEntityType));
        //}
        public void Remove(EntityKey empty)
        {
            WeaponKey = empty ;
          
        }
        public void Sync(WeaponBagSlotData from)
        {
            Sync(from.WeaponKey);
        }
        public void Sync(EntityKey key)
        {
            WeaponKey = key;
        
        }
     //   public bool IsEmpty { get { return   WeaponKey == EntityKey.Default; } }

        public static bool operator ==(WeaponBagSlotData x, WeaponBagSlotData y)
        {
            return x.WeaponKey == y.WeaponKey;
        }
        public static bool operator !=(WeaponBagSlotData x, WeaponBagSlotData y)
        {
            return x.WeaponKey != y.WeaponKey;
        }

        public override string ToString()
        {
            return WeaponKey.ToString();
        }


    }








}