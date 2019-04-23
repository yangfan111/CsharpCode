using System.Collections.Generic;
using Core;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Utils;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Weapon
{
     /// <summary>
    /// 武器背包集合
    /// </summary>
    [Player]
    public class PlayerWeaponBagSetComponent : IUserPredictionComponent
    {
        [NetworkProperty] public List<WeaponBagContainer> WeaponBags;
        [NetworkProperty, DontInitilize] public int HeldBagPointer;


        public WeaponBagContainer this[int bagIndex]
        {
            get
            {
                AssertUtility.Assert(bagIndex < WeaponBags.Count);
                if (bagIndex < 0)
                    bagIndex = 0;
                return WeaponBags[bagIndex];
            }
        }

        private bool isInitialized;

        public void CopyFrom(object rightComponent)
        {
            CopyFrom(rightComponent as PlayerWeaponBagSetComponent);
        }

        public void Initialize(int usableLength)
        {
            if (isInitialized)
                return;
            if (WeaponBags != null)
            {
                isInitialized = true;
                return;
            }

            WeaponBags = new List<WeaponBagContainer>(usableLength);
            for (int i = 0; i < usableLength; i++)
            {
                WeaponBags.Add(new WeaponBagContainer());
            }

            isInitialized = true;
        }

        private void CopyFrom(PlayerWeaponBagSetComponent right)
        {
            right.Initialize(GlobalConst.WeaponBagMaxCount);
            Initialize(GlobalConst.WeaponBagMaxCount);
            for (int i = 0; i < GlobalConst.WeaponBagMaxCount; i++)
            {
                //  DebugUtil.LogInUnity("left:{0} right:{1}", WeaponBags[i].ToString(), right.WeaponBags[i]);
                WeaponBags[i].RewindTo(right.WeaponBags[i]);
            }
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponBagSet;
        }

        public bool IsApproximatelyEqual(object right)
        {
            return IsApproximatelyEqual(right as PlayerWeaponBagSetComponent);
        }

        private bool IsApproximatelyEqual(PlayerWeaponBagSetComponent rightComponent)
        {
            if (WeaponBags[0] == null || (!WeaponBags[0].IsSimilar(rightComponent.WeaponBags[0])))
            {
                //builder.Append("Approxiamate diff :");
                //builder.Append("left:"+ WeaponBags[i]+"\n");
                //builder.Append("right:" + rightComponent.WeaponBags[i]+ "\n");
                //Logger.InfoFormat(builder.ToString());
                return false;
            }

            ;
            return true;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        //public override string ToString()
        //{
        //    //string str = "Prepare Convert Value....";
        //    //for (int i = 0; i < GameGlobalConst.WeaponBagMaxCount; i++)
        //    //{
        //    //    if (WeaponBags != null && WeaponBags[i] != null)
        //    //        str += WeaponBags[i].ToString();

        //    //}
        //    return str;
        //}
        public void ClearPointer()
        {
            HeldBagPointer = 0;
            WeaponBags[0].ClearPointer();
        }


        #region//shotcut

        public int HeldSlotIndex
        {
            get { return WeaponBags[0].HeldSlotPointer; }
        }

        public int LastSlotIndex
        {
            get { return WeaponBags[0].LastSlotPointer; }
        }

        public WeaponBagContainer HeldBagContainer
        {
            get { return WeaponBags[0]; }
        }

        #endregion
    }

}