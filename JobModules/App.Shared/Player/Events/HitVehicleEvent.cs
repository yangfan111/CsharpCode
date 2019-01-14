using System.IO;
using App.Shared.Components.Serializer;
using App.Shared.EntityFactory;
using Core.EntityComponent;
using Core.Event;
using Core.ObjectPool;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Shared.Player.Events
{
    public class HitVehicleEvent : IEvent
    {
        
        public EntityKey Target;
        public Vector3 Offset;
        public Vector3 Normal;
        public Vector3 HitPoint;
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(HitVehicleEvent)){}
            public override object MakeObject()
            {
                return new HitVehicleEvent();
            }

        }
       public HitVehicleEvent()
        {
        }

        public  EEventType EventType
        {
            get { return EEventType.HitVehicle; }
        }

        public bool IsRemote { get; set; }
        public void ReadBody(BinaryReader reader)
        {
            Target= FieldSerializeUtil.Deserialize(Target, reader);
            Offset=FieldSerializeUtil.Deserialize(Offset, reader);
            Normal=FieldSerializeUtil.Deserialize(Normal, reader);
            HitPoint=FieldSerializeUtil.Deserialize(HitPoint, reader);
        }

        public void WriteBody(MyBinaryWriter writer)
        {
            FieldSerializeUtil.Serialize(Target, writer);
            FieldSerializeUtil.Serialize(Offset, writer);
            FieldSerializeUtil.Serialize(Normal, writer);
            FieldSerializeUtil.Serialize(HitPoint, writer);
        }

        public void RewindTo(IEvent value)
        {
            HitVehicleEvent right = value as HitVehicleEvent;
            Target = right.Target;
            Offset = right.Offset;
            Normal = right.Normal;
            HitPoint = right.HitPoint;
        }
    }
    
    public class HitVehicleEventHandler:DefaultEventHandler
    {
        public override EEventType EventType
        {
            get { return EEventType.HitVehicle; }
        }

      
        public override void DoEventClient( Entitas.IContexts contexts, IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            Contexts c =contexts as Contexts;
            HitVehicleEvent ev = e as HitVehicleEvent;
            if (playerEntity != null)
            {
             
                ClientEffectFactory.CreateHitVehicleEffect(c.clientEffect, 
                    c.session.commonSession.EntityIdGenerator,
                    ev.HitPoint,
                    playerEntity.entityKey.Value,
                    ev.Target,
                    ev.Offset,
                    ev.Normal);
            }
        }

      

        public override bool ClientFilter(IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            return playerEntity != null;
        }
     
    }
}