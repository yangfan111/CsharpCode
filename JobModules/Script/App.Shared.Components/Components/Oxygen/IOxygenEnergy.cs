
using Utils.Appearance;

namespace App.Shared.Components.Oxygen
{
    public interface IOxygenEnergy
    {
        float SightShiftBuff { get; }
        // 处于屏息状态
        bool InShiftState { set; get; }
        // 处于潜水状态
        bool InDivingState { set; get; }
        // 处于Debuff状态
        bool InSightDebuffState { get; }
        bool InDivingDeffState { get; }

        float CurrentOxygen { get; set; }

        void SyncFrom(IPredictedOxygenState state);
        void SyncTo(IPredictedOxygenState state);

        void ResetOxygen(bool needReset);
        void UpdateOxygenEnergy(float deltaTime);
    }
}
