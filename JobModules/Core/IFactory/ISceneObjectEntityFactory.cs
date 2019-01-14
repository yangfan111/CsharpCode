using System;
using System.Collections.Generic;
using Assets.XmlConfig;
using Core.Bag;
using Entitas;
using UnityEngine;

namespace Core
{
    public interface IMapObjectEntityFactory
    {
        IEntity CreateDoor(int objectId,
            GameObject gameObject, Action<object> detachCallback);

        IEntity CreateDestructibleObject(int objectId,
            GameObject gameObject, Action<object> detachCallback);

        IEntity CreateGlassyObject(int objectId, GameObject gameObject, Action<object> brokenCallback);
    }
    
    public interface ISceneObjectEntityFactory
    {
        List<int> FreeCastEntityToDestoryList { get; }

        IEntity CreateSimpleEquipmentEntity(
            ECategory category,
            int id,
            int count,
            Vector3 position);

        IEntity CreateWeaponEntity(WeaponInfo weaponInfo, Vector3 position);

        IEntity CreateCastEntity(Vector3 position, float size, int key, string tip);

        void DestroyEquipmentEntity(int entityKey);

        IEntity CreateDropWeaponEntity(WeaponInfo weaponInfo, Vector3 position, int lifeTime);
    }
}
