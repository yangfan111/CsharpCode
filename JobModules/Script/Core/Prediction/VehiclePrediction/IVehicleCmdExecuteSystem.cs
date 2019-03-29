using Core.Prediction.VehiclePrediction.Cmd;
using Entitas;

namespace Core.Prediction.VehiclePrediction
{
    /// <summary>
    /// - 从网络同步到底层
    /// - 执行Cmd
    /// - 调用插件Update
    /// - 从底层同步到网络
    /// </summary>
    public interface IVehicleCmdExecuteSystem
    {
        bool IsEntityValid(Entity entity);
        void UpdateSimulationTime(int simulationTime);
        void SyncFromComponent(Entity entity);
        void ExecuteVehicleCmd(Entity entity, IVehicleCmd cmd);
        void FixedUpdate(Entity entity);
        void Update(Entity entity);
        void SyncToComponent(Entity entity, bool forceSync = false);
    }
}