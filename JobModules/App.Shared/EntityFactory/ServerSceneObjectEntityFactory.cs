using System;
using System.Collections.Generic;
using App.Shared.WeaponLogic;
using App.Shared.Components;
using App.Shared.GameModules.Common;
using App.Shared.Player;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Bag;
using Core.EntityComponent;
using Core.GameTime;
using Core.SceneTriggerObject;
using Core.Utils;
using Entitas;
using UltimateFracturing;
using UnityEngine;
using Utils.Singleton;

namespace App.Shared.EntityFactory
{
    public class ServerSceneObjectEntityFactory : ISceneObjectEntityFactory
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(ServerSceneObjectEntityFactory));
        private readonly SceneObjectContext _sceneObjectContext;
        protected readonly IEntityIdGenerator _idGenerator;
        private readonly IEntityIdGenerator _equipGenerator;
        private readonly ICurrentTime _currentTime;

        public List<int> FreeCastEntityToDestoryList
        {
            get 
            {
                return _freeCastEntityToDestroyList;
            }
        }
        private readonly List<int> _freeCastEntityToDestroyList = new List<int>();

        public ServerSceneObjectEntityFactory(SceneObjectContext sceneObjectContext, 
            IEntityIdGenerator entityIdGenerator,
            IEntityIdGenerator equipGenerator, ICurrentTime currentTime)
        {
            _sceneObjectContext = sceneObjectContext;
            _idGenerator = entityIdGenerator;
            _equipGenerator = equipGenerator;
            _currentTime = currentTime;
        }

        public virtual SceneObjectEntity CreateSceneObjectEntity()
        {
            var entity = _sceneObjectContext.CreateEntity();
            return entity;
        }

        public virtual IEntity CreateSimpleEquipmentEntity(
            ECategory category,
            int id,
            int count,
            Vector3 position)
        {
            var entity = _sceneObjectContext.CreateEntity();
            entity.AddEntityKey(new EntityKey(_equipGenerator.GetNextEntityId(), (short) EEntityType.SceneObject));
            entity.isFlagSyncNonSelf = true;
            entity.AddPosition(position);
            entity.AddSimpleEquipment(id, count, (int) category);
            entity.AddFlagImmutability(_currentTime.CurrentTime);
            return entity;
        }

        public virtual IEntity CreateWeaponEntity(WeaponInfo weaponInfo, Vector3 position)
        {
            var entity = _sceneObjectContext.CreateEntity();
            entity.AddEntityKey(new EntityKey(_equipGenerator.GetNextEntityId(), (short) EEntityType.SceneObject));
            entity.isFlagSyncNonSelf = true;
            entity.AddPosition(position);
            entity.AddWeapon();
            weaponInfo.ToSceneWeaponComponent(entity.weapon);
            entity.AddFlagImmutability(_currentTime.CurrentTime);
            return entity;
        }

        public virtual IEntity CreateDropWeaponEntity(WeaponInfo weaponInfo, Vector3 position, int lifeTime)
        {
            var entity = CreateWeaponEntity(weaponInfo, position);
            if(null == entity)
            {
                return null;
            }
            if(lifeTime > 0)
            {
                var sceneObjectEntity = entity as SceneObjectEntity;
                if(null != sceneObjectEntity)
                {
                    sceneObjectEntity.AddLifeTime(DateTime.Now, lifeTime);
                }
            }
            if(SingletonManager.Get<WeaponConfigManager>().IsC4(weaponInfo.Id))
            {
                var sceneObjectEntity = entity as SceneObjectEntity;
                if(null != sceneObjectEntity)
                {
                    sceneObjectEntity.AddCastFlag((int)EPlayerCastState.C4Pickable);
                }
            }
            return entity;
        }

        public virtual void DestroyEquipmentEntity(int entityKey)
        {
            var entity =
                _sceneObjectContext.GetEntityWithEntityKey(new EntityKey(entityKey, (short) EEntityType.SceneObject));
            entity.isFlagDestroy = true;
        }

        public virtual IEntity CreateCastEntity(Vector3 position, float size, int key, string tip)
        {
            var entity = _sceneObjectContext.CreateEntity();
            entity.AddEntityKey(new EntityKey(_equipGenerator.GetNextEntityId(), (short) EEntityType.SceneObject));
            entity.AddSimpleCastTarget(key, size, tip);
            entity.AddPosition(position);
            entity.isFlagSyncNonSelf = true;
            entity.AddFlagImmutability(_currentTime.CurrentTime);
            return entity;
        }
    }
}