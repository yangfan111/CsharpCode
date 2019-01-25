using System;
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
    
    public class CarGameDataComponent : VehicleBaseGameDataComponent, ISelfLatestComponent, INonSelfLatestComponent, IComponent
    {

        [DontInitilize] public int WheelCount;
        [DontInitilize] public int MaxWheelHp;

        [NetworkProperty] [DontInitilize] public float FirstWheelHp;

        [NetworkProperty] [DontInitilize] public float SecondWheelHp;

        [NetworkProperty] [DontInitilize] public float ThirdWheelHp;

        [NetworkProperty] [DontInitilize] public float FourthWheelHp;

        public override float DecreaseHp(VehiclePartIndex index, float amount, EUIDeadType damageType, EntityKey damageSource)
        {
            AssertUtility.Assert(amount >= 0);

            switch (index)
            {
                case VehiclePartIndex.FirstWheel:
                    FirstWheelHp = FirstWheelHp < amount ? 0.0f : FirstWheelHp - amount;
                    return FirstWheelHp;
                case VehiclePartIndex.SecondWheel:
                    SecondWheelHp = SecondWheelHp < amount ? 0.0f : SecondWheelHp - amount;
                    return SecondWheelHp;
                case VehiclePartIndex.ThirdWheel:
                    ThirdWheelHp = ThirdWheelHp < amount ? 0.0f : ThirdWheelHp - amount;
                    return ThirdWheelHp;
                case VehiclePartIndex.FourthWheel:
                    FourthWheelHp = FourthWheelHp < amount ? 0.0f : FourthWheelHp - amount;
                    return FourthWheelHp;
                default:
                    return base.DecreaseHp(index, amount, damageType, damageSource);
            }
        }

        public float GetWheelHp(VehiclePartIndex index)
        {
            switch (index)
            {
                case VehiclePartIndex.FirstWheel:
                    return FirstWheelHp;
                case VehiclePartIndex.SecondWheel:
                    return SecondWheelHp;
                case VehiclePartIndex.ThirdWheel:
                    return ThirdWheelHp;
                case VehiclePartIndex.FourthWheel:
                    return FourthWheelHp;
                default:
                    throw new Exception("Undefined Vehicle Wheel Index!");
            }
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.CarGameData;
        }

        public override void CopyFrom(object rightComponent)
        {
            base.CopyFrom(rightComponent);

            var r = (CarGameDataComponent) rightComponent;
           
            FirstWheelHp = r.FirstWheelHp;
            SecondWheelHp = r.SecondWheelHp;
            ThirdWheelHp = r.ThirdWheelHp;
            FourthWheelHp = r.FourthWheelHp;
        }
        
        public void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public override void Reset()
        {
            FirstWheelHp = SecondWheelHp = ThirdWheelHp = FourthWheelHp = MaxWheelHp;
            base.Reset();
        }

        public override void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }
}
