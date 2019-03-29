using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.GameModules.Player.Oxygen
{
    public class DivingEnergy : BaseEnergy
    {
        private readonly float _subDivingEnergySpeed = 10;
        public bool InDivingState;
        public override float Update(float deltaTime)
        {
            if (CurrentOxygen < MinOxygenEnergy)
            {
                InDebuffState = true;
            }
            else
            {
                InDebuffState = false;
            }

            if (InDivingState)
            {
                return deltaTime * _subDivingEnergySpeed;
            }
            return 0;
        }
    }
}
