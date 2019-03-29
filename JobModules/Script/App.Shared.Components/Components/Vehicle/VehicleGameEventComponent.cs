using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.GameHandler;
using Entitas;

namespace App.Shared.Components.Vehicle
{
    [Vehicle]
    public class VehicleGameEventComponent : GameEventComponent, IComponent, IVehicleResetableComponent
    {

        public void Reset()
        {
            _eventQueue.Clear();
        }
    }
}
