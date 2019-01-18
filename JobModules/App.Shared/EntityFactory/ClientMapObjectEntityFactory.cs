using System;
using App.Shared.Components;
using Core.GameTime;
using Entitas;
using UnityEngine;

namespace App.Shared.EntityFactory
{
    public class ClientMapObjectEntityFactory:ServerMapObjectEntityFactory
    {
        public ClientMapObjectEntityFactory(MapObjectContext mapObjectContext, IEntityIdGenerator entityIdGenerator) :
            base(mapObjectContext, entityIdGenerator)
        {
        }

        public override IEntity CreateDoor(string objectId,
            GameObject gameObject, Action<object> detachCallback)
        {
            return CreateDoorInternal(objectId, gameObject, detachCallback, null);
        }
        
        public override IEntity CreateDestructibleObject(string objectId,
            GameObject gameObject, Action<object> detachCallback)
        {
            return CreateDestructibleObjectInternal(objectId, gameObject, detachCallback, null, true);
        }
    }
}