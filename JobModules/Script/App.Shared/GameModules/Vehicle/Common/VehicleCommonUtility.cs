using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Vehicle.Common;
using App.Shared.Terrains;
using Core.Configuration;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;
using VehicleCommon;

namespace App.Shared.GameModules.Vehicle
{
    public static class VehicleCommonUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleCommonUtility));
        public static void SetLayer(this VehicleEntity vehicle, int layer)
        {
            if (!vehicle.hasGameObject)
            {
                return;
            }

            var go = vehicle.gameObject.UnityObject.AsGameObject;
            SetLayer(go.transform, layer);
        }

        private static void SetLayer(Transform transform, int layer)
        {
            if (transform.gameObject.layer != UnityLayerManager.GetLayerIndex(EUnityLayerName.Hitbox) &&
                transform.gameObject.layer != UnityLayerManager.GetLayerIndex(EUnityLayerName.VehicleTrigger) &&
                transform.gameObject.layer != UnityLayerManager.GetLayerIndex(EUnityLayerName.Player) &&
                transform.gameObject.layer != UnityLayerManager.GetLayerIndex(EUnityLayerName.VehicleTire))
            {
                transform.gameObject.layer = layer;
            }

            foreach (Transform child in transform)
            {
                SetLayer(child, layer);
            }
        }

        public static Transform GetChildByName(Transform transform, string name)
        {
            if (name == null)
            {
                return null;
            }

            if (transform.name.Equals(name))
            {
                return transform;
            }

            for (int i = 0; i < transform.childCount; ++i)
            {
                var childTransform = GetChildByName(transform.GetChild(i), name);
                if (childTransform != null)
                {
                    return childTransform;
                }
            }

            return null;
        }

        public static TController GetController<TController>(this VehicleEntity vehicle) where TController: class
        {
            var controller = vehicle.gameObject.Controller;
            return controller as TController;
        }

        public static GameObject GetVehicleGameObjectFromChildCollider<TController>(Collider collider)
        {
            Transform transform = collider.transform;
            while (transform.parent != null && transform.GetComponent<TController>() == null)
            {
                transform = transform.parent.transform;
            }

            if (transform.GetComponent<TController>() == null)
            {
                _logger.InfoFormat("Can not get vehicle controller {0} for collider {1}.", typeof(TController), collider.name);
                return null;
            }

            return transform.gameObject;
        }

        public static void SetTimer(VehicleEntity vehicle, VehicleTimer timer)
        {
            var controller = GetController<VehicleCommonController>(vehicle);
            controller.SetTimer(timer);
        }

        public static void InitController(VehicleCommonController controller, bool isServer, int vehicleId)
        {
            controller.VehicleId = vehicleId;
            SetControllerInterface(controller);
            if (isServer)
            {
                DisableEffectScripts(controller);
                controller.processContacts = false;
                controller.disableVisuals = true;
            }
        }

        private static void SetControllerInterface(VehicleCommonController controller)
        {
            controller.GeneralGroundMaterial = SingletonManager.Get<TerrainManager>().GetCurrentTerrain();
            controller.WaterInterface = SingletonManager.Get<VehicleWaterInterface>();
        }

        private static void DisableEffectScripts(VehicleCommonController controller)
        {
            var audioScript = controller.GetComponent<VehicleAbstractAudioPack>();
            if(audioScript != null)
                audioScript.enabled = false;

            var effectScripts = controller.GetComponents<VehicleEffectMonoBehavior>();
            foreach (var script in effectScripts)
            {
                script.enabled = false;
            }
        }


        public static void SetSoundGameData(VehicleBaseGameDataComponent gameData)
        {
            var soundConfigManager = SingletonManager.Get<VehicleSoundConfigManager>();
            gameData.ChannelSoundId1 = soundConfigManager.GetRandomSoundId(1);
            gameData.ChannelSoundId2 = soundConfigManager.GetRandomSoundId(2);
            gameData.ChannelSoundId3 = soundConfigManager.GetRandomSoundId(3);
        }
    }
}
