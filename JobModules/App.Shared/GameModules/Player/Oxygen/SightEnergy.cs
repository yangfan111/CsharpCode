using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.GameModules.Player.Oxygen
{
    public class SightEnergy : BaseEnergy
    {
        private readonly float _subSightEnergySpeed = 20.0f;
        // debuff
        private readonly float _debuffTime = 2.5f;
        private float _currentDebuffTime = 0;

        public float SightShiftBuff { set; get; }
        public bool InShiftState;

        private bool _canShift = true;
        private bool _sureShift;
        public bool SureShift()
        {
            return _sureShift;
        }

        public override float Update(float deltaTime)
        {
            CalcSureShift();
            SightShiftBuff = (_sureShift && CurrentOxygen > MinOxygenEnergy) ?
                0.1f : 1.0f;

            if (CurrentOxygen < MinOxygenEnergy)
            {
                _currentDebuffTime = _debuffTime;
            }

            _currentDebuffTime -= deltaTime;
            if (_currentDebuffTime < 0)
            {
                _currentDebuffTime = 0;
            }
            InDebuffState = _currentDebuffTime > 0;

            if (_sureShift)
            {
                return deltaTime * _subSightEnergySpeed;
            }
            return 0;
        }

        private void CalcSureShift()
        {
            if (_canShift && CurrentOxygen < MaxOxygenEnergy / 2.0f && !InShiftState || InDebuffState)
            {
                _canShift = false;
            }

            if (!_canShift && CurrentOxygen >= MaxOxygenEnergy / 2.0f)
            {
                _canShift = true;
            }

            _sureShift = InShiftState && _canShift;
        }
    }
}
