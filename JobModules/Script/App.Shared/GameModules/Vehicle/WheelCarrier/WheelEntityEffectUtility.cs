using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Prediction.VehiclePrediction.Cmd;
using EVP;
using EVP.Scripts;
using Shared.Scripts.Vehicles;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle.WheelCarrier
{
    public class WheelEntityEffectUtility : IVehicleEntityEffectUtility
    {
        public void PlayExplosionEffect(VehicleEntity vehicle)
        {
            var controller = vehicle.GetController<VehicleAbstractController>();
            if (controller.IsInWater())
            {
                PlayExplosionInWaterEffect(vehicle);
            }
            else
            {
                PlayExplosionInLandEffect(vehicle);
            }
        }


        public void SetEngineEffectPercent(VehicleEntity vehicle, float percent)
        {
            var go = vehicle.gameObject;
            var effect = go.UnityObject.AsGameObject.GetComponent<VehicleEngineEffect>();
            effect.Percent = percent;

            if (percent > 0)
            {
                var eventEffect = go.UnityObject.AsGameObject.GetComponent<VehicleEventEffect>();
                eventEffect.SetBrokenMaterial<VehicleMaterialLoader>(false);
            }
        }

        public void EnableEngineAudio(VehicleEntity vehicle, bool enabled)
        {
            var go = vehicle.gameObject;
            var audio = go.UnityObject.AsGameObject.GetComponent<VehicleAudioPack>();
            audio.EnableEngineRunning = enabled;
        }

        public void PlayExplosionInLandEffect(VehicleEntity vehicle)
        {
            var go = vehicle.gameObject;
            var effect = go.UnityObject.AsGameObject.GetComponent<VehicleEventEffect>();
            effect.Explosion<VehicleMaterialLoader>();
        }


        public static void PlayExplosionInWaterEffect(VehicleEntity vehicle)
        {
            var go = vehicle.gameObject;
            var effect = go.UnityObject.AsGameObject.GetComponent<VehicleEventEffect>();
            effect.ExplosionInWater<VehicleMaterialLoader>();
        }

        public static void EnableWheelRender(VehicleEntity vehicle, VehiclePartIndex index, bool enabled)
        {
            var go = vehicle.gameObject;
            var controllerIndex = VehicleIndexHelper.ToVehicleControllerWheelIndex(index);
            var controller = go.UnityObject.AsGameObject.GetComponent<VehicleAbstractController>();
            var rendererRoot = controller.GetTireMeshRenderRoot(controllerIndex);
            if (rendererRoot == null)
            {
                var meshRender = controller.GetTireMeshRender(controllerIndex);
                if (meshRender != null && meshRender.enabled != enabled)
                {
                    meshRender.enabled = enabled;
                }
            }else if (rendererRoot.gameObject.activeSelf != enabled)
            {
                rendererRoot.gameObject.SetActive(enabled);
            }
        }

        public static void PlayWheelExplosion(VehicleEntity vehicle, VehiclePartIndex index)
        {
            //disable wheel mesh render
            EnableWheelRender(vehicle, index, false);

            //play explosion effect
            var go = vehicle.gameObject;
            var controllerIndex = VehicleIndexHelper.ToVehicleControllerWheelIndex(index);
            var effect = go.UnityObject.AsGameObject.GetComponent<VehicleEventEffect>();
            effect.WheelExplosion(controllerIndex);
        }
    }
}
