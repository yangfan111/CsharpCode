using System.Linq;
using Core.EntityComponent;
using Entitas;

namespace App.Shared
{
    public static class EContextsExtentsions
    {
        public static Entitas.Entity GetEntityWithEntityKey(this Contexts contexts, EntityKey key)
        {
            switch (key.EntityType)
            {
                case (int) EEntityType.Vehicle:
                    return contexts.vehicle.GetEntityWithEntityKey(key);
                case (int) EEntityType.Player:
                    return contexts.player.GetEntityWithEntityKey(key);
                case (int) EEntityType.Bullet:
                    return contexts.bullet.GetEntityWithEntityKey(key);
                case (int)EEntityType.SceneObject:
                    return contexts.sceneObject.GetEntityWithEntityKey(key);
                case (int)EEntityType.FreeMove:
                    return contexts.freeMove.GetEntityWithEntityKey(key);
                case (int)EEntityType.Throwing:
                    return contexts.throwing.GetEntityWithEntityKey(key);
                case (int)EEntityType.Sound:
                    return contexts.sound.GetEntityWithEntityKey(key);
                case (int)EEntityType.ClientEffect:
                    return contexts.clientEffect.GetEntityWithEntityKey(key);
                case (int)EEntityType.MapObject:
                    return contexts.mapObject.GetEntityWithEntityKey(key);
                case (int)EEntityType.Weapon:
                    return contexts.weapon.GetEntityWithEntityKey(key);
                default:
                    return null;
            }
        }
        
        public static EntityKey[] GetEntitysWithEntityKey(this Contexts contexts, EEntityType type)
        {
            switch (type)
            {
                case  EEntityType.Vehicle:
                    return contexts.vehicle.GetEntities().Select(x =>x.hasEntityKey?x.entityKey.Value:EntityKey.Default).ToArray();
                case  EEntityType.Player:
                    return contexts.player.GetEntities().Select(x =>x.hasEntityKey?x.entityKey.Value:EntityKey.Default).ToArray();
                case  EEntityType.Bullet:
                    return contexts.bullet.GetEntities().Select(x =>x.hasEntityKey?x.entityKey.Value:EntityKey.Default).ToArray();
                case EEntityType.SceneObject:
                    return contexts.sceneObject.GetEntities().Select(x =>x.hasEntityKey?x.entityKey.Value:EntityKey.Default).ToArray();
                case EEntityType.FreeMove:
                    return contexts.freeMove.GetEntities().Select(x =>x.hasEntityKey?x.entityKey.Value:EntityKey.Default).ToArray();
                case EEntityType.Throwing:
                    return contexts.throwing.GetEntities().Select(x =>x.hasEntityKey?x.entityKey.Value:EntityKey.Default).ToArray();
                case EEntityType.MapObject:
                    return contexts.mapObject.GetEntities().Select(x => x.hasEntityKey ? x.entityKey.Value : EntityKey.Default).ToArray();
                default:
                    return new EntityKey[0];
            }
        }
    }
}