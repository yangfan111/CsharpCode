using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.GameModule.Module;
using Core.Utils;

namespace Core.GameModule.System
{
    public class PhysicsInitSystem : AbstractFrameworkSystem<IPhysicsInitSystem>
    {
        private IList<IPhysicsInitSystem> _systems;

        public PhysicsInitSystem(IGameModule module)
        {
            _systems = module.PhysicsInitSystems;
            Init();
        }

        public override IList<IPhysicsInitSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IPhysicsInitSystem system)
        {
            system.OnInit();
        }
       
    }
}
