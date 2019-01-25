using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.GameModule.Module;

namespace Core.GameModule.System
{
    public sealed class EntityCleanUpSystem : AbstractFrameworkSystem<IEntityCleanUpSystem>
    {
        private IList<IEntityCleanUpSystem> _systems;

        public EntityCleanUpSystem(IGameModule module)
        {
            _systems = module.EntityCleanUpSystems;
            Init();
        }


        public override IList<IEntityCleanUpSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IEntityCleanUpSystem system)
        {
            system.OnEntityCleanUp();
        }
    }
}
