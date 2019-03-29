using Core.GameModule.Interface;
using Entitas;

namespace App.Client.GameModules.Throwing
{
    public class ThrowingCleanUpSystem : IEntityCleanUpSystem
    {
        private IGroup<ThrowingEntity> _group;
        public ThrowingCleanUpSystem(ThrowingContext throwingContext)
        {
            _group = throwingContext.GetGroup(ThrowingMatcher.AllOf(ThrowingMatcher.FlagDestroy));
        }

        public void OnEntityCleanUp()
        {
            foreach (var entity in _group)
            {
                if (!entity.hasThrowingGameObject)
                {
                    continue;
                }
            }
        }
    }
}
