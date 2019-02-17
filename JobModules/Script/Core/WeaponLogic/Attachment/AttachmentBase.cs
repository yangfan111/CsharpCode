using Core.Configuration;
using System.Collections.Generic;

namespace Core.WeaponLogic.Attachment
{
    public interface IPlayerWeaponConfigManager
    {
        ExpandWeaponLogicConfig GetWeaponLogicConfig(int id, WeaponPartsStruct weaponParts);
    }

    public class WeaponPartsStructComparer : IEqualityComparer<WeaponPartsStruct>
    {
        public bool Equals(WeaponPartsStruct x, WeaponPartsStruct y)
        {
            return x.LowerRail == y.LowerRail
                && x.UpperRail == y.UpperRail
                && x.Stock == y.Stock
                && x.Muzzle == y.Muzzle
                && x.Magazine == y.Magazine;
        }

        public int GetHashCode(WeaponPartsStruct obj)
        {
            int hash = 17;
            // Maybe nullity checks, if these are objects not primitives!
            hash = hash * 23 + obj.LowerRail.GetHashCode();
            hash = hash * 23 + obj.UpperRail.GetHashCode();
            hash = hash * 23 + obj.Magazine.GetHashCode();
            hash = hash * 23 + obj.Muzzle.GetHashCode();
            hash = hash * 23 + obj.Stock.GetHashCode();
            return hash;
        }

        private static readonly WeaponPartsStructComparer _instance = new WeaponPartsStructComparer();
        public static WeaponPartsStructComparer Instance
        {
            get
            {
                return _instance;
            }
        }
    }

    public struct WeaponPartsStruct
    {
        public int UpperRail;
        public int LowerRail;
        public int Magazine;
        public int Stock;
        public int Muzzle;

        public static WeaponPartsStruct Default()
        {
            return new WeaponPartsStruct
            {
                UpperRail = 0,
                LowerRail = 0,
                Magazine = 0,
                Stock = 0,
                Muzzle = 0,
            };
        }

        public WeaponPartsStruct Clone()
        {
            var result = new WeaponPartsStruct();
            result.UpperRail = UpperRail;
            result.LowerRail = LowerRail;
            result.Magazine = Magazine;
            result.Stock = Stock;
            result.Muzzle = Muzzle;
            return result;
        }
        public WeaponPartsStruct Sync (WeaponScanStruct scanData)
        {
            LowerRail = scanData.LowerRail;
            UpperRail = scanData.UpperRail;
            Magazine = scanData.Magazine;
            Stock = scanData.Stock;
            Muzzle = scanData.Muzzle;
            return this;
        }
        public override string ToString()
        {
            return string.Format("Upper {0}, Lower {1}, Magazine {2}, Stock {3}, Muzzle {4}",
                UpperRail,
                LowerRail,
                Magazine,
                Stock,
                Muzzle);
        }
    }
}
