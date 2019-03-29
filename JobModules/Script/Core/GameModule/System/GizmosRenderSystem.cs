using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.GameModule.Module;
using Core.Utils;
using Utils.Singleton;

namespace Core.GameModule.System
{
    public class GizmosRenderSystem : AbstractFrameworkSystem<IGizmosRenderSystem>
    {
        private IList<IGizmosRenderSystem> _systems;

        public GizmosRenderSystem(IGameModule module)
        {
            _systems = module.GizmosRenderSystems;
            Init();
        }


        public override IList<IGizmosRenderSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IGizmosRenderSystem system)
        {
            system.OnGizmosRender();
        }

     

    }

    public class OnGuiSystem : AbstractFrameworkSystem<IOnGuiSystem>
    {
        private IList<IOnGuiSystem> _systems;

        public OnGuiSystem(IGameModule module)
        {
            _systems = module.OnGUISystems;
            Init();
        }


        public override IList<IOnGuiSystem> Systems
        {
            get { return _systems; }
        }

       
        public override void SingleExecute(IOnGuiSystem system)
        {
            
            try
            {
                SingletonManager.Get<DurationHelp>().ProfileStart(CustomProfilerStep.GUI);
                system.OnGUI();
            }
            finally
            {
                SingletonManager.Get<DurationHelp>().ProfileEnd(CustomProfilerStep.GUI);
            }
        }


    }

    public class GamePlaySystem : AbstractFrameworkSystem<IGamePlaySystem>
    {
        private IList<IGamePlaySystem> _systems;

        public GamePlaySystem(IGameModule module)
        {
            _systems = module.GamePlaySystems;
            Init();
        }


        public override IList<IGamePlaySystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IGamePlaySystem system)
        {
            system.OnGamePlay();
        }



    }

    public class LateUpdateSystem : AbstractFrameworkSystem<ILateUpdateSystem>
    {
        private IList<ILateUpdateSystem> _systems;

        public LateUpdateSystem(IGameModule module)
        {
            _systems = module.LateUpdateSystems;
            Init();
        }


        public override IList<ILateUpdateSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(ILateUpdateSystem system)
        {
            system.OnLateUpdate();
        }
        



    }
}
