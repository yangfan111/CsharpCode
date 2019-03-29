using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;

namespace App.Shared.Components.Vehicle
{
    [Vehicle]
    public class VehicleDrivableComponent : IComponent, IResetableComponent
    {
        public bool IsDrivable;
        public float UndrivableTime;

        public void Reset()
        {
            IsDrivable = true;
            UndrivableTime = 0;
        }
    }
}
