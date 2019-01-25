using Core.ObjectPool;
using UnityEngine;

namespace Core.Prediction.VehiclePrediction.Cmd
{
    public interface IVehicleCmd : ICmd, IRefCounter
    {
        int PlayerId { get; set; }

        int VehicleId { get; set; }

        float MoveHorizontal { get; set; }

        float MoveVertical { get; set; }

        bool IsSpeedup { get; set; }

        bool IsHandbrake { get; set; }
        bool IsHornOn { get; set; }

        //for motorcycle
        bool IsLeftShift { get; set; }
        bool IsRightShift { get; set; }
        bool IsStunt { get; set; }

        int ExecuteTime { get; set; }

        int CmdSeq { get; set; }
    }

}
