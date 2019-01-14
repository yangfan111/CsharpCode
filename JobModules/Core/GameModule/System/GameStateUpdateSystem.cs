using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.GameModule.Interface;
using Core.GameModule.Module;

namespace Core.GameModule.System
{
    public class GameStateUpdateSystem : AbstractFrameworkSystem<IGameStateUpdateSystem>
    {
        private IList<IGameStateUpdateSystem> _systems;

        enum Stage
        {
            Update,
            SendGameEvents,
            ProcessGameEvents
        };

        private Stage _stage;
        public GameStateUpdateSystem(IGameModule module)
        {
            _systems = module.GameStateUpdateSystems;
            Init();
        }

        public override IList<IGameStateUpdateSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IGameStateUpdateSystem system)
        {
            if (_stage == Stage.Update)
            {
                system.Update();
            }
            else if (_stage == Stage.SendGameEvents)
            {
                system.SendGameEvents();
            }
            else if (_stage == Stage.ProcessGameEvents)
            {
                system.ProcessGameEvents();
            }
            
        }

        public override void Execute()
        {
            ExecuteStage(Stage.Update);
            ExecuteStage(Stage.SendGameEvents);
            ExecuteStage(Stage.ProcessGameEvents);
        }

        private void ExecuteStage(Stage stage)
        {
            _stage = stage;

            int len = Systems.Count;
            for (int i = 0; i < len; i++)
            {
                var module = Systems[i];
                SingleExecute(module);
            }
        }
    }
}
