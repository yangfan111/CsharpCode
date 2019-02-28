using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.GameModule.Module;
using Core.Utils;
using Utils.Singleton;

namespace Core.GameModule.System
{
    public class RenderSystem : AbstractFrameworkSystem<IRenderSystem>
    {
        private IList<IRenderSystem> _systems;

        public RenderSystem(IGameModule module)
        {
            _systems = module.RenderSystems;
            Init();
        }


        public override IList<IRenderSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IRenderSystem system)
        {
            system.OnRender();
            
        }
        public override void Execute()
        {
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.Render);
                base.Execute();
            }
            finally {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.Render);
            }
           
        }
      

    }
}
