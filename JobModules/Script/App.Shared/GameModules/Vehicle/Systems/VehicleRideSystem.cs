using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using App.Shared.Player;
using Core.Components;
using Core.EntityComponent;
using Core.Enums;
using Core.GameModule.Interface;
using Core.GameTime;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle
{
    public class VehicleRideSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(VehicleRideSystem));
        private VehicleContext _vehicleContext;
        private const float MaxVehicleEnterDistance = 5f;
        private ICurrentTime _currentTime;
        private Contexts _contexts;
        public VehicleRideSystem(Contexts contexts)
        {
            _vehicleContext = contexts.vehicle;
            _contexts = contexts;
            _currentTime = contexts.session.currentTimeObject;
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            if (cmd.PredicatedOnce)
            {
                return;
            }

            var playerEntity = (PlayerEntity)owner.OwnerEntity;

            if (CheckRideOffVehicle(playerEntity))
            {
                RideOffVehicle(playerEntity);
                return;
            }

            if (cmd.IsUseAction) 
            {
                if (!playerEntity.IsOnVehicle())
                {
                    if( cmd.UseType == (int)EUseActionType.Vehicle)
                    {
                        CheckAndRideOnVehicle(playerEntity, cmd.UseEntityId, cmd.UseVehicleSeat);
                    }
                }
                else
                {
                    RideOffVehicle( playerEntity);
                }
            }
            else if (cmd.ChangedSeat > 0)
            {
                ChangeSeatOnVehicle(playerEntity, cmd.ChangedSeat);
            }
        }
        private bool CheckRideOffVehicle(PlayerEntity playerEntity)
        {
            var controlledVehicle = playerEntity.controlledVehicle;
            if (controlledVehicle.IsLocalOnVehicle)
            {
                //the player is dead, ride off vehicle
                if (!playerEntity.gamePlay.IsLifeState(EPlayerLifeState.Alive))
                    return true;

                return controlledVehicle.IsRideOffSignalOn();
            }


            return false;
        }

        private bool CheckRideOnVehicle(PlayerEntity playerEntity)
        {
            const float minRideOnInterval = 0.5f;
            if (playerEntity.gamePlay.IsLifeState(EPlayerLifeState.Alive) && Time.time > playerEntity.controlledVehicle.LastVehicleControllTime + minRideOnInterval)
            {
                return true;
            }
            return false;
        }


        private void CheckAndRideOnVehicle(PlayerEntity playerEntity, int vehicleEntityId, int seatId)
        {
            if(CheckRideOnVehicle(playerEntity)){

                var vehicle = _vehicleContext.GetEntityWithEntityKey(new EntityKey(vehicleEntityId, (short)EEntityType.Vehicle));
                if(DistanceTooLarge(playerEntity, vehicle))
                {
                    Logger.Error("player is too far away from vehicle , wrong logic or client is cheating");
                    return;
                }
                if (vehicle != null)
                {
                    var preferedSeat = seatId;
                    if (!vehicle.vehicleSeat.IsOccupied(seatId))
                    {
                        playerEntity.autoMoveInterface.PlayerAutoMove.StopAutoMove();
                        RideOnVehicle(playerEntity, vehicle, preferedSeat);
                    }
                    else
                    {
                        var seat = vehicle.FindPreferedSeat(playerEntity);
                        RideOnVehicle(playerEntity, vehicle, seat);
                        Logger.Error("seat client use is occupied, wrong logic or client is cheating ");
                    }
                }
                else
                {
                    Logger.ErrorFormat("no vehicle entity with id {0}", vehicleEntityId);
                }
            }
        }

        private bool DistanceTooLarge(PlayerEntity playerEntity, VehicleEntity vehicle)
        {
            var distance = Vector3.Distance(playerEntity.position.Value, vehicle.position.Value);
            return distance > MaxVehicleEnterDistance;
        }

       
        private void RideOnVehicle(PlayerEntity playerEntity, VehicleEntity vehicle, int preferedSeat)
        {
            if(!playerEntity.IsVehicleEnterable(vehicle))
            {
                return;
            }

            if (!vehicle.IsRidable())
            {
                vehicle.ResetPose(!SharedConfig.IsOffline && !SharedConfig.IsServer);
                return;
            }

            if (!vehicle.AddPassager(playerEntity, ref preferedSeat))
            {
                Logger.Debug("add passager failed");
                return;
            }

            var rigidBody = vehicle.gameObject.UnityObject.AsGameObject.GetComponent<Rigidbody>();
            playerEntity.controlledVehicle.RideOn(preferedSeat, vehicle.entityKey.Value, rigidBody, _currentTime.CurrentTime);
            CheckAndAddOwnerId(vehicle, playerEntity);
            SetPositionInterpolateMode(playerEntity);
            playerEntity.SetCharacterStateWithVehicle(_contexts, _vehicleContext);
        }

        private void RideOffVehicle(PlayerEntity playerEntity)
        {
            if (playerEntity.IsOnVehicle())
            {
                var vehicle = _vehicleContext.GetEntityWithEntityKey(playerEntity.controlledVehicle.EntityKey);
                if (vehicle == null)
                {
                    return;
                }

                var seat = VehicleEntityUtility.GetVehicleSeat(playerEntity);
                CheckAndRemoveOwnerId(vehicle, seat);
                vehicle.RemovePassager(playerEntity);
                SetRideOffPosition(playerEntity, vehicle);
                //Logger.ErrorFormat("PosAfterRideOff_{0} : {1},{2},{3}", playerEntity.userCmd.Latest.Seq,
                //    playerEntity.position.Value.x, playerEntity.position.Value.y, playerEntity.position.Value.z);
                AddRideOffVelocity(playerEntity, vehicle);
                SetPositionInterpolateMode(playerEntity);
            }

            playerEntity.controlledVehicle.RideOff(_currentTime.CurrentTime);
            playerEntity.SetCharacterStateWithVehicle(_contexts, _vehicleContext);
        }

        private void SetPositionInterpolateMode(PlayerEntity playerEntity)
        {
            playerEntity.position.InterpolateType = (int)PositionInterpolateMode.Discrete;
            playerEntity.position.ServerTime = _currentTime.CurrentTime;
        }

        private void SetRideOffPosition(PlayerEntity playerEntity, VehicleEntity vehicle)
        {
            //set the player ride-off position
            var seat = VehicleEntityUtility.GetVehicleSeat(playerEntity);
            if (vehicle.IsRideOffFromLeft(seat))
            {
                if (RideOffFromLeft(playerEntity, vehicle))
                {
                    return;
                }

                if (RideOffFromRight(playerEntity, vehicle))
                {
                    return;
                }
            }
            else
            {
                if (RideOffFromRight(playerEntity, vehicle))
                {
                    return;
                }

                if (RideOffFromLeft(playerEntity, vehicle))
                {
                    return;
                }
            }

            //if the player can not ride off from left or right, try to ride off from back or front
            if (RideOffFromBack(playerEntity, vehicle))
            {
                return;
            }

            if (RideOffFromFront(playerEntity, vehicle))
            {
                return;
            }

            //almost never happen, the vehicle is stuck, and the player has to ride off from the top of the vehicle.
            RideOffFromTop(playerEntity, vehicle);
        }

        private void AddRideOffVelocity(PlayerEntity playerEntity, VehicleEntity vehicle)
        {
            if (playerEntity.gamePlay.IsLifeState(EPlayerLifeState.Alive))
            {
                playerEntity.playerMove.Velocity = vehicle.GetLinearVelocity();
                playerEntity.playerMove.IsGround = false;
                playerEntity.stateInterface.State.Freefall();
            }
        }

        private bool RideOffFromLeft(PlayerEntity playerEntity, VehicleEntity vehicleEntity)
        {

            var vehicle = vehicleEntity.gameObject.UnityObject.AsGameObject;
            var direction = -vehicle.transform.right;

            return RideOffFromDirection(playerEntity, vehicleEntity, direction); ;
        }

        private bool RideOffFromRight(PlayerEntity playerEntity, VehicleEntity vehicleEntity)
        {
            var vehicle = vehicleEntity.gameObject.UnityObject.AsGameObject;
            var direction = vehicle.transform.right;

            return RideOffFromDirection(playerEntity, vehicleEntity, direction); ;
        }

        private bool RideOffFromFront(PlayerEntity playerEntity, VehicleEntity vehicleEntity)
        {
            var vehicle = vehicleEntity.gameObject.UnityObject.AsGameObject;
            var direction = vehicle.transform.forward;

            return RideOffFromDirection(playerEntity, vehicleEntity, direction); ;
        }

        private bool RideOffFromBack(PlayerEntity playerEntity, VehicleEntity vehicleEntity)
        {
            var vehicle = vehicleEntity.gameObject.UnityObject.AsGameObject;
            var direction = -vehicle.transform.forward;

            return RideOffFromDirection(playerEntity, vehicleEntity, direction); ;
        }
       
        private void RideOffFromTop(PlayerEntity playerEntity, VehicleEntity vehicleEntity)
        {
            RideOffFromDirection(playerEntity, vehicleEntity, Vector3.up);
        }

        private bool RideOffFromDirection(PlayerEntity playerEntity, VehicleEntity vehicleEntity, Vector3 direction)
        {
            Vector3 position;
            bool resolved = true;

            if (SharedConfig.IsServer)
            {
                position = playerEntity.moveUpdate.VehicleRideOffOffset;
            }else
            {
                
                var radius = playerEntity.characterContoller.Value.radius;
                resolved = VehicleEntityUtility.GetRideOffPosition(playerEntity, vehicleEntity, direction, out position, 0.3f, radius * 2);
                playerEntity.moveUpdate.VehicleRideOffOffset = position;
            }

            playerEntity.RootGo().transform.position = position;
            playerEntity.position.Value = position;
            return resolved;
        }


        private void ChangeSeatOnVehicle(PlayerEntity playerEntity, int seatIndex)
        {
            if (!playerEntity.IsOnVehicle())
            {
                return;
            }

            var vehicleComp = playerEntity.controlledVehicle;
            var vehicle = _vehicleContext.GetEntityWithEntityKey(vehicleComp.EntityKey);
            if (vehicle == null)
            {
                return;
            }

            var seat = GetSeatIdFromCmdSeatIndex(vehicle, seatIndex);
            if (seat < 0)
            {
                return;
            }

            if (vehicle.IsOnVehicleSeat(playerEntity, seat))
            {
                return;
            }

           
            if (!vehicle.ChangeSeatOnVehicle(playerEntity, seat))
            {
                return;
            }

            var originSeat = VehicleEntityUtility.GetVehicleSeat(playerEntity);

            CheckAndRemoveOwnerId(vehicle, originSeat);
            CheckAndAddOwnerId(vehicle, playerEntity);

            playerEntity.DriveStart(_contexts, seat, vehicle.vehicleAssetInfo.PostureId);
        }

        private int GetSeatIdFromCmdSeatIndex(VehicleEntity vehicle, int seatIndex)
        {
            var seats = vehicle.vehicleSeat;
            return seats.GetSeatIdByIndex(seatIndex);
        }

        private void CheckAndAddOwnerId(VehicleEntity vehicle, PlayerEntity playerEntity)
        {
            var ownerId = playerEntity.entityKey.Value;
            if (playerEntity.IsVehicleDriver())
            {
                if (vehicle.hasOwnerId)
                {
                    if (vehicle.ownerId.Value == ownerId)
                    {
                        return;
                    }
                    
                    vehicle.RemoveOwnerId();
                }
                
                vehicle.AddOwnerId(ownerId);
            }
        }

        private void CheckAndRemoveOwnerId(VehicleEntity vehicle, int seat)
        {
            if (vehicle.hasOwnerId && vehicle.IsVehicleDriver(seat))
            {
                vehicle.RemoveOwnerId();
            }
        }

        
    }
}
