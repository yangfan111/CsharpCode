using Core.ObjectPool;
using Core.Utils;
using UnityEngine;

namespace Core.Prediction.VehiclePrediction.Cmd
{
    public struct CarSteerable
    {

        public Vector3 Position;

        public Quaternion Rotation;

        public float Angle;

//       public bool Grounded;
//
//        public bool GroundedOnTerrain;
//        public float ColliderSteerAngle;
//        public float SteerAngle;
//        public float MotorTorque;
//
//        public float SuspensionDistance;
//        public float SuspensionSpring;
//        public float SuspensionTargetPosition;
//
//
//        public float SprungMass;
//        public float ForceDistance;
//
//        public float AngularVelocity;
//        public float AngularPosition;
//        public float RotationAngle;
//
//        public float RotationSpeed;
//
//        public float CorrectedRotationSpeed;
//        public float Jounce;
//
//        public float TireLowSideSpeedTimers;
//        public float TireLowforwardSpeedTimers;
//        public bool Grounded;
//
//        public Vector3 Point;
//
//        public Vector3 Normal;
//        public Vector3 ForwardDir;
//
//        public Vector3 SidewaysDir;
//        public float Force;
//
//        public float ForwardSlip;
//        public float SidewaysSlip;
    }

    public struct ShipSteerable
    {
//        public bool Submerged;
//        public float Rpm;
//        public float SpinVelocity;
        public float Angle;
    }

    public struct VehicleSteerable
    {
        public CarSteerable Car;
        public ShipSteerable Ship;
    }

    public struct VehicleBody
    {
        public Vector3 Position;
        public Quaternion Rotation;
//        public Vector3 Body2WorldPosition;
//        public Quaternion Body2WorldRotation;
        public Vector3 LinearVelocity;
        public Vector3 AngularVelocity;

//        public int ContactCount;

        public bool Crashed;

        public bool IsSleeping;
//        public float WakeCounter;
    }

    public class VehicleCmd : BaseRefCounter, IVehicleCmd
    {
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(VehicleCmd)){}
            public override object MakeObject()
            {
                return new VehicleCmd();
            }
        }
        public static VehicleCmd Allocate()
        {
            return ObjectAllocatorHolder<VehicleCmd>.Allocate();
        }

        public int Seq { get; set; }

        public int PlayerId { get; set; }
        public int VehicleId { get; set; }

        public float MoveHorizontal { get; set; }

        public float MoveVertical { get; set; }

        public bool IsSpeedup { get; set; }

        public bool IsHandbrake { get; set; }
        public bool IsHornOn { get; set; }

        public bool IsLeftShift { get; set; }
        public bool IsRightShift { get; set; }
        public bool IsStunt { get; set; }

        public int VehicleType;
        public VehicleBody Body;

        public int SteerableCount;
        public VehicleSteerable[] Steerables = new VehicleSteerable[4];

        public int ExecuteTime { get; set; }

        public int CmdSeq { get; set; } //cmd count 

        protected override void OnCleanUp()
        {
            ObjectAllocatorHolder<VehicleCmd>.Free(this);
        }

        /// <summary>
        /// 只拷贝根据输入获取的内容
        /// </summary>
        /// <param name="cmd"></param>
        public void CopyInputTo(VehicleCmd cmd)
        {
            cmd.MoveVertical = MoveVertical;
            cmd.MoveHorizontal = MoveHorizontal;
            cmd.IsHandbrake = IsHandbrake;
            cmd.IsHornOn = IsHornOn;
            cmd.IsSpeedup = IsSpeedup;

            cmd.IsLeftShift = IsLeftShift;
            cmd.IsRightShift = IsRightShift;
            cmd.IsStunt = IsStunt;
        }

        public void ResetInput()
        {
            MoveVertical = 0;
            MoveHorizontal = 0;
            IsHandbrake = false;
            IsHornOn = false; 
            IsSpeedup = false;

            IsLeftShift = false;
            IsRightShift = false;
            IsStunt = false;
        }

        public override string ToString()
        {
            return string.Format("{0}, Seq: {1}, MoveHorizontal: {2}, MoveVertical: {3}", "VehicleCmd", Seq, MoveHorizontal, MoveVertical);
        }
    }
}
