using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.GameModules.Player.Oxygen
{
    public class BaseEnergy
    {
        public float MaxOxygenEnergy { set; get; }
        public float MinOxygenEnergy { set; get; }
        public float CurrentOxygen;
        public bool InDebuffState;
        public virtual float Update(float deltaTime)
        {
            return 0;
        }
    }
}
