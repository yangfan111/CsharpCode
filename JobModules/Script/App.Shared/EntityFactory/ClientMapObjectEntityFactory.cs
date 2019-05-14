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

        public override IEntity CreateDoor(int objectId, GameObject gameObject)
        {
            return CreateDoorInternal(objectId, gameObject, null);
        }
        
        public override IEntity CreateDestructibleObject(int objectId, GameObject gameObject)
        {
            return CreateDestructibleObjectInternal(objectId, gameObject,null, true);
        }
        
    }
}