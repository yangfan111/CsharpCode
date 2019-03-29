using Entitas;
using System.Collections.Generic;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="WeaponCleanupSystem" />
    /// </summary>
    public class WeaponCleanupSystem : ReactiveSystem<WeaponEntity>
    {
        private Contexts _contexts;

        public WeaponCleanupSystem(Contexts contexts) : base(contexts.weapon)
        {
            _contexts = contexts;
        }

        protected override void Execute(List<WeaponEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.weaponRuntimeData.Reset();
            }
        }

        protected override bool Filter(WeaponEntity entity)
        {
            return !entity.isFlagDestroy;
        }

        protected override ICollector<WeaponEntity> GetTrigger(IContext<WeaponEntity> context)
        {
            return context.CreateCollector(WeaponMatcher.Active.Removed());
        }
    }
}
