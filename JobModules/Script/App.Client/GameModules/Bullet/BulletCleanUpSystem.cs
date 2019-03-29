using Core.GameModule.Interface;
using Entitas;

namespace App.Client.GameModules.Bullet
{
    public class BulletCleanUpSystem : IEntityCleanUpSystem
    {
        private IGroup<BulletEntity> _group;
        public BulletCleanUpSystem(BulletContext bulletContext)
        {
            _group = bulletContext.GetGroup(BulletMatcher.AllOf(BulletMatcher.FlagDestroy));
        }

        public void OnEntityCleanUp()
        {
            foreach (var entity in _group)
            {
                if (!entity.hasBulletGameObject)
                {
                    continue;
                }
            }
        }
    }
}
