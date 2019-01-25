using App.Protobuf;
using App.Shared.Components.Vehicle;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using XmlConfig;

namespace App.Shared.Network
{
    public class VehicleCmdMessageConverter
    {
        public static ReusableList<IVehicleCmd> FromProtoBuf(VehicleCmdMessage message)
        {
            ReusableList<IVehicleCmd> list = ReusableList<IVehicleCmd>.Allocate();
            var count = message.VehicleCmdList.Count;

            for (int i = 0; i < count; ++i)
            {
                var item = message.VehicleCmdList[i];

                var cmd = VehicleCmd.Allocate();
                {
                    cmd.Seq = item.Seq;
                    cmd.PlayerId = item.PlayerId;
                    cmd.VehicleId = item.VehicleId;
                    cmd.MoveHorizontal = item.MoveHorizontal;
                    cmd.MoveVertical = item.MoveVertical;
                    cmd.IsSpeedup = item.IsSpeedup;
                    cmd.IsHandbrake = item.IsHandbrake;
                    cmd.IsHornOn = item.IsHornOn;
                    cmd.IsStunt = item.IsStunt;
                    cmd.IsLeftShift = item.IsLeftShift;
                    cmd.IsRightShift = item.IsRightShift;

                    cmd.ExecuteTime = item.ExecuteTime;

                    cmd.CmdSeq = item.CmdSeq;

                    cmd.Body.Position = Vector3Converter.ProtobufToUnityVector3(item.Body.Position);
                    cmd.Body.Rotation = Vector3Converter.ProtobufToUnityQuaternion(item.Body.Rotation);
//                    cmd.Body.Body2WorldPosition = Vector3Converter.ProtobufToUnityVector3(item.Body.Body2WorldPosition);
//                    cmd.Body.Body2WorldRotation = Vector3Converter.ProtobufToUnityQuaternion(item.Body.Body2WorldRotation);
                    cmd.Body.LinearVelocity = Vector3Converter.ProtobufToUnityVector3(item.Body.LinearVelocity);
                    cmd.Body.AngularVelocity = Vector3Converter.ProtobufToUnityVector3(item.Body.AngularVelocity);
 //                   cmd.Body.ContactCount = item.Body.ContactCount;
                    cmd.Body.Crashed = item.Body.Crashed;
                    cmd.Body.IsSleeping = item.Body.IsSleeping;

                    var carSteerableCount = item.CarSteerables.Count;
                    for (int j = 0; j < carSteerableCount; ++j)
                    {
                        var steerable = cmd.Steerables[j].Car;
                        var steerableItem = item.CarSteerables[j];
                        steerable.Position = Vector3Converter.ProtobufToUnityVector3(steerableItem.Position);
                        steerable.Rotation = Vector3Converter.ProtobufToUnityQuaternion(steerableItem.Rotation);
                        steerable.Angle = steerableItem.Angle;
//                        steerable.Grounded = steerableItem.Grounded;
//                        steerable.GroundedOnTerrain = steerableItem.GroundedOnTerrain;

//                        steerable.ColliderSteerAngle = steerableItem.ColliderSteerAngle;
//                        steerable.SteerAngle = steerableItem.SteerAngle;
//                        steerable.MotorTorque = steerableItem.MotorTorque;
//
//                        steerable.SuspensionDistance = steerableItem.SuspensionDistance;
//                        steerable.SuspensionSpring = steerableItem.SuspensionSpring;
//                        steerable.SuspensionTargetPosition = steerableItem.SuspensionTargetPosition;
//
//                        steerable.SprungMass = steerableItem.SprungMass;
//                        steerable.ForceDistance = steerableItem.ForceDistance;
//
//                        steerable.AngularVelocity = steerableItem.AngularVelocity;
//                        steerable.AngularPosition = steerableItem.AngularPosition;
//                        steerable.RotationAngle = steerableItem.RotationAngle;
//
//                        steerable.RotationSpeed = steerableItem.RotationSpeed;
//                        steerable.CorrectedRotationSpeed = steerableItem.CorrectedRotationSpeed;
//                        steerable.Jounce = steerableItem.Jounce;
//
//                        steerable.TireLowSideSpeedTimers = steerableItem.TireLowSideSpeedTimers;
//                        steerable.TireLowforwardSpeedTimers = steerableItem.TireLowforwardSpeedTimers;
//
//                        steerable.Grounded = steerableItem.Grounded;
//                        steerable.Point = Vector3Converter.ProtobufToUnityVector3(steerableItem.Point);
//                        steerable.Normal = Vector3Converter.ProtobufToUnityVector3(steerableItem.Normal);
//                        steerable.ForwardDir = Vector3Converter.ProtobufToUnityVector3(steerableItem.ForwardDir);
//                        steerable.SidewaysDir = Vector3Converter.ProtobufToUnityVector3(steerableItem.SidewaysDir);
//                        steerable.Force = steerableItem.Force;
//
//                        steerable.ForwardSlip = steerableItem.ForwardSlip;
//                        steerable.SidewaysSlip = steerableItem.SidewaysSlip;

                        cmd.Steerables[j].Car = steerable;
                    }

                    var shipSteerableCount = item.ShipSteerables.Count;
                    for (int j = 0; j < shipSteerableCount; ++j)
                    {
                        var steerable = cmd.Steerables[j].Ship;
                        var steerableItem = item.ShipSteerables[j];

                        steerable.Angle = steerableItem.Angle;
//                        steerableItem.Rpm = steerableItem.Rpm;
//                        steerableItem.SpinVelocity = steerableItem.SpinVelocity;
//                        steerableItem.Submerged = steerableItem.Submerged;

                        cmd.Steerables[j].Ship = steerable;
                    }

                    if (carSteerableCount > shipSteerableCount)
                    {
                        cmd.VehicleType = (int)EVehicleType.Car;
                        cmd.SteerableCount = carSteerableCount;
                    }
                    else
                    {
                        cmd.VehicleType = (int) EVehicleType.Ship;
                        cmd.SteerableCount = shipSteerableCount;
                    }
                };

                list.Value.Add(cmd);
            }
            

            return list;
        }

        public static void ToProtoBuf(VehicleCmdMessage msg, ReusableList<IVehicleCmd> list)
        {
            foreach (var cmd in list.Value)
            {
                var item = VehicleCmdItem.Allocate();

                item.Seq = cmd.Seq;
                item.PlayerId = cmd.PlayerId;
                item.VehicleId = cmd.VehicleId;
                item.MoveHorizontal = cmd.MoveHorizontal;
                item.MoveVertical = cmd.MoveVertical;
                item.IsSpeedup = cmd.IsSpeedup;
                item.IsHandbrake = cmd.IsHandbrake;
                item.ExecuteTime = cmd.ExecuteTime;
                item.CmdSeq = cmd.CmdSeq;
                item.IsHornOn = cmd.IsHornOn;
                item.IsStunt = cmd.IsStunt;
                item.IsLeftShift = cmd.IsLeftShift;
                item.IsRightShift = cmd.IsRightShift;
                
                var vehicleCmd = (VehicleCmd) cmd;
                item.Body = VehicleBodyItem.Allocate();
                item.Body.Position = Vector3Converter.UnityToProtobufVector3(vehicleCmd.Body.Position);
                item.Body.Rotation = Vector3Converter.UnityToProtobufQuaternion(vehicleCmd.Body.Rotation);
                item.Body.LinearVelocity = Vector3Converter.UnityToProtobufVector3(vehicleCmd.Body.LinearVelocity);
                item.Body.AngularVelocity = Vector3Converter.UnityToProtobufVector3(vehicleCmd.Body.AngularVelocity);
                item.Body.Crashed = vehicleCmd.Body.Crashed;
                item.Body.IsSleeping = vehicleCmd.Body.IsSleeping;

                if (vehicleCmd.VehicleType == (int) EVehicleType.Car)
                {
                    for (int i = 0; i < vehicleCmd.SteerableCount; ++i)
                    {
                        var steerableItem = CarSteerableItem.Allocate();
                        var steerable = vehicleCmd.Steerables[i].Car;
                        steerableItem.Position = Vector3Converter.UnityToProtobufVector3(steerable.Position);
                        steerableItem.Rotation = Vector3Converter.UnityToProtobufQuaternion(steerable.Rotation);
                        steerableItem.Angle = steerable.Angle;
                        item.CarSteerables.Add(steerableItem);
                    }
                }
                else
                {
                    for (int i = 0; i < vehicleCmd.SteerableCount; ++i)
                    {
                        var steerableItem = ShipSteerableItem.Allocate();
                        var steerable = vehicleCmd.Steerables[i].Ship;

                        steerableItem.Angle = steerable.Angle;
                        item.ShipSteerables.Add(steerableItem);
                    }
                }

                

                msg.VehicleCmdList.Add(item);
            }
        }
    }
}
