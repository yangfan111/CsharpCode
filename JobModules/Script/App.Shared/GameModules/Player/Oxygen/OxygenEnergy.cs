using App.Shared.Components.Oxygen;
using Core.Utils;
using Utils.Appearance;

namespace App.Shared.GameModules.Player.Oxygen
{
    public class OxygenEnergy : IOxygenEnergy, IPredictedOxygenState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(OxygenEnergy));
        private readonly float _addOxygenSpeed = 10;

        public float SightShiftBuff { get { return _sightEnergy.SightShiftBuff; } }
        // 处于屏息状态
        public bool InShiftState { set { _sightEnergy.InShiftState = value; }
            get { return _sightEnergy.SureShift(); } }
        // 处于潜水状态
        public bool InDivingState { set { _divingEnergy.InDivingState = value; }
            get { return _divingEnergy.InDivingState; } }
        // 处于Debuff状态
        public bool InSightDebuffState { get { return _sightEnergy.InDebuffState; } }
        public bool InDivingDeffState { get { return _divingEnergy.InDebuffState; } }
        // energy
        public int ShiftVeryTime { get; set; }
        
        private float _currentOxygen = 0;

        private readonly SightEnergy _sightEnergy = new SightEnergy();
        private readonly DivingEnergy _divingEnergy = new DivingEnergy();

        public float MaxOxygenEnergy { private set; get; }
        public float MinOxygenEnergy { private set; get; }
        public float CurrentOxygen { get { return _currentOxygen; } set { _currentOxygen = value; } }

        public OxygenEnergy(float maxEnergy, float minEnergy)
        {
            MaxOxygenEnergy = maxEnergy;
            MinOxygenEnergy = minEnergy;
            _sightEnergy.MaxOxygenEnergy = maxEnergy;
            _sightEnergy.MinOxygenEnergy = minEnergy;
            _divingEnergy.MaxOxygenEnergy = maxEnergy;
            _divingEnergy.MinOxygenEnergy = minEnergy;
        }

        public void ResetOxygen(bool needReset)
        {
            if(needReset)
                _currentOxygen = MinOxygenEnergy;
        }

        public void UpdateOxygenEnergy(float deltaTime)
        {
            // 消耗氧气
            _currentOxygen -= _sightEnergy.Update(deltaTime);
            _currentOxygen -= _divingEnergy.Update(deltaTime);
            // 回复氧气
            if (!InShiftState && !InDivingState)
            {
                _currentOxygen += _addOxygenSpeed * deltaTime;
            }

            _sightEnergy.CurrentOxygen = _currentOxygen;
            _divingEnergy.CurrentOxygen = _currentOxygen;

            if (_currentOxygen >= MaxOxygenEnergy)
            {
                _currentOxygen = MaxOxygenEnergy;
            }
            else if (_currentOxygen < MinOxygenEnergy)
            {
                _currentOxygen = MinOxygenEnergy;
            }
        }

        public void SyncFrom(IPredictedOxygenState state)
        {
            CurrentOxygen = state.CurrentOxygen;
        }

        public void SyncTo(IPredictedOxygenState state)
        {
            state.CurrentOxygen = CurrentOxygen;
        }
    }
}
