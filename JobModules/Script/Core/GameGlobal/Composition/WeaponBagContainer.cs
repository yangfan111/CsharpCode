using Core.Animation;
using Core.Utils;
using System;
using System.IO;
using Core.SnapshotReplication.Serialization.Serializer;
using Utils.Utils.Buildin;
using Core.EntityComponent;
using System.Collections.Generic;
using System.Text;

namespace Core
{

    public class WeaponBagContainer : IPatchClass<WeaponBagContainer>, IDisposable
    {
        public WeaponBagContainer() : this(true)
        {
        }
        public WeaponBagContainer(bool initialize)
        {
            for (int i = 0; i < GlobalConst.WeaponSlotMaxLength; i++)
            {
                slotWeapons.Add(new WeaponBagSlotData());
            }

        }
        public int ToIndex(EWeaponSlotType slot)
        {
            switch (slot)
            {
                case EWeaponSlotType.Pointer:
                    return HeldSlotPointer;
                default:
                    return (int)slot;

            }
        }
        private List<WeaponBagSlotData> slotWeapons = new List<WeaponBagSlotData>(GlobalConst.WeaponSlotMaxLength);

        public int HeldSlotPointer { get; private set; }

        public int LastSlotPointer { get; private set; }

        /// <summary>
        /// 背包实际索引号
        /// </summary>
        public int BagIndex { get; private set; }
        public void ChangeSlotPointer(int nowSlot)
        {

            LastSlotPointer = HeldSlotPointer;
            HeldSlotPointer = nowSlot;

        }
        public void ClearPointer()
        {
            LastSlotPointer = 0;
            HeldSlotPointer = 0;
        }
        public WeaponBagSlotData HeldSlotData { get { return slotWeapons[HeldSlotPointer]; } }

        public WeaponBagSlotData LastSlotData { get { return slotWeapons[LastSlotPointer]; } }

        public WeaponBagSlotData this[EWeaponSlotType slot]
        {
            get
            {
                int index = ToIndex(slot);
                AssertUtility.Assert(index < slotWeapons.Count);
                return slotWeapons[index];
            }
        }
        public bool HasValue { get; set; }



        public WeaponBagContainer Clone()
        {

            WeaponBagContainer clone = new WeaponBagContainer(false);
            clone.HeldSlotPointer = HeldSlotPointer;
            clone.LastSlotPointer = LastSlotPointer;
            for (int i = 0; i < GlobalConst.WeaponSlotMaxLength; i++)
            {
                clone.slotWeapons[i] = slotWeapons[i].Clone();
            }
            return clone;
        }
        ~WeaponBagContainer()
        {
            Dispose();
        }
        //public void Trash()
        //{
        //    HeldSlotPointer = 0;
        //    LastSlotPointer = 0;
        //    for (int i = 0; i < GameGlobalConst.WeaponSlotMaxLength; i++)
        //    {
        //        slotWeapons[i].Remove();
        //    }
        //}
        public void MergeFromPatch(WeaponBagContainer from)
        {

            HeldSlotPointer = from.HeldSlotPointer;
            LastSlotPointer = from.LastSlotPointer;
            for (int i = 0; i < GlobalConst.WeaponSlotMaxLength; i++)
            {
                // if (from._bitArray[i+2] )
                slotWeapons[i].Sync(from.slotWeapons[i]);
            }
            //from._bitArray.ReleaseReference();
            //from._bitArray = null;
        }
      
        /// <summary>
        /// 等值判定
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public bool IsSimilar(WeaponBagContainer right)
        {
            if (right == null) return false;
            if (HeldSlotPointer != right.HeldSlotPointer || LastSlotPointer != right.LastSlotPointer) return false;
            for (int i = 0; i < GlobalConst.WeaponSlotMaxLength; i++)
            {
                if (slotWeapons[i] != right.slotWeapons[i])
                    return false;
            }
            return true;
        }
        private BitArrayWrapper _bitArray;
        //public void Read(BinaryReader reader)
        //{

        //    if (_bitArray != null) _bitArray.ReleaseReference();
        //    _bitArray =  reader.ReadBitArray();
        //    HeldSlotPointer = _bitArray[0] ? reader.ReadInt32(): HeldSlotPointer;
        //    LastSlotPointer = _bitArray[1] ? reader.ReadInt32() : LastSlotPointer;
        //    for (int i = 0; i < GameGlobalConst.WeaponSlotMaxLength; i++)
        //    {
        //        if (_bitArray[i+2])
        //        {
        //            var entityId = reader.ReadInt32();
        //            var entityType = reader.ReadInt16();
        //            var weaponKey = new EntityKey(entityId, entityType);
        //            slotWeapons[i].Sync(weaponKey);
        //        }
        //    }
        //}
        public void Read(BinaryReader reader)
        {

            HeldSlotPointer = reader.ReadInt32();
            LastSlotPointer = reader.ReadInt32();
            for (int i = 0; i < GlobalConst.WeaponSlotMaxLength; i++)
            {
                var entityId = reader.ReadInt32();
                var entityType = reader.ReadInt16();
                var weaponKey = new EntityKey(entityId, entityType);
                slotWeapons[i].Sync(weaponKey);
            }
        }
        public void Write(WeaponBagContainer comparedInfo, MyBinaryWriter writer)
        {


            writer.Write(HeldSlotPointer);
            writer.Write(LastSlotPointer);

            for (int i = 0; i < GlobalConst.WeaponSlotMaxLength; i++)
            {

                writer.Write(slotWeapons[i].WeaponKey.EntityId);
                writer.Write(slotWeapons[i].WeaponKey.EntityType);

            }
        }
        public void RewindTo(WeaponBagContainer right)
        {
            HeldSlotPointer = right.HeldSlotPointer;
            LastSlotPointer = right.LastSlotPointer;

            for (int i = 0; i < GlobalConst.WeaponSlotMaxLength; i++)
            {
                slotWeapons[i].Sync(right.slotWeapons[i]);
            }

        }
        /// <summary>
        /// 判断与比较值不相同的情况下，将当前值写入Wirter
        /// </summary>
        /// <param name="comparedInfo"></param>
        /// <param name="writer"></param>
        //public void Write(WeaponBagContainer comparedInfo, MyBinaryWriter writer)
        //{

        //    BitArrayWrapper bitArray = BitArrayWrapper.Allocate(2+ GameGlobalConst.WeaponSlotMaxLength, false);
        //    if (comparedInfo == null)
        //    {
        //        bitArray.SetAll(true);
        //    }
        //    else
        //    {
        //        bitArray[0] = true;
        //        bitArray[1] = true;
        //        for (int i = 0; i < GameGlobalConst.WeaponSlotMaxLength; i++)
        //        {
        //            bitArray[i+2] = slotWeapons[i] != comparedInfo.slotWeapons[i];
        //        }
        //    }
        //    writer.Write(bitArray);
        //    writer.Write(HeldSlotPointer);
        //    writer.Write(LastSlotPointer);
        //    for (int i = 0; i < GameGlobalConst.WeaponSlotMaxLength; i++)
        //    {
        //        if(bitArray[i + 2])
        //        {
        //            writer.Write(slotWeapons[i].WeaponKey.EntityId);
        //            writer.Write(slotWeapons[i].WeaponKey.EntityType);
        //        }

        //    }
        //    bitArray.ReleaseReference();
        //}
        public void Dispose()
        {
            if (_bitArray != null)
            {
                _bitArray.ReleaseReference();
            }
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Weapon bag container\n");
            foreach (var weapon in slotWeapons)
            {

                stringBuilder.Append(weapon.ToString());
                stringBuilder.Append("\n");
            }
            stringBuilder.Append("--------+\n");

            return stringBuilder.ToString();

        }
    }
}