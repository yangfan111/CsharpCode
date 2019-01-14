
using App.Shared.GameModules.Vehicle.Common;
using App.Shared.GameModules.Vehicle.Ship;
using App.Shared.GameModules.Vehicle.WheelCarrier;
using EVP;
using Core.GameModule.Interface;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Vehicle
{
    public static class VehicleMoveUtility
    {
        /* public static void MoveVehicle(this PlayerEntity playerEntity, IVehicleCmd cmd, VehicleContext context)
         {
             
             if (!playerEntity.hasControlledVehicle)
             {
                 return;
             }
 
             //when execute cmd from the player really controlls the object.
             var controlledEntity = playerEntity.controlledVehicle;
             var seat = VehicleEntityUtility.GetVehicleSeat(playerEntity);
             if (!VehicleEntityUtility.IsVehicleDriver(seat))
             {
                 return;
             }
 
             var vehicle = VehicleEntityUtility.GetVehicleEntity(context, controlledEntity);
             if (vehicle == null)
             {
                 return;
             }
 
             vehicle.Move(cmd);
         }*/

        private static IVehicleMoveUtility[] EntityAPI =
        {
            new WheelEntityMoveUtility(),
            new ShipMoveUtility()
        };

        public static void Move(this VehicleEntity vehicle, IVehicleCmd cmd)
        {
            EntityAPI[vehicle.GetTypeValue()].Move(vehicle, cmd);
        }
    }
}
