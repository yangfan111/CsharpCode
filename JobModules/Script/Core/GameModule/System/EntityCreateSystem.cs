using System.Collections.Generic;
using Core.GameModule.Interface;
using Core.GameModule.Module;

namespace Core.GameModule.System
{
    public class EntityCreateSystem : AbstractFrameworkSystem<IEntityInitSystem>
    {
        private IList<IEntityInitSystem> _systems;

        public EntityCreateSystem(IGameModule module)
        {
            _systems = module.EntityInitSystems;
            Init();
        }

        public override IList<IEntityInitSystem> Systems
        {
            get { return _systems; }
        }

        public override void SingleExecute(IEntityInitSystem system)
        {
            system.OnEntityInit();
        }
    }
}
