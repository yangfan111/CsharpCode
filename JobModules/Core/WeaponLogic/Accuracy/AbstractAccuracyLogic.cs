using Core.WeaponLogic.Attachment;
using Core.WeaponLogic.Common;
using WeaponConfigNs;

namespace Core.WeaponLogic.Accuracy
{
    public abstract class AbstractAccuracyLogic<T1, T3> : AbstractAttachableWeaponLogic<T1, T3>, IAccuracyLogic where T1: ICopyableConfig<T1>, new()
    {
        public AbstractAccuracyLogic(T1 config) : base(config) { }

        public abstract void OnIdle(IPlayerWeaponState state, IWeaponCmd cmd);
        public abstract void BeforeFireBullet(IPlayerWeaponState state, IWeaponCmd cmd, int index);
    }
}
