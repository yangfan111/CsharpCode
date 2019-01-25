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
    public class HitPlayerEvent : IEvent
    {
        
        public EntityKey Target;
        public Vector3 Offset;
      
        public Vector3 HitPoint;
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(HitPlayerEvent)){}
            public override object MakeObject()
            {
                return new HitPlayerEvent();
            }

        }
       public HitPlayerEvent()
        {
        }

        public  EEventType EventType
        {
            
            get { return EEventType.HitPlayer; }
        }

        public bool IsRemote { get; set; }
        public void ReadBody(BinaryReader reader)
        {
            Target=FieldSerializeUtil.Deserialize(Target, reader);
            Offset=FieldSerializeUtil.Deserialize(Offset, reader);
       
            HitPoint=FieldSerializeUtil.Deserialize(HitPoint, reader);
        }

        public void WriteBody(MyBinaryWriter writer)
        {
            FieldSerializeUtil.Serialize(Target, writer);
            FieldSerializeUtil.Serialize(Offset, writer);
          
            FieldSerializeUtil.Serialize(HitPoint, writer);
        }

        public void RewindTo(IEvent value)
        {
            HitPlayerEvent right = value as HitPlayerEvent;
            Target = right.Target;
            Offset = right.Offset;
            HitPoint = right.HitPoint;
        }
    }
    
    public class HitPlayerEventHandler:DefaultEventHandler
    {
        public override EEventType EventType
        {
            get { return EEventType.HitPlayer; }
        }

      
        public override void DoEventClient( Entitas.IContexts contexts, IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            Contexts c =contexts as Contexts;
            HitPlayerEvent ev = e as HitPlayerEvent;
            if (playerEntity != null)
            {
             
                ClientEffectFactory.CreateHitPlayerEffect(c.clientEffect, 
                    c.session.commonSession.EntityIdGenerator,
                    ev.HitPoint,
                    playerEntity.entityKey.Value,
                    ev.Target,
                    ev.Offset);
            }
        }

      

        public override bool ClientFilter(IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            return playerEntity != null;
        }
     
    }
}