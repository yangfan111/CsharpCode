using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Components;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.SyncLatest;
using Core.Utils;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Vehicle
{
  
    [Vehicle]
    
    public class VehicleBrokenFlagComponent : ISelfLatestComponent, INonSelfLatestComponent, IComponent, IVehicleResetableComponent
    {
        [DontInitilize]
        [NetworkProperty]
        public int Flag; //for uplevel vehicle state

        [DontInitilize]
        public int ColliderFlag; //for downlevel state for unity colliders of vehicle  


        public void Init()
        {
            Flag = 0;
            ColliderFlag = 0;
        }

        public bool IsBodyBroken()
        {
            return (Flag & (int) 0x1) != 0;
        }

        public bool IsVehiclePartBroken(VehiclePartIndex index)
        {
            AssertUtility.Assert(index != VehiclePartIndex.Body);
            return (Flag & (1 << ((int)index + 1))) != 0;
        }

        public void SetBodyBroken()
        {
            Flag |= 0x1;
        }

        public void SetVehiclePartBroken(VehiclePartIndex index)
        {
            AssertUtility.Assert(index != VehiclePartIndex.Body);
            Flag |= (1 << ((int) index + 1));
        }

        public bool IsBodyColliderBroken()
        {
            return (ColliderFlag & ((int) 0x1)) != 0;
        }

        public void SetBodyColliderBroken()
        {
            ColliderFlag |= 0x1;
        }

        public bool IsVehiclePartColliderBroken(VehiclePartIndex index)
        {
            AssertUtility.Assert(index != VehiclePartIndex.Body);
            return (ColliderFlag & (1 << ((int)index + 1))) != 0;
        }

        public void SetVehiclePartColliderBroken(VehiclePartIndex index)
        {
            AssertUtility.Assert(index != VehiclePartIndex.Body);
            ColliderFlag |= 1 << ((int) index + 1);
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.VehicleBrokenFlag;
        }

        public void CopyFrom(object rightComponent)
        {
            var r = (VehicleBrokenFlagComponent) rightComponent;
            Flag = r.Flag;
        }

        public void Reset()
        {
            Flag = 0;
            ColliderFlag = 0;
        }

        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

}
