using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.map.position;
using com.wd.free.unit;
using App.Shared.GameModules.Vehicle;
using App.Shared.Components.Vehicle;
using UnityEngine;
using XmlConfig;
using com.wd.free.util;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    [Serializable]
    public class CreateCarAction : AbstractGameAction
    {
        private string carId;

        private IPosSelector pos;

        public override void DoAction(IEventArgs args)
        {
            UnitPosition up = pos.Select(args);

            Contexts con = args.GameContext;

            VehicleEntityUtility.CreateNewVehicle(con.vehicle, FreeUtil.ReplaceInt(carId, args),
                con.session.commonSession.EntityIdGenerator.GetNextEntityId(),
                map.position.UnityPositionUtil.ToVector3(up));
        }
    }
}
