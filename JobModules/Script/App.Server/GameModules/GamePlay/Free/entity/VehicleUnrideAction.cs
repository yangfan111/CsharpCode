using App.Shared.GameModules.Player;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using System;

namespace App.Server.GameModules.GamePlay.Free.hall
{
    [Serializable]
    public class VehicleUnrideAction : AbstractGameAction, IRule
    {
        public int type;

        public override void DoAction(IEventArgs args)
        {
            if (type == 1)
            {
                foreach (PlayerEntity player in args.GameContext.player.GetInitializedPlayerEntities())
                {
                    if(player.hasControlledVehicle)
                        player.controlledVehicle.ForceRideOff();
                }
            }

            if (type == 2)
            {
                foreach (VehicleEntity vehicle in args.GameContext.vehicle.GetEntities())
                {
                    vehicle.isFlagDestroy = true;
                }
            }
        }

        public int GetRuleID()
        {
            return (int) ERuleIds.VehicleUnrideAction;
        }
    }
}
