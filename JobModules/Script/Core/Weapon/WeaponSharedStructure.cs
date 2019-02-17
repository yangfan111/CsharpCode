using Assets.Utils.Configuration;
using Utils.Singleton;
namespace Core
{
    public interface I
    /// <summary>
    /// 武器展示数据
    /// </summary>
    public struct WeaponScanInfo
    {
        public int Key;//weapon Entity Key
        public int Id;
        public int AvatarId;
        public int Muzzle;
        public int Magazine;
        public int Stock;
        public int UpperRail;
        public int LowerRail;
        public int Bullet;
        public int ReservedBullet;
        public readonly static WeaponScanInfo Empty = new WeaponScanInfo();
        public override string ToString()
        {
            return string.Format("id : {0}, avatarId {1}, muzzle {2}, magazine {3}, stock {4}, upper {5}, lower {6}, bullet {7}, reserved {8}",
                Id, AvatarId, Muzzle, Magazine, Stock, UpperRail, LowerRail, Bullet, ReservedBullet);
        }
        public bool IsVailed { get { return Id > 0; } }
        public bool IsSafeVailed
        {
            get
            {
                if (Id < 1) return false;
                return SingletonManager.Get<WeaponConfigManager>().GetConfigById(Id) != null;
            }
        }
    }

  

}