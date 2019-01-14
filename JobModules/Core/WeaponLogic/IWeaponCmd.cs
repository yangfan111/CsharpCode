using Core.GameInputFilter;

namespace Core.WeaponLogic
{
    public interface IWeaponCmd
    {
        bool IsFire { get; }
        bool IsSpecialFire { get; }
        bool IsSwitchFireMode { get; }
        int FrameInterval { get; }
        int CmdSeq { get; }
        int RenderTime { get; }
        bool IsCanFire { get; }
        bool IsThrowing { get; }
        bool IsReload { get; }
        bool SwitchThrowMode { get; }
        IFilteredInput FilteredInput { get; }
    }
}
