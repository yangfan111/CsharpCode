using System;
using System.Collections.Generic;
using App.Shared.Components;
using App.Shared.GameModules.Player;
using App.Shared.Player;
using Assets.App.Server.GameModules.GamePlay.Free;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.cpkf.yyjd.tools.util.math;
using Core;
using Core.EntityComponent;
using Core.Enums;
using Core.Free;
using Core.GameTime;
using Core.Utils;
using Entitas;
using Free.framework;
using UnityEngine;
using Utils.Singleton;
using Object = UnityEngine.Object;

namespace App.Shared.EntityFactory
{
    public class ServerSceneObjectEntityFactory : ISceneObjectEntityFactory
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(ServerSceneObjectEntityFactory));
        private readonly SceneObjectContext _sceneObjectContext;
        private readonly PlayerContext _playerContext;
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

        public ServerSceneObjectEntityFactory(SceneObjectContext sceneObjectContext, PlayerContext playerContext,
            IEntityIdGenerator entityIdGenerator,
            IEntityIdGenerator equipGenerator, ICurrentTime currentTime)
        {
            _sceneObjectContext = sceneObjectContext;
            _playerContext = playerContext;
            _idGenerator = entityIdGenerator;
            _equipGenerator = equipGenerator;
            _currentTime = currentTime;
        }

        public virtual SceneObjectEntity CreateSceneObjectEntity()
        {
            var entity = _sceneObjectContext.CreateEntity();
            return entity;
        }
        public virtual IEntity CreateSceneAudioEmitterEntity(Vector3 p, EntityKey entityKey)
        {
            if (SharedConfig.IsOffline)
            {
                #if UNITY_EDITOR
                var playerEntity = _playerContext.GetEntityWithEntityKey(entityKey);
                
                Object res = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>("Assets/Assets/CoreRes/Sound/TestModel/AudioEmitter.prefab");
                var clone = GameObject.Instantiate(res) as GameObject;
                var  emitter = clone.AddComponent<AudioEmitterEditor>();
                emitter.transform.position = new Vector3(p.x,p.y+2f,p.z);
                emitter.LookAtListener = true;
                emitter.SetListener(playerEntity.appearanceInterface.Appearance.CharacterP3);
                #endif
            }
            return null;
        }
        public virtual IEntity CreateSimpleEquipmentEntity(
            ECategory category,
            int id,
            int count,
            Vector3 position)
        {
            var entity = _sceneObjectContext.CreateEntity();
            entity.AddEntityKey(new EntityKey(_equipGenerator.GetNextEntityId(), (short)EEntityType.SceneObject));
            entity.isFlagSyncNonSelf = true;

            entity.AddPosition();
            entity.position.Value = position;
            entity.AddSimpleEquipment(id, count, (int)category);
            if (category == ECategory.Weapon)
            {
                entity.AddWeaponObject();
                entity.weaponObject.ConfigId = id;
            }

            entity.AddFlagImmutability(_currentTime.CurrentTime);
            return entity;
        }

        public virtual IEntity CreateSceneWeaponObjectEntity(WeaponScanStruct weaponScan, Vector3 position)
        {
            var entity = _sceneObjectContext.CreateEntity();
            entity.AddEntityKey(new EntityKey(_equipGenerator.GetNextEntityId(), (short)EEntityType.SceneObject));
            entity.isFlagSyncNonSelf = true;
            entity.AddPosition();
            entity.position.Value = position;
            entity.AddWeaponObject();
            entity.weaponObject.GameCopyFrom(weaponScan);
            entity.AddFlagImmutability(_currentTime.CurrentTime);
            return entity;
        }


        public virtual IEntity CreateDropSceneWeaponObjectEntity(WeaponScanStruct weaponScan, Vector3 position, int lifeTime)
        {
            var entity = CreateSceneWeaponObjectEntity(weaponScan, position);
            if (lifeTime > 0)
            {
                var sceneObjectEntity = entity as SceneObjectEntity;
                if (null != sceneObjectEntity)
                {
                    sceneObjectEntity.AddLifeTime(DateTime.Now, lifeTime);
                }
            }
            if (WeaponUtil.IsC4p(weaponScan.ConfigId))
                (entity as SceneObjectEntity).AddCastFlag((int)EPlayerCastState.C4Pickable);
            return entity;
        }

        public virtual void DestroySceneWeaponObjectEntity(EntityKey key)
        {
            var entity = _sceneObjectContext.GetEntityWithEntityKey(key);
            entity.isFlagDestroy = true;
        }

        public virtual IEntity CreateCastEntity(Vector3 position, float size, int key, string tip)
        {
            var entity = _sceneObjectContext.CreateEntity();
            entity.AddEntityKey(new EntityKey(_equipGenerator.GetNextEntityId(), (short)EEntityType.SceneObject));
            entity.AddSimpleCastTarget(key, size, tip);
            entity.AddPosition();
            entity.position.Value = position;
            entity.isFlagSyncNonSelf = true;
            entity.AddFlagImmutability(_currentTime.CurrentTime);
            return entity;
        }
        public virtual IEntity GetSceneEntity(int value)
        {
            return _sceneObjectContext.GetEntityWithEntityKey(new EntityKey(value, (short)EEntityType.SceneObject));
        }

    }
}