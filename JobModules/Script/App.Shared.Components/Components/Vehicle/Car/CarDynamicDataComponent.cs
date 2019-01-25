using Core.Components;
using Core.Interpolate;
using Core.Playback;
using Core.Prediction.VehiclePrediction;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Utils;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Vehicle
{
    
    public abstract class 
        CarDynamicDataComponent : VehicleDynamicDataComponent
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(CarDynamicDataComponent));


        [NetworkProperty]
        [DontInitilize]
        public bool IsHornOn;

        [NetworkProperty]
        [DontInitilize]
        public  float BrakeInput; //for car : brake Input, for motor: stuntInput

        [NetworkProperty]
        [DontInitilize]
        public  float HandbrakeInput;

        [NetworkProperty]
        [DontInitilize]
        public float SteerAngle;//for car: steer angle, for motor:  horizontal shift

        [NetworkProperty]
        public bool Crashed;

       
        public override void CopyFrom(object rightComponent)
        {
            base.CopyFrom(rightComponent);

            var data = (CarDynamicDataComponent)rightComponent;

            IsHornOn = data.IsHornOn;

            BrakeInput = data.BrakeInput;
            HandbrakeInput = data.HandbrakeInput;

            SteerAngle = data.SteerAngle;

            Crashed = data.Crashed;
        }

        public override void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            base.Interpolate(left, right, interpolationInfo);

            var l = (CarRewindDataComponent)left;
            var r = (CarRewindDataComponent)right;

            var rotio = interpolationInfo.Ratio;
            IsHornOn = l.IsHornOn && r.IsHornOn;
            BrakeInput = InterpolateUtility.Interpolate(l.BrakeInput, r.BrakeInput, rotio);
            HandbrakeInput = InterpolateUtility.Interpolate(l.HandbrakeInput, r.HandbrakeInput, rotio);

            SteerAngle = InterpolateUtility.Interpolate(l.SteerAngle, r.SteerAngle, rotio);
 
            Crashed = r.Crashed;
        }

        public override string ToString()
        {
            return string.Format("Position: {0}, Rotation: {1}", Position.ToStringExt(), Rotation.ToStringExt());
        }

        public override void Reset()
        {
            IsHornOn = false;
            BrakeInput = 0;
            HandbrakeInput = 0;
            SteerAngle = 0;
            Crashed = false;
            
            base.Reset();
        }
    }


 
    [Vehicle]
    
    public class CarRewindDataComponent : CarDynamicDataComponent, IVehiclePredictionComponent, IPlaybackComponent
    {
        public override void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public override void SyncLatestFrom(object rightComponent)
        {
            CopyFrom(rightComponent);
        }

        public override int GetComponentId()
        {
            return (int)EComponentIds.CarRewindData;
        }   
    }
}
