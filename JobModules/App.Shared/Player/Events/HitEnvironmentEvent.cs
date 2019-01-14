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
    public class HitEnvironmentEvent : IEvent
    {
        
      
        public Vector3 Offset;
        public Vector3 HitPoint;
        public EEnvironmentType EnvironmentType;
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(HitEnvironmentEvent)){}
            public override object MakeObject()
            {
                return new HitPlayerEvent();
            }

        }
       public HitEnvironmentEvent()
        {
        }

        public  EEventType EventType
        {
            
            get { return EEventType.HitEnvironment; }
        }

        public bool IsRemote { get; set; }
       

        public void ReadBody(BinaryReader reader)
        {
            
            Offset = FieldSerializeUtil.Deserialize(Offset, reader);
            HitPoint= FieldSerializeUtil.Deserialize(HitPoint, reader);
            EnvironmentType = (EEnvironmentType)FieldSerializeUtil.Deserialize((byte) 0, reader);
        }

        public void WriteBody(MyBinaryWriter writer)
        {
           
            FieldSerializeUtil.Serialize(Offset, writer);
          
            FieldSerializeUtil.Serialize(HitPoint, writer);
            FieldSerializeUtil.Serialize((byte)EnvironmentType, writer);
        }

        public void RewindTo(IEvent value)
        {
            HitEnvironmentEvent right = value as HitEnvironmentEvent;
           
            Offset = right.Offset;
            HitPoint = right.HitPoint;
        }
    }
    
    public class HitEnvironmentEventHandler:DefaultEventHandler
    {
        public override EEventType EventType
        {
            get { return EEventType.HitEnvironment; }
        }

      
        public override void DoEventClient( Entitas.IContexts contexts, IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            Contexts c =contexts as Contexts;
            HitEnvironmentEvent ev = e as HitEnvironmentEvent;
            if (playerEntity != null)
            {
                ClientEffectFactory.CreateHitEnvironmentEffect(c.clientEffect, 
                    c.session.commonSession.EntityIdGenerator,
                    ev.HitPoint,
                    ev.Offset,
                    playerEntity.entityKey.Value,ev.EnvironmentType);
            }
        }

      

        public override bool ClientFilter(IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            return playerEntity != null;
        }
     
    }
}