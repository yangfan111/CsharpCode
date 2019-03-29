using System;
using App.Shared.Components;
using Assets.XmlConfig;
using Core;
using Core.EntityComponent;
using Core.GameTime;
using Entitas;
using UnityEngine;

namespace App.Shared.EntityFactory
{
    public class ClientSceneObjectEntityFactory : ServerSceneObjectEntityFactory
    {
        public ClientSceneObjectEntityFactory(SceneObjectContext sceneObjectContext, PlayerContext playerContext,
            IEntityIdGenerator entityIdGenerator, IEntityIdGenerator equipGenerator, ICurrentTime currentTime) : base(
            sceneObjectContext, playerContext, entityIdGenerator, equipGenerator, currentTime)
        {

        }

        public override IEntity CreateSimpleEquipmentEntity(ECategory category, int id, int count, Vector3 position)
        {
            return null;
        }

        public override void DestroySceneWeaponObjectEntity(EntityKey key)
        {
        }

        public override IEntity CreateSceneWeaponObjectEntity(WeaponScanStruct weaponInfo, Vector3 position)
        {
            return null;
        }

        public override IEntity CreateDropSceneWeaponObjectEntity(WeaponScanStruct weaponKey, Vector3 position, int lifeTime)
        {
            return null;
        }

        public override IEntity CreateCastEntity(Vector3 position, float size, int key, string tip)
        {
            return null;
        }
    }
}
