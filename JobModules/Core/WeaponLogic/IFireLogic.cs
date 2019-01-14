using Core.WeaponLogic.Attachment;
using WeaponConfigNs;

namespace Core.WeaponLogic
{
    public interface IFireLogic : IBulletContainer
    {
        void SetAttachment(WeaponPartsStruct attachments);
        void OnFrame(IPlayerWeaponState playerWeapon, IWeaponCmd cmd);
        void SetVisualConfig(ref VisualConfigGroup config);
        void Reset();
        float GetFocusSpeed();
        float GetBreathFactor();
        float GetReloadSpeed();
        float GetFocusFov();
        bool IsFovModified();
    }
}