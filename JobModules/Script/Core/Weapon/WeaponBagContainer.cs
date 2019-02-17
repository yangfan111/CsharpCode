using Core.Animation;
using Core.Utils;
using System;
using System.IO;
using Core.SnapshotReplication.Serialization.Serializer;
using Utils.Utils.Buildin;
using Core.EntityComponent;

namespace Core
{

    public class WeaponBagContainer : IPatchClass<WeaponBagContainer>, IDisposable
    {
        private BitArrayWrapper bitArrayWraper;
        public event WeaponHeldUpdateEvent OnHeldWeaponUpdate;
        public WeaponBagContainer() : this(true)
        {
        }
        public WeaponBagContainer(bool initialize)
        {
            for (int i = 0; i < GameGlobalConst.WeaponSlotMaxLength; i++)
            {
                slotWeapons[i] = new WeaponBagSlotData();
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
        private WeaponBagSlotData[] slotWeapons = new WeaponBagSlotData[GameGlobalConst.WeaponSlotMaxLength];

        public int HeldSlotPointer { get; private set; }

        public int LastSlotPointer { get; private set; }


        public void ChangeSlotPointer(int nowSlot)
        {
        
            LastSlotPointer = HeldSlotPointer;
            HeldSlotPointer = nowSlot;
            if (OnHeldWeaponUpdate != null)
                OnHeldWeaponUpdate();
        }

        public WeaponBagSlotData HeldSlotData { get { return slotWeapons[HeldSlotPointer]; } }

        public WeaponBagSlotData LastSlotData { get { return slotWeapons[LastSlotPointer]; } }

        public WeaponBagSlotData this[EWeaponSlotType slot]
        {
            get
            {
                int index = ToIndex(slot);
                AssertUtility.Assert(index < slotWeapons.Length);
                return slotWeapons[index];
            }
        }
        public bool HasValue { get; set; }



        public WeaponBagContainer Clone()
        {

            WeaponBagContainer clone = new WeaponBagContainer(false);
            clone.HeldSlotPointer = HeldSlotPointer;
            clone.LastSlotPointer = LastSlotPointer;
            for (int i = 0; i < GameGlobalConst.WeaponSlotMaxLength; i++)
            {
                clone.slotWeapons[i] = slotWeapons[i].Clone();
            }
            return clone;
        }
        ~WeaponBagContainer()
        {
            Dispose();
        }

        public void MergeFromPatch(WeaponBagContainer from)
        {
            bool hasUtil = from.bitArrayWraper != null;
            HeldSlotPointer = from.HeldSlotPointer;
            LastSlotPointer = from.LastSlotPointer;
            for (int i = 0; i < GameGlobalConst.WeaponSlotMaxLength; i++)
            {
                if (!hasUtil || from.bitArrayWraper[i])
                    slotWeapons[i].Sync(from.slotWeapons[i]);
            }
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
            for (int i = 0; i < GameGlobalConst.WeaponSlotMaxLength; i++)
            {
                if (slotWeapons[i] != right.slotWeapons[i])
                    return false;
            }
            return true;
        }

        public void Read(BinaryReader reader)
        {
            HeldSlotPointer = reader.ReadInt32();
            LastSlotPointer = reader.ReadInt32();
            if (bitArrayWraper != null) bitArrayWraper.ReleaseReference();
            bitArrayWraper = reader.ReadBitArray();
            for (int i = 0; i < GameGlobalConst.WeaponSlotMaxLength; i++)
            {
                if (bitArrayWraper[i])
                {
                    var entityId = reader.ReadInt32();
                    var entityType = reader.ReadInt16();
                    var weaponKey = new EntityKey(entityId, entityType);
                    slotWeapons[i].Sync(weaponKey);
                }
            }
        }

        public void RewindTo(WeaponBagContainer right)
        {
            HeldSlotPointer = right.HeldSlotPointer;
            LastSlotPointer = right.LastSlotPointer;
            for (int i = 0; i < GameGlobalConst.WeaponSlotMaxLength; i++)
            {
                slotWeapons[i].Sync(right.slotWeapons[i]);
            }

        }
        /// <summary>
        /// 判断与比较值不相同的情况下，将当前值写入Wirter
        /// </summary>
        /// <param name="comparedInfo"></param>
        /// <param name="writer"></param>
        public void Write(WeaponBagContainer comparedInfo, MyBinaryWriter writer)
        {

            writer.Write(HeldSlotPointer);
            writer.Write(LastSlotPointer);
            BitArrayWrapper bitArray = BitArrayWrapper.Allocate(GameGlobalConst.WeaponSlotMaxLength, false);
            //  return new BitArray(5, true);
            if (comparedInfo == null)
            {
                bitArray.SetAll(true);
            }
            else
            {
                for (int i = 0; i < GameGlobalConst.WeaponSlotMaxLength; i++)
                {
                    bitArray[i] = slotWeapons[i] != comparedInfo.slotWeapons[i];

                }
            }

            writer.Write(bitArray);
            for (int i = 0; i < GameGlobalConst.WeaponSlotMaxLength; i++)
            {
                if (bitArray[i])
                {
                    writer.Write(slotWeapons[i].WeaponKey.EntityId);
                    writer.Write(slotWeapons[i].WeaponKey.EntityType);
                }
            }


            bitArray.ReleaseReference();
        }
        public void Dispose()
        {
            if (bitArrayWraper != null)
            {
                bitArrayWraper.ReleaseReference();
            }
        }
        public override string ToString()
        {
            string s = "*****Weapon bag container*****\n";
            foreach (var wp in slotWeapons)
            {
                s += wp.ToString() + "\n";
            }
            s += "*****************";
            return s;
        }


    }
}