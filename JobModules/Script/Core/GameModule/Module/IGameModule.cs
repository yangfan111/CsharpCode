using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Prediction.VehiclePrediction;

namespace Core.GameModule.Module
{
    /// <summary>
    /// 按以下顺序执行：
    /// </summary>
    public interface IGameModule 
        


    {
        /**
         * IModuleInitSystem, //只执行一次
        IEntityCreateSystem, // 创建Entity
        
        IUserCmdExecuteSystem, //预测

        IResourceLoadSystem, // 资源加载
        IResourceInitSystem, // 资源加载后初始化。一般是ReactivateResourceInitSystem，根据Collector检测某些资源加载成功，根据Filter确定所有资源加载成功

        IPreRenderSystem, // Render之前调用
        IRenderSystem, // Render

        IEntityPreDestroySystem, // 设置Destroy标志
        IEntityDestroySystem, // 根据Destroy，进行清理
        IEntityPostDestroySystem // 销毁设置了Destroy标志的Entity
    */
        void Init();
        List<IEntityInitSystem> EntityInitSystems { get; }
        List<IModuleInitSystem> ModuleInitSystems { get; }

        List<IPlaybackSystem> PlaybackSystems { get; }
        List<IUserCmdExecuteSystem> UserCmdExecuteSystems { get; }
        List<IVehicleCmdExecuteSystem> VehicleCmdExecuteSystems { get; }

        List<IResourceLoadSystem> ResourceLoadSystems { get; }

        List<IGameStateUpdateSystem> GameStateUpdateSystems { get; }
        
        List<IPhysicsInitSystem> PhysicsInitSystems { get; }
        List<IPhysicsUpdateSystem> PhysicsUpdateSystems { get; }
        List<IPhysicsPostUpdateSystem> PhysicsPostUpdateSystems { get; }

        List<IGizmosRenderSystem> GizmosRenderSystems { get; }
        List<IRenderSystem> RenderSystems { get; }
        List<IEntityCleanUpSystem> EntityCleanUpSystems { get; }

        List<ILateUpdateSystem> LateUpdateSystems { get; }
        List<IOnGuiSystem> OnGUISystems { get; }
        List<IUiSystem> UiSystems { get; }
        List<IUiHfrSystem> UiHfrSystems { get; }
        List<IGamePlaySystem> GamePlaySystems { get; }
        List<IBeforeUserCmdExecuteSystem> BeforeUserCmdExecuteSystems { get;  }
    }
}