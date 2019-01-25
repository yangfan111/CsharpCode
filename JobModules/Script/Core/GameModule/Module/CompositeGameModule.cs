using System.Collections.Generic;
using Core.Utils;

namespace Core.GameModule.Module
{
    public class CompositeGameModule : GameModule
    {
        private readonly List<IGameModule> _modules = new List<IGameModule>();
        

        public static LoggerAdapter Logger = new LoggerAdapter(typeof(CompositeGameModule));

        public CompositeGameModule AddModule(IGameModule module)
        {
            _modules.Add(module);
            return this;
        }


        public override void Init()
        {
            foreach (var module in _modules)
            {
                module.Init();
            }

           
            foreach (var module in _modules)
            {
                ModuleInitSystems.AddRange(module.ModuleInitSystems);
                EntityInitSystems.AddRange(module.EntityInitSystems);
                PlaybackSystems.AddRange(module.PlaybackSystems);
                UserCmdExecuteSystems.AddRange(module.UserCmdExecuteSystems);
                BeforeUserCmdExecuteSystems.AddRange(module.BeforeUserCmdExecuteSystems);
                
                ResourceLoadSystems.AddRange(module.ResourceLoadSystems);

                GameStateUpdateSystems.AddRange(module.GameStateUpdateSystems);
 
                PhysicsInitSystems.AddRange(module.PhysicsInitSystems);
                PhysicsUpdateSystems.AddRange(module.PhysicsUpdateSystems);
                PhysicsPostUpdateSystems.AddRange(module.PhysicsPostUpdateSystems);

                GizmosRenderSystems.AddRange(module.GizmosRenderSystems);
                RenderSystems.AddRange(module.RenderSystems);

                EntityCleanUpSystems.AddRange(module.EntityCleanUpSystems);

                LateUpdateSystems.AddRange(module.LateUpdateSystems);
                OnGUISystems.AddRange(module.OnGUISystems);
                GamePlaySystems.AddRange(module.GamePlaySystems);
                UiSystems.AddRange(module.UiSystems);
                UiHfrSystems.AddRange(module.UiHfrSystems);
                VehicleCmdExecuteSystems.AddRange(module.VehicleCmdExecuteSystems);
            }

         
        }

    }
}