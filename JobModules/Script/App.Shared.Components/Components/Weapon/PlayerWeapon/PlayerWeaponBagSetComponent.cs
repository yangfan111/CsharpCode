using Core;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;

namespace App.Shared.Components.Weapon
{

     /// <summary>
    /// 武器背包集合
    /// </summary>
    [Player]
    public class PlayerWeaponBagSetComponent : ISelfLatestComponent
    {
        [NetworkProperty] public WeaponBagContainer WeaponBag;



        private bool isInitialized;

        public void CopyFrom(object rightComponent)
        {
            CopyFrom(rightComponent as PlayerWeaponBagSetComponent);
        }

        public void Initialize()
        {
            if (isInitialized)
                return;
            if (WeaponBag != null)
            {
                isInitialized = true;
                return;
            }

            WeaponBag = new WeaponBagContainer();
            isInitialized = true;
        }

        private void CopyFrom(PlayerWeaponBagSetComponent right)
        {
            right.Initialize();
            Initialize();
            WeaponBag.RewindTo(right.WeaponBag);
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
            if (WeaponBag == null || (!WeaponBag.IsSimilar(rightComponent.WeaponBag)))
            {
                //builder.Append("Approxiamate diff :");
                //builder.Append("left:"+ WeaponBags[i]+"\n");
                //builder.Append("right:" + rightComponent.WeaponBags[i]+ "\n");
                //Logger.InfoFormat(builder.ToString());
                return false;
            }
            return true;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public override string ToString()
        {
            string str = "Prepare Convert Value....";
            return WeaponBag.ToString();
            return str;
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent as PlayerWeaponBagSetComponent);
        }

     
    }

}