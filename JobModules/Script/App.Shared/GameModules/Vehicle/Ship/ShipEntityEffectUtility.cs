using Core.Prediction.VehiclePrediction.Cmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DWP;
using Shared.Scripts.Vehicles;

namespace App.Shared.GameModules.Vehicle
{
    public class ShipEntityEffectUtility : IVehicleEntityEffectUtility
    {
        public void EnableEngineAudio(VehicleEntity vehicle, bool enabled)
        {
            var go = vehicle.gameObject;
            var audio = go.UnityObjWrapper.Value.GetComponent<ShipAudioPack>();
            audio.EnableEngineRunning = enabled;
        }

        public void PlayExplosionEffect(VehicleEntity vehicle)
        {
            var go = vehicle.gameObject;
            var effect = go.UnityObjWrapper.Value.GetComponent<ShipEventEffect>();
            effect.PlayExplosionEffect<VehicleMaterialLoader>();
        }

        public void SetEngineEffectPercent(VehicleEntity vehicle, float percent)
        {
            var go = vehicle.gameObject;
            var effect = go.UnityObjWrapper.Value.GetComponent<ShipEngineEffect>();
            effect.Percent = percent;

            if (percent > 0)
            {
                var eventEffect = go.UnityObjWrapper.Value.GetComponent<ShipEventEffect>();
                eventEffect.SetBrokenMaterial<VehicleMaterialLoader>(false);
            }
        }
    }
}
