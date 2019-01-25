using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.GameModule.Module;

namespace Core.GameModule.System
{
    public class PhysicsUpdateSystem :  AbstractFrameworkSystem<IPhysicsUpdateSystem>
    {
        private IList<IPhysicsUpdateSystem> _systems;

        public PhysicsUpdateSystem(IGameModule module)
        {
            _systems = module.PhysicsUpdateSystems;
            Init();
        }

        public override IList<IPhysicsUpdateSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IPhysicsUpdateSystem system)
        {
            system.Update();
        }
    }

    public class PhysicsPostUpdateSystem : AbstractFrameworkSystem<IPhysicsPostUpdateSystem>
    {
        private IList<IPhysicsPostUpdateSystem> _systems;

        public PhysicsPostUpdateSystem(IGameModule module)
        {
            _systems = module.PhysicsPostUpdateSystems;
            Init();
        }

        public override IList<IPhysicsPostUpdateSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IPhysicsPostUpdateSystem system)
        {
            system.PostUpdate();
        }
    }
}
