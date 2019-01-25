using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.GameModules.Vehicle
{
    public interface IVehicleEntityEffectUtility
    {
        void PlayExplosionEffect(VehicleEntity vehicle);
        void SetEngineEffectPercent(VehicleEntity vehicle, float percent);
        void EnableEngineAudio(VehicleEntity vehicle, bool enabled);
    }
}
