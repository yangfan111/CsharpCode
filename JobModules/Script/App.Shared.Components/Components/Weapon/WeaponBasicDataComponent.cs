using Assets.Utils.Configuration;
using Core.Components;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Utils;
using Entitas.CodeGeneration.Attributes;
using System.Text;
using Utils.Singleton;

namespace App.Shared.Components.Weapon
{
    [Weapon]
    public class WeaponBasicDataComponent : IUserPredictionComponent
    {
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int ConfigId;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int WeaponAvatarId;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int UpperRail;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int LowerRail;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int SideRail;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Stock;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Muzzle;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Magazine;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Bullet;
        [DontInitilize, NetworkProperty] public bool PullBolt;
        [DontInitilize, NetworkProperty(127,0,1)] public int FireModel;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int ReservedBullet;
        [DontInitilize] private WeaponAllConfigs configCache;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Bore;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Feed;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Trigger;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Interlock;
        [DontInitilize, NetworkProperty(SyncFieldScale.PositiveInt)] public int Brake;

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
            }
        }

        public void CopyFrom(object rightComponent)
        {
            var remote = rightComponent as WeaponBasicDataComponent;
            ConfigId = remote.ConfigId;
            WeaponAvatarId = remote.WeaponAvatarId;
            UpperRail = remote.UpperRail;
            LowerRail = remote.LowerRail;
            SideRail = remote.SideRail;
            Stock = remote.Stock;
            Muzzle = remote.Muzzle;
            Magazine = remote.Magazine;
            Bullet = remote.Bullet;
            ReservedBullet = remote.ReservedBullet;
            PullBolt = remote.PullBolt;
            FireModel = remote.FireModel;
            Bore = remote.Bore;
            Feed = remote.Feed;
            Trigger = remote.Trigger;
            Interlock = remote.Interlock;
            Brake = remote.Brake;
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
            var result = ConfigId == remote.ConfigId && WeaponAvatarId == remote.WeaponAvatarId && UpperRail == remote.UpperRail &&
                LowerRail == remote.LowerRail && SideRail == remote.SideRail && Stock == remote.Stock &&
                Muzzle == remote.Muzzle && Magazine == remote.Magazine && Bullet == remote.Bullet &&
                ReservedBullet == remote.ReservedBullet && PullBolt == remote.PullBolt && FireModel == remote.FireModel &&
                Bore == remote.Bore && Feed == remote.Feed && Trigger == remote.Trigger &&
                Interlock == remote.Interlock && Brake == remote.Brake;
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