using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Components;
using Core.EntityComponent;
using Core.Enums;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Core.Utils;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Vehicle
{
    [Vehicle]
   
    public class ShipGameDataComponent : VehicleBaseGameDataComponent, ISelfLatestComponent, INonSelfLatestComponent, IComponent
    {
        public int GetComponentId()
        {
            return (int) EComponentIds.ShipGameData;
        }

        public override float DecreaseHp(VehiclePartIndex index, float amount, EUIDeadType damageType, EntityKey damageSource)
        {
            AssertUtility.Assert(amount >= 0);
            return base.DecreaseHp(index, amount, damageType, damageSource);
        }

        public override void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
}
