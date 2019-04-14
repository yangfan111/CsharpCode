using System.Text;
using Assets.Utils.Configuration;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Utils;
using Entitas.CodeGeneration.Attributes;
using Utils.Singleton;

namespace App.Shared.Components.Weapon
{
    [Weapon]
    public class WeaponBasicDataComponent : IUserPredictionComponent
    {
        [DontInitilize, NetworkProperty] public  int              ConfigId;
        [DontInitilize, NetworkProperty] public  int              WeaponAvatarId;
        [DontInitilize, NetworkProperty] public  int              UpperRail;
        [DontInitilize, NetworkProperty] public  int              LowerRail;
        [DontInitilize, NetworkProperty] public  int              Stock;
        [DontInitilize, NetworkProperty] public  int              Muzzle;
        [DontInitilize, NetworkProperty] public  int              Magazine;
        [DontInitilize, NetworkProperty] public  int              Bullet;
        [DontInitilize, NetworkProperty] public  bool             PullBolt;
        [DontInitilize, NetworkProperty] public  int              FireModel;
        [DontInitilize, NetworkProperty] public  int              ReservedBullet;
        [DontInitilize]                  private WeaponAllConfigs configCache;

        public int RealFireModel
        {
            get
            {
                if (FireModel == 0)
                {
                    if (configCache == null || configCache.S_Id != ConfigId)
                        configCache = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(ConfigId);
                    return (int) configCache.GetDefaultFireModel();
                }

                return FireModel;

                //if (FireModel == 0 && ConfigId > 0)
                //{
                //    var weaponAllConfig = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(ConfigId);
                //    FireModel = (int)weaponAllConfig.GetDefaultFireModel();
                //}
                //return FireModel;
            }
        }

        //public override string ToString()
        //{
        //    builder.Length = 0;
        //    builder.Append(ConfigId);
        //    builder.Append("**");
        //    builder.Append(WeaponAvatarId);
        //    builder.Append("**");
        //    builder.Append(UpperRail);
        //    builder.Append("**");

        //    builder.Append(LowerRail);
        //    builder.Append("**");

        //    builder.Append(Stock);
        //    builder.Append("**");

        //    builder.Append(Muzzle);
        //    builder.Append("**");

        //    builder.Append(Magazine);
        //    builder.Append("**");

        //    builder.Append(Bullet);
        //    builder.Append("**");
        //    builder.Append(PullBolt);
        //    builder.Append("**");
        //    builder.Append(FireModel);
        //    builder.Append("**");
        //    builder.Append(ClipSize);
        //    builder.Append("**");

        //    builder.Append(ReservedBullet);
        //    return builder.ToString();
        //}

//        public static explicit operator WeaponObjectComponent(WeaponBasicDataComponent remote)
//        {
//            var newComp = new WeaponObjectComponent();
//            newComp.ConfigId = remote.ConfigId;
//            newComp.WeaponAvatarId = remote.WeaponAvatarId;
//            newComp.UpperRail = remote.UpperRail;
//            newComp.LowerRail = remote.LowerRail;
//            newComp.Stock = remote.Stock;
//            newComp.Muzzle = remote.Muzzle;
//            newComp.Magazine = remote.Magazine;
//            newComp.Bullet = remote.Bullet;
//            newComp.ReservedBullet = remote.ReservedBullet;
//         //   newComp.ClipSize = remote.ClipSize;
//            //newComp.PullBolt       = remote.PullBolt;
//            //newComp.FireModel      = remote.FireModel;
//            return newComp;
//        }

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as WeaponBasicDataComponent;
            ConfigId       = remote.ConfigId;
            WeaponAvatarId = remote.WeaponAvatarId;
            UpperRail      = remote.UpperRail;
            LowerRail      = remote.LowerRail;
            Stock          = remote.Stock;
            Muzzle         = remote.Muzzle;
            Magazine       = remote.Magazine;
            Bullet         = remote.Bullet;
            ReservedBullet = remote.ReservedBullet;
            PullBolt       = remote.PullBolt;
            FireModel      = remote.FireModel;
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.WeaponBasicInfo;
        }

        private static LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponBasicDataComponent));

        StringBuilder builder = new StringBuilder();

        public bool IsApproximatelyEqual(object right)
        {
            var remote = right as WeaponBasicDataComponent;
            var result = ConfigId == remote.ConfigId &&
                         WeaponAvatarId == remote.WeaponAvatarId &&
                         UpperRail == remote.UpperRail &&
                         LowerRail == remote.LowerRail &&
                         Stock == remote.Stock &&
                         Muzzle == remote.Muzzle &&
                         Magazine == remote.Magazine &&
                         Bullet == remote.Bullet &&
                         ReservedBullet == remote.ReservedBullet &&
                         PullBolt == remote.PullBolt &&
                         FireModel == remote.FireModel;
            //    ClipSize == remote.ClipSize;
            //Logger.InfoFormat("Left:{0}\n right :{1}", this, remote);
            return result;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public void Reset()
        {
            CopyFrom(Empty);
        }

        public static readonly WeaponBasicDataComponent Empty = new WeaponBasicDataComponent();
    }

}