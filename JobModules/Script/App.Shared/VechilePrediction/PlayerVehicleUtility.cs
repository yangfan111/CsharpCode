using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;

public class PlayerVehicleUtility
{
    public static VehicleEntity GetControlledVehicle(PlayerEntity player, VehicleContext context)
    {
        var selfPlayer = player;
        if (selfPlayer != null && selfPlayer.IsOnVehicle())
        {
            var controlledComp = selfPlayer.controlledVehicle;
            var vehicleEntity = context.GetEntityWithEntityKey(controlledComp.EntityKey);

            if (vehicleEntity != null && player.IsVehicleDriver())
            {
                return vehicleEntity;
            }
        }
        return null;
    }

    public static VehicleEntity GetVehicle(PlayerEntity player, VehicleContext context)
    {
        if (null != player && player.IsOnVehicle())
        {
            var controlledComp = player.controlledVehicle;
            var vehicleEntity = context.GetEntityWithEntityKey(controlledComp.EntityKey);

            if (vehicleEntity != null)
            {
                return vehicleEntity;
            }
        }
        return null;
    }
}