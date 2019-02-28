using System;
using System.Collections.Generic;
using System.Text;
using App.Shared.Components;
using App.Shared.Configuration;
using App.Shared.Components.Player;
using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle.Ship;
using App.Shared.GameModules.Vehicle.WheelCarrier;
using App.Shared.Player;
using Utils.AssetManager;
using Core.Configuration;
using Core.EntityComponent;
using Core.GameHandler;
using Core.ObjectPool;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Prediction.VehiclePrediction.Event;
using Core.Utils;
using Shared.Scripts.Vehicles;
using UnityEngine;
using Utils.Singleton;
using VehicleCommon;
using XmlConfig;

namespace App.Shared.GameModules.Vehicle
{

    public static class VehicleEntityUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleEntityUtility));

        private static IVehicleEntityUtility[] EntityAPI = 
        {
            new WheelEntityUtility(),
            new ShipEntityUtility()
        };

        public static VehicleEntity CreateNewVehicle(VehicleContext context, int entityId, VehicleAssetConfigItem configItem, Vector3 position,
            Vector3 eulerRotAngle)
        {
            var vehicle = context.CreateEntity();
            vehicle.AddEntityKey(new EntityKey(entityId, (int) EEntityType.Vehicle));
            vehicle.AddVehicleType(configItem.Type);
            vehicle.AddVehicleAssetInfo(configItem.Id, (int)configItem.Type, configItem.BundleName, configItem.AssetName, 
                configItem.TipName, configItem.PostureId, configItem.HasRadio,
                configItem.CameraAnchorOffset, configItem.CameraDistance, configItem.CameraRotationDamping);
            vehicle.AddVehicleDynamicData(position, eulerRotAngle);
            vehicle.AddPosition(Vector3.zero);
            vehicle.position.AlwaysEqual = true;

            vehicle.AddVehicleCmd();
            vehicle.AddVehicleCmdOwner(new VehicleCmdOwnerAdapter(vehicle));
            vehicle.AddVehicleSyncEvent(new Queue<IVehicleSyncEvent>());
            vehicle.AddVehicleCollisionDamage(new Dictionary<EntityKey, VehicleCollisionDamage>(new EntityKeyComparer()));
            return vehicle;
        }

        public static List<VehicleEntity> CreateVehicles(int mapId, VehicleContext context, IEntityIdGenerator idGenerator)
        {
            var birthPoints = GetVehicleBirthPoints(mapId);
            var vehicles = new List<VehicleEntity>();

            foreach (var point in birthPoints)
            {
                var entityId = idGenerator.GetNextEntityId();
                var vehicleId = point.VId;

                var vehicle = CreateNewVehicle(context, vehicleId, entityId, point.Position);

                if (vehicle != null)
                {
                    vehicles.Add(vehicle);
                }
            }

            return vehicles;
        }

        //private static int testVehicleCount = 0;
        public static VehicleEntity CreateNewVehicle(VehicleContext context, int vehicleId, int entityId, Vector3 position)
        {
           
            var configItem = SingletonManager.Get<VehicleAssetConfigManager>().GetConfigItem(vehicleId);
            if (configItem != null)
            {
                //if (testVehicleCount < 40)
                //    position = new Vector3(5, -100, 5);
                var vehicle = CreateNewVehicle(context, entityId, configItem, position, Vector3.zero);

               //testVehicleCount++;
                return vehicle;
            }

            return null;
        }

        private static List<VehicleBirthPoint> GetVehicleBirthPoints(int mapId)
        {
            return SingletonManager.Get<MapConfigManager>().SceneParameters.VehicleBirthPoints;
        }

        public static void SetTimer(this VehicleEntity vehicle, VehicleTimer timer)
        {
            VehicleCommonUtility.SetTimer(vehicle, timer);
        }

        public static void AddVehicleComponentsPostInit(this VehicleEntity vehicle, EVehicleType type, UnityObject unityObj, PlayerContext playerContext, bool isServer)
        {
            var go = unityObj.AsGameObject;
            if (!vehicle.hasGameObject)
            {
                var controller = go.GetComponent<VehicleCommonController>();
                vehicle.AddGameObject(controller, unityObj);
                controller.PutToSleep();
            }

            if (!go.activeSelf)
            {
                var controller = go.GetComponent<VehicleCommonController>();
                controller.IsActive = true;
            }

            go.transform.position = GetNewBirthPositionOnOverlap(go);

            if (vehicle.HasDynamicData())
            {
                var dydata = vehicle.GetDynamicData();
                dydata.Position = go.transform.position;
            }

            if (!vehicle.hasVehicleType)
            {
                vehicle.AddVehicleType(type);
            }
           
            vehicle.AddEntityKeyIdentifier(vehicle.entityKey.Value);
            vehicle.AddVehicleObjectCollision(playerContext);

            vehicle.AddSeatComponent();
            vehicle.AddDynamicAndEffectDataPostInit();

            vehicle.AddVehicleAllGameData();
            vehicle.AddHitBoxComponent();
            vehicle.AddGameEventComponent();

            if (!vehicle.hasVehicleCmd)
            {
                vehicle.AddVehicleCmd();
            }

            if (!vehicle.hasVehicleCmdOwner)
            {
                vehicle.AddVehicleCmdOwner(new VehicleCmdOwnerAdapter(vehicle));
            }

            if (!vehicle.hasVehicleSyncEvent)
            {
                vehicle.AddVehicleSyncEvent(new Queue<IVehicleSyncEvent>());
            }

            if (!vehicle.hasVehicleCollisionDamage)
            {
                vehicle.AddVehicleCollisionDamage(new Dictionary<EntityKey, VehicleCollisionDamage>(new EntityKeyComparer()));
            }

            EntityAPI[vehicle.GetTypeValue()].InitController(vehicle, isServer, vehicle.vehicleAssetInfo.Id);

            vehicle.isFlagSyncSelf = true;
            vehicle.isFlagSyncNonSelf = true;
            vehicle.isFlagCompensation = true;
        }


        private static Vector3 GetNewBirthPositionOnOverlap(GameObject go)
        {
            var position = go.transform.position;
            if (SharedConfig.IsOffline || SharedConfig.IsServer)
            {
                var rigidBody = go.GetComponent<Rigidbody>();
                int tryCount = 0;
                const float checkRadius = 5.0f;
                while (true)
                {
                    var colliders = Physics.OverlapSphere(position, checkRadius, UnityLayerManager.GetLayerMask(EUnityLayerName.Vehicle));
                    if (colliders.Length == 0)
                    {
                        break;
                    }

                    var colliderCount = colliders.Length;
                    var noOtherVehicle = true;
                    for (int i = 0; i < colliderCount; ++i)
                    {
                        if (colliders[i].attachedRigidbody != rigidBody)
                        {
                            noOtherVehicle = false;
                            break;
                        }
                    }

                    if (noOtherVehicle)
                    {
                        break;
                    }


                    _logger.WarnFormat("There is a vehicle in birth position {0}", position);
                    tryCount++;
                    var angle = UnityEngine.Random.Range(0, 2 * (float)Math.PI);
                    var dir = new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle));
                    position += checkRadius * tryCount * dir;
                    const float liftHeight = 1000.0f;
                    position.y += liftHeight;

                    RaycastHit hit;
                    if (Physics.Raycast(position, -Vector3.up, out hit, 2 * liftHeight, UnityLayers.SceneCollidableLayerMask))
                    {
                        position = hit.point + new Vector3(0, 2.0f, 0);
                    }
                }

            }

            return position;
        }

        public static void AddGameEvent(this VehicleEntity vehicle, GameEvent evt)
        {
            var gameEventList = vehicle.vehicleGameEvent;
            gameEventList.Enque(evt);
        }

        private static void AddSeatComponent(this VehicleEntity vehicle)
        {
            if (!vehicle.hasVehicleSeat)
            {
                vehicle.AddVehicleSeat();
            }

            vehicle.ConfigureVehicleSeat();
        }

        public static VehicleSeatConfigInfo GetSeatConfig(this VehicleEntity vehicle)
        {
            var go = vehicle.gameObject.UnityObject.AsGameObject;
            var cfg = go.GetComponent<VehicleBaseConfig>();

            var seatInfo = ObjectAllocatorHolder<VehicleSeatConfigInfo>.Allocate();
            seatInfo.DriverSeat = cfg.driverSeat;
            seatInfo.CodriverSeat = cfg.codriverSeat;
            seatInfo.BackDriverSeat = cfg.backDriverSeat;
            seatInfo.BackCodriverSeat = cfg.backCodriverSeat;
            seatInfo.BackDriverSeat_1 = cfg.backDriverSeat_1;
            seatInfo.BackCodriverSeat_1 = cfg.backCodriverSeat_1;

            return seatInfo;
        }

        private static void ConfigureVehicleSeat(this VehicleEntity vehicle)
        {
            AssertUtility.Assert(vehicle.hasGameObject && vehicle.hasVehicleSeat);

            var config = GetSeatConfig(vehicle);
            var seats = vehicle.vehicleSeat;

            if (config.DriverSeat != null)
            {
                seats.AddSeat((int) VehicleSeatIndex.Driver);
            }

            if (config.CodriverSeat != null)
            {
                seats.AddSeat((int)VehicleSeatIndex.Codriver);
            }

            if (config.BackDriverSeat != null)
            {
                seats.AddSeat((int)VehicleSeatIndex.BackDriver);
            }

            if (config.BackCodriverSeat != null)
            {
                seats.AddSeat((int)VehicleSeatIndex.BackCodriver);
            }

            if (config.BackDriverSeat_1)
            {
                seats.AddSeat((int) VehicleSeatIndex.BackDriver_1);
            }

            if (config.BackCodriverSeat_1)
            {
                seats.AddSeat((int)VehicleSeatIndex.BackCodriver_1);
            }

            ObjectAllocatorHolder<VehicleSeatConfigInfo>.Free(config);
        }


        private static void AddVehicleDynamicData(this VehicleEntity vehicle, Vector3 position = new Vector3(), Vector3 eulerRotAngle = new Vector3())
        {
            var rotation = new Quaternion();
            rotation.eulerAngles = eulerRotAngle;
            EntityAPI[vehicle.GetTypeValue()].AddDynamicData(vehicle, position, rotation);
        }

        private static void AddDynamicAndEffectDataPostInit(this VehicleEntity vehicle)
        {
            EntityAPI[vehicle.GetTypeValue()].AddDynamicAndEffectDataPostInit(vehicle);
        }

        private static void AddVehicleAllGameData(this VehicleEntity vehicle)
        {
            EntityAPI[vehicle.GetTypeValue()].AddVehicleAllGameData(vehicle);
        }

        private static void AddHitBoxComponent(this VehicleEntity vehicle)
        {
            EntityAPI[vehicle.GetTypeValue()].AddHitBox(vehicle);
        }

        private static void AddGameEventComponent(this VehicleEntity vehicle)
        {
            if (!vehicle.hasVehicleGameEvent)
            {
                vehicle.AddVehicleGameEvent();
            }
        }

        private static void AddEntityKeyIdentifier(this VehicleEntity vehicle, EntityKey entityKey)
        {
            var go = vehicle.gameObject.UnityObject.AsGameObject;
            go.AddComponent<EntityReference>();
            var comp = go.GetComponent<EntityReference>();
            comp.Init(vehicle.entityAdapter);
        }

        private static void AddVehicleObjectCollision(this VehicleEntity vehicle, PlayerContext context)
        {
            vehicle.gameObject.UnityObject.AsGameObject.AddComponent<VehicleObjectCollision>();
        }

        public static VehicleEntity GetVehicleEntity(VehicleContext context, ControlledVehicleComponent vehicleComp)
        {
            return context.GetEntityWithEntityKey(vehicleComp.EntityKey);
        }

        public static EVehicleType GetType(this VehicleEntity vehicle)
        {
            return vehicle.vehicleType.VType;
        }

        public static int GetTypeValue(this VehicleEntity vehicle)
        {
            return (int) vehicle.vehicleType.VType;
        }

        public static bool IsRideOffFromLeft(this VehicleEntity vehicle, int seat)
        {
            if (seat == (int)VehicleSeatIndex.Driver || seat == (int)VehicleSeatIndex.BackDriver ||
                seat == (int) VehicleSeatIndex.BackDriver_1)
            {
                return true;
            }

            return false;
        }

        public static int GetVehicleSeat(PlayerEntity player)
        {
            return player.controlledVehicle.Role;
        }

        public static bool HasDriver(this VehicleEntity vehicle)
        {
            if (vehicle.hasVehicleSeat)
            {
                var seats = vehicle.vehicleSeat;
                return seats.IsOccupied((int)VehicleSeatIndex.Driver);
            }

            return false;
        }

        public static bool HasAnyPassager(this VehicleEntity vehicle)
        {
            var seats = vehicle.vehicleSeat;
            return seats.IsAnyOccupied();
        }

        public static bool IsVehicleDriver(this PlayerEntity player)
        {
            var seat = GetVehicleSeat(player);
            return IsVehicleDriver(seat);
        }

        public static bool IsVehicleDriver(this VehicleEntity vehicle, int seat)
        {
            return IsVehicleDriver(seat);
        }

        public static bool IsVehicleDriver(this VehicleEntity vehicle, PlayerEntity player)
        {
            return player.IsVehicleDriver()
                && vehicle.entityKey.Value == player.controlledVehicle.EntityKey;
        }

        public static bool IsVehicleDriver(int seat)
        {
            return seat == (int)VehicleSeatIndex.Driver;
        }


        public static bool AddPassager(this VehicleEntity vehicle, PlayerEntity player, ref int preferedSeat)
        {
            var seats = vehicle.vehicleSeat;
            if (seats.IsConfigured(preferedSeat) && !seats.IsOccupied(preferedSeat))
            {
                vehicle.SetSeatOccupied(preferedSeat, player);
                return true;
            }

            for (int seat = 1; seat <= (int) VehicleSeatIndex.MaxSeatCount; ++seat)
            {
                if (seats.IsConfigured(seat) && !seats.IsOccupied(seat))
                {
                    preferedSeat = seat;
                    vehicle.SetSeatOccupied(preferedSeat,  player);
                    return true;
                }
            }

            preferedSeat = (int) VehicleSeatIndex.None;
            return false;
        }

        public static void SeatPlayer(this VehicleEntity vehicle, PlayerEntity player, int newSeat)
        {
            player.controlledVehicle.Role = newSeat;
            vehicle.SetSeatOccupied(newSeat, player);
            
        }

        public static void SetSeatOccupied(this VehicleEntity vehicle, int seat, PlayerEntity player)
        {
            var seats = vehicle.vehicleSeat;
            seats.SetOccupation(seat, player.entityKey.Value, IsVehicleDriver(seat));
        }

        public static Transform GetVehicleSeatTransform(this VehicleEntity vehicle, PlayerEntity player)
        {
            if (vehicle.hasGameObject)
            {
                var config = GetSeatConfig(vehicle);
                Transform seatTransform;
                var seat = GetVehicleSeat(player);
                switch (seat)
                {
                    case (int) VehicleSeatIndex.Driver:
                        seatTransform = config.DriverSeat;
                        break;
                    case (int)VehicleSeatIndex.Codriver:
                        seatTransform = config.CodriverSeat;
                        break;
                    case (int)VehicleSeatIndex.BackDriver:
                        seatTransform = config.BackDriverSeat;
                        break;
                    case (int) VehicleSeatIndex.BackCodriver:
                        seatTransform = config.BackCodriverSeat;
                        break;
                    case (int)VehicleSeatIndex.BackDriver_1:
                        seatTransform = config.BackDriverSeat_1;
                        break;
                    case (int)VehicleSeatIndex.BackCodriver_1:
                        seatTransform = config.BackCodriverSeat_1;
                        break;
                    default:
                        throw new Exception("Undefined Vehicle Seat!");
                }

                ObjectAllocatorHolder<VehicleSeatConfigInfo>.Free(config);
                return seatTransform;
            }

            return null;
        }

        public static void EnablePassagerCollider(this VehicleEntity vehicle, PlayerEntity player)
        {
            var go = vehicle.gameObject.UnityObject.AsGameObject;
            var cfg = go.GetComponent<VehicleBaseConfig>();
            bool enable = player.IsOnVehicle();
            if (cfg.passagerColider)
            {
                var seat = GetVehicleSeatByHierachy(vehicle, player);
                switch (seat)
                {
                    case (int) VehicleSeatIndex.Driver:
                        if (cfg.driverCollider != null)
                        {
                            cfg.driverCollider.gameObject.SetActive(enable);
                        }
                        break;
                    case (int)VehicleSeatIndex.Codriver:
                        if (cfg.coDriverCollider != null)
                        {
                            cfg.coDriverCollider.gameObject.SetActive(enable);
                        }
                        break;
                    case (int)VehicleSeatIndex.BackDriver:
                        if (cfg.backDriverCollider != null)
                        {
                            cfg.backDriverCollider.gameObject.SetActive(enable);
                        }
                        break;
                    case (int)VehicleSeatIndex.BackCodriver:
                        if (cfg.backCodriverCollider != null)
                        {
                            cfg.backCodriverCollider.gameObject.SetActive(enable);
                        }
                        break;
                    case (int)VehicleSeatIndex.BackDriver_1:
                        if (cfg.backDriverCollider_1 != null)
                        {
                            cfg.backDriverCollider_1.gameObject.SetActive(enable);
                        }
                        break;
                    case (int)VehicleSeatIndex.BackCodriver_1:
                        if (cfg.backCodriverCollider_1 != null)
                        {
                            cfg.backCodriverCollider_1.gameObject.SetActive(enable);
                        }
                        break;
                }
            }
        }

        private static int GetVehicleSeatByHierachy(VehicleEntity vehicle, PlayerEntity player)
        {
            var parent = player.RootGo().transform.parent;
            if (parent != null)
            {
                var go = vehicle.gameObject.UnityObject.AsGameObject;
                var cfg = go.GetComponent<VehicleBaseConfig>();

                if (cfg.driverSeat == parent)
                {
                    return (int) VehicleSeatIndex.Driver;
                }

                if (cfg.codriverSeat == parent)
                {
                    return (int) VehicleSeatIndex.Codriver;
                }

                if (cfg.backDriverSeat == parent)
                {
                    return (int) VehicleSeatIndex.BackDriver;
                }

                if (cfg.backCodriverSeat == parent)
                {
                    return (int) VehicleSeatIndex.BackCodriver;
                }

                if (cfg.backDriverSeat_1 == parent)
                {
                    return (int) VehicleSeatIndex.BackDriver_1;
                }

                if (cfg.backCodriverSeat_1 == parent)
                {
                    return (int) VehicleSeatIndex.BackCodriver_1;
                }
            }

            return (int) VehicleSeatIndex.None;
        }

        public static void RemovePassager(this VehicleEntity vehicle, PlayerEntity player)
        {
            vehicle.UnseatPlayer(player);
        }

        public static void UnseatPlayer(this VehicleEntity vehicle,  PlayerEntity player)
        {
            var seat =  GetVehicleSeat(player);
            vehicle.UnseatPlayer(player, seat);
        }

        public static void UnseatPlayer(this VehicleEntity vehicle, PlayerEntity player, int seat)
        {
            var seats = vehicle.vehicleSeat;
            seats.RemoveOccupation(seat);

            var seatPosition = player.RootGo().transform.parent.position;
            player.RootGo().transform.SetParent(null, false);

            player.RootGo().transform.position = seatPosition;
            player.position.Value = seatPosition;
        }

        public static bool IsOnVehicleSeat(this VehicleEntity vehicle, PlayerEntity player, int seat)
        {
            if (GetVehicleSeat(player) == seat)
            {
                return true;
            }

            return false;
        }

        public static bool ChangeSeatOnVehicle(this VehicleEntity vehicle, PlayerEntity player, int newSeat)
        {
            var seats = vehicle.vehicleSeat;
            if (newSeat > (int)VehicleSeatIndex.MaxSeatCount)
            {
                return false;
            }

            if (seats.IsOccupied(newSeat) || !seats.IsConfigured(newSeat))
            {
                return false;
            }

            vehicle.UnseatPlayer(player);
            vehicle.SeatPlayer(player, newSeat);

            return true;
        }

        public static int FindPreferedSeat(this VehicleEntity vehicle, PlayerEntity player)
        {
            var position = player.RootGo().transform.position;

            return vehicle.FindPreferedSeat(position);
        }

        public static int FindPreferedSeat(this VehicleEntity vehicle, Vector3 position)
        {
            AssertUtility.Assert(vehicle.hasGameObject && vehicle.hasGameObject);

            var config = GetSeatConfig(vehicle);

            float distance = System.Single.MaxValue;
            int seat = (int)VehicleSeatIndex.None;

            var seats = vehicle.vehicleSeat;
            if (config.DriverSeat != null && !seats.IsOccupied((int)VehicleSeatIndex.Driver))
            {
                var dist = (config.DriverSeat.position - position).sqrMagnitude;
                if (dist < distance)
                {
                    distance = dist;
                    seat = (int)VehicleSeatIndex.Driver;
                }
            }

            if (config.CodriverSeat != null && !seats.IsOccupied((int)VehicleSeatIndex.Codriver))
            {
                var dist = (config.CodriverSeat.position - position).sqrMagnitude;
                if (dist < distance)
                {
                    distance = dist;
                    seat = (int)VehicleSeatIndex.Codriver;
                }
            }

            if (config.BackDriverSeat != null && !seats.IsOccupied((int)VehicleSeatIndex.BackDriver))
            {
                var dist = (config.BackDriverSeat.position - position).sqrMagnitude;
                if (dist < distance)
                {
                    distance = dist;
                    seat = (int)VehicleSeatIndex.BackDriver;
                }
            }

            if (config.BackCodriverSeat != null && !seats.IsOccupied((int)VehicleSeatIndex.BackCodriver))
            {
                var dist = (config.BackCodriverSeat.position - position).sqrMagnitude;
                if (dist < distance)
                {
                    distance = dist;
                    seat = (int)VehicleSeatIndex.BackCodriver;
                }
            }

            if (config.BackDriverSeat_1 != null && !seats.IsOccupied((int) VehicleSeatIndex.BackDriver_1))
            {
                var dist = (config.BackDriverSeat_1.position - position).sqrMagnitude;
                if (dist < distance)
                {
                    distance = dist;
                    seat = (int)VehicleSeatIndex.BackDriver_1;
                }
            }

            if (config.BackCodriverSeat_1 != null && !seats.IsOccupied((int)VehicleSeatIndex.BackCodriver_1))
            {
                var dist = (config.BackCodriverSeat_1.position - position).sqrMagnitude;
                if (dist < distance)
                {
                    seat = (int)VehicleSeatIndex.BackCodriver_1;
                }
            }

            ObjectAllocatorHolder<VehicleSeatConfigInfo>.Free(config);
            return seat;
        }

        public static VehicleDynamicDataComponent GetDynamicData(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].GetDynamicData(vehicle);
        }

        public static bool HasDynamicData(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].HasDynamicData(vehicle);
        }

        public static float GetThrottleInput(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].GetThrottleInput(vehicle);
        }

        public static bool IsAccelerated(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].IsAccelerated(vehicle);
        }

        public static bool IsSleepingDisable(this VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller != null && controller.DisableSleeping;
        }

        public static void DisableSleeping(this VehicleEntity vehicle, bool disable)
        {
            var controller = GetController(vehicle);
            if (controller != null)
            {
                controller.DisableSleeping = disable;
            }
        }

        public static void SetKinematic(this VehicleEntity vehicle, bool isKinematic, bool reserveVelocity = false)
        {
            var controller = GetController(vehicle);
            if (controller != null)
            {
                var linearVelocity = controller.Velocity;
                var angularVelocity = controller.AngularVelocity;

                controller.IsKinematic = isKinematic;
                if (reserveVelocity)
                {
                    controller.Velocity = linearVelocity;
                    controller.AngularVelocity = angularVelocity;
                }
            }
        }

        public static bool IsKinematic(this VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller == null || controller.IsKinematic;
        }

        public static void SetActive(this VehicleEntity vehicle, bool active)
        {
            if (vehicle.hasGameObject)
            {
                var controller = GetController(vehicle);
                controller.IsActive = active;
            }
        }

        public static bool IsActiveSelf(this VehicleEntity vehicle)
        {
            if (vehicle.hasGameObject)
            {
                return GetController(vehicle).IsActive;
            }

            return false;
        }

        public static void SetLodLevel(this VehicleEntity vehicle, bool isLowLevel)
        {
            if (vehicle.hasGameObject)
            {
                GetController(vehicle).SetLodLevel(isLowLevel);
            }   
        }

        public static bool IsLowLod(this VehicleEntity vehicle)
        {
            return GetController(vehicle).IsLowLod();
        }

        public static void ConfigHitBoxImposter(this VehicleEntity vehicle, GameObject hitboxImposter)
        {
            EntityAPI[vehicle.GetTypeValue()].ConfigHitBoxImposter(vehicle, hitboxImposter);
        }

        public static float GetHitFactor(this VehicleEntity vehicle, Collider collider, out VehiclePartIndex partIndex)
        {
            return EntityAPI[vehicle.GetTypeValue()].GetHitFactor(vehicle, collider, out partIndex);
        }

        public static void UpdateHitBoxes(this VehicleEntity vehicle, IGameEntity gameEntity)
        {
            EntityAPI[vehicle.GetTypeValue()].UpdateHitBoxes(vehicle, gameEntity);
        }

        public static void GetCollisionObjectFactor(this VehicleEntity vehicle, Collider collider, out float factorA, out float factorB, out float boxFactor)
        {
            EntityAPI[vehicle.GetTypeValue()].GetCollisionObjectFactor(vehicle, collider, out factorA, out factorB, out boxFactor);
        }

        public static void GetCollisionPlayerFactor(this VehicleEntity vehicle, out float factor)
        {
            EntityAPI[vehicle.GetTypeValue()].GetCollisionPlayerFactor(vehicle, out factor);
        }

        public static void GetExplosionToPlayerFactor(this VehicleEntity vehicle, out float factorA, out float factorB)
        {
            EntityAPI[vehicle.GetTypeValue()].GetExplosionToPlayerFactor(vehicle, out factorA, out factorB);
        }

        public static void GetExplosionToVehicleFactor(this VehicleEntity vehicle, out float factorA, out float factorB)
        {
            EntityAPI[vehicle.GetTypeValue()].GetExplosionToVehicleFactor(vehicle, out factorA, out factorB);
        }

        public static Vector3 GetExplosionCenter(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].GetExplosionCenter(vehicle);
        }

        public static bool IsCar(this VehicleEntity vehicle)
        {
            return vehicle.vehicleType.VType == EVehicleType.Car;
        }

        public static bool IsShip(this VehicleEntity vehicle)
        {
            return vehicle.vehicleType.VType == EVehicleType.Ship;
        }

        public static VehicleEntity GetVehicleFromChildCollider(Collider collider)
        {
            if (collider == null)
            {
                return null;
            }

            GameObject go = VehicleCommonUtility.GetVehicleGameObjectFromChildCollider<VehicleCommonController>(collider);

            if (go == null)
            {
                _logger.ErrorFormat("can not find vehicle for collider {0}", collider.name);
                return null;
            }

            var entityRef = go.GetComponent<EntityReference>();
            if (entityRef == null)
            {
                _logger.ErrorFormat("entity reference is null for vehicle {0}", go.name);
                return null;
            }
            
            return (VehicleEntity)entityRef.Reference;
        }

        public static bool HasGameData(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].HasGameData(vehicle);
        }

        public static bool HasHitBoxBuffer(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].HasHitBoxBuffer(vehicle);
        }

        public static void BuildVehicleGUIInfo(this VehicleEntity vehicle, int index, StringBuilder infoBuilder)
        {
            EntityAPI[vehicle.GetTypeValue()].BuildVehicleGUIInfo(vehicle, index, infoBuilder);
        }

        public static void SetDebugInput(this VehicleEntity vehicle, VechileDebugInput inputType, float value)
        {
            EntityAPI[vehicle.GetTypeValue()].SetDebugInput(vehicle, inputType, value);
        }

        public static VehicleBaseGameDataComponent GetGameData(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].GetGameData(vehicle);
        }

        public static void SetControllerBroken(this VehicleEntity vehicle)
        {
            EntityAPI[vehicle.GetTypeValue()].SetControllerBroken(vehicle);
        }

        public static void ClearInput(this VehicleEntity vehicle)
        {
            EntityAPI[vehicle.GetTypeValue()].ClearInput(vehicle);
        }

        public static void EnableInput(this VehicleEntity vehicle, bool enabled)
        {
            EntityAPI[vehicle.GetTypeValue()].EnableInput(vehicle, enabled);
        }

        public static void SetHandBrakeOn(this VehicleEntity vehicle, bool isOn)
        {
            EntityAPI[vehicle.GetTypeValue()].SetHandBrakeOn(vehicle, isOn);
        }

        //if syncRemote = true, the reseted pose is depedent on the remote side's (server) calculation.
        public static void ResetPose(this VehicleEntity vehicle, bool syncRemote)
        {
            EntityAPI[vehicle.GetTypeValue()].ResetPose(vehicle, syncRemote);
        }

        public static bool IsCrashed(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].IsCrashed(vehicle);
        }

        public static bool IsPassagerInWater(this VehicleEntity vehicle)
        {
            if (vehicle.IsShip())
            {
                return false;
            }

            return EntityAPI[vehicle.GetTypeValue()].IsInWater(vehicle);
        }

        public static bool IsInWater(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].IsInWater(vehicle);
        }

        public static bool IsFocusable(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].IsFocusable(vehicle);
        }

        public static bool IsRidable(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].IsRidable(vehicle);
        }

        public static Vector3 GetLinearVelocity(this VehicleEntity vehicle)
        {
            var controller = GetController(vehicle);
            return controller == null ? Vector3.zero : controller.Velocity;
        }

        public static float GetUiPresentSpeed(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].GetUiPresentSpeed(vehicle);
        }

        public static Vector3 GetPrevLinearVelocity(this VehicleEntity vehicle)
        {
            return EntityAPI[vehicle.GetTypeValue()].GetPrevLinearVelocity(vehicle);
        }

        public static void AddImpulseAtPosition(this VehicleEntity vehicle, Vector3 impulse, Vector3 position)
        {
            EntityAPI[vehicle.GetTypeValue()].AddImpulseAtPosition(vehicle, impulse, position);
        }

        public static IVehicleCmd CreateVehicleCmd(IVehicleCmdGenerator generator, VehicleContext context, PlayerEntity player, int currentSimulationTime)
        {
            if (!player.IsOnVehicle())
            {
                return null;
            }   

            var vehicle = context.GetEntityWithEntityKey(player.controlledVehicle.EntityKey);
            if (vehicle == null)
            {
                return null;
            }

            
            var vehicleCmd = generator.GeneratorVehicleCmd(currentSimulationTime);
            if (vehicleCmd == null)
            {
                return null;
            }

            vehicleCmd.PlayerId = player.entityKey.Value.EntityId;
            vehicleCmd.VehicleId = vehicle.entityKey.Value.EntityId;

            if (!SharedConfig.ServerAuthorative)
            {
                vehicle.SetVehicleStateToCmd(vehicleCmd);
            }

            return vehicleCmd;
        }

        public static bool IsPlayerOverlapAtPosition(PlayerEntity player, Vector3 position, int layerMask)
        {
            Vector3 p1, p2;
            float radius;
            PlayerEntityUtility.GetCapsule(player, position, out p1, out p2, out radius);

            var colliders = Physics.OverlapCapsule(p1, p2, radius, layerMask) ;
            return colliders.Length > 0;
        }

        public static bool GetRideOffPosition(PlayerEntity playerEntity, VehicleEntity vehicleEntity, Vector3 direction, out Vector3 resolvedPosition, float liftHeight = 0.01f, float displacement = 0.05f)
        {
            var character = playerEntity.RootGo();
            const float sweepDistance = 5.0f;
            direction = -direction;
            var p = character.transform.position - direction * sweepDistance;

            Vector3 p1, p2;
            float radius;
            PlayerEntityUtility.GetCapsule(playerEntity, p, out p1, out p2, out radius);

            vehicleEntity.SetLayer(UnityLayerManager.GetLayerIndex(EUnityLayerName.User));

            resolvedPosition = character.transform.position;
            var hit = new RaycastHit();
            var hitDist = 2.0f;

            var lowOffset = new Vector3(0, -0.5f, 0);
            if (Physics.CapsuleCast(p1, p2, radius, direction, out hit, sweepDistance, UnityLayerManager.GetLayerMask(EUnityLayerName.User)) ||
                //the seat position may be higher than the vehicle's height, then low the position to get collided position
                Physics.CapsuleCast(p1 + lowOffset, p2 + lowOffset, radius, direction, out hit, sweepDistance, UnityLayerManager.GetLayerMask(EUnityLayerName.User)))
            {
                hitDist = hit.distance;
            }
            

            {
                var distance = (hitDist - displacement) * direction;

                var colliders = Physics.OverlapCapsule(p1 + distance, p2 + distance, radius);
                if (colliders.Length > 0)
                {
                    vehicleEntity.SetLayer(UnityLayerManager.GetLayerIndex(EUnityLayerName.Vehicle));
                    return false;
                }

               
                var disp = direction * (hitDist - displacement - sweepDistance);
                var position = resolvedPosition;
                position += disp;

                if (liftHeight > 0.0f)
                {
                    RaycastHit upHit; ;
                    Physics.CapsuleCast(p1 + disp, p2 + disp, radius, Vector3.up, out upHit, liftHeight);
                    position += upHit.distance * Vector3.up;
                }

                var dist = resolvedPosition - position;
                var ray = new Ray(position, dist.normalized);
    
                if (Physics.Raycast(ray, out hit, dist.magnitude, UnityLayers.AllCollidableLayerMask))
                {
                    vehicleEntity.SetLayer(UnityLayerManager.GetLayerIndex(EUnityLayerName.Vehicle));
                    return false;
                }

                resolvedPosition = position;
            }
            
            vehicleEntity.SetLayer(UnityLayerManager.GetLayerIndex(EUnityLayerName.Vehicle));
            return true;
        }

        public static void Notify(this VehicleEntity vehicle, GameEvent evt)
        {
            vehicle.vehicleGameEvent.Enque(evt);
        }

        public static bool CompareAndStore<T>(this VehicleEntity vehicle, string name, T val) where T: struct
        {
            return vehicle.vehicleGameEvent.CompareAndStore<T>(name, val);
        }

        public static void Reset(this VehicleEntity vehicle, bool isActiveAfterReset = false)
        {
            var controller = GetController(vehicle);
            controller.ResetGameObject(typeof(VehicleMaterialLoader), isActiveAfterReset);
        }

        private static VehicleCommonController GetController(VehicleEntity vehicle)
        {
            return vehicle.gameObject.Controller;
        }
    }
}

