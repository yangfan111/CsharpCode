using System.IO;
using App.Shared.Components.Serializer;
using App.Shared.EntityFactory;
using Core;
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
        public int ChunkId;
        public bool needEffectEntity;
        public EEnvironmentType EnvironmentType;
        public int HitAudioId;
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(HitEnvironmentEvent))
            {
            }

            public override object MakeObject()
            {
                return new HitEnvironmentEvent();
            }
        }

        public HitEnvironmentEvent()
        {
            needEffectEntity = true;
        }
        

        public EEventType EventType
        {
            get { return EEventType.HitEnvironment; }
        }

        public bool IsRemote { get; set; }


        public void ReadBody(BinaryReader reader)
        {
            Offset = FieldSerializeUtil.Deserialize(Offset, reader);
            HitPoint = FieldSerializeUtil.Deserialize(HitPoint, reader);
            EnvironmentType = (EEnvironmentType) FieldSerializeUtil.Deserialize((byte) 0, reader);
            HitAudioId =  FieldSerializeUtil.Deserialize(HitAudioId, reader);
            needEffectEntity = FieldSerializeUtil.Deserialize((byte) 0, reader)==1;
            ChunkId = FieldSerializeUtil.Deserialize(ChunkId, reader);




        }

        public void WriteBody(MyBinaryWriter writer)
        {
            FieldSerializeUtil.Serialize(Offset, writer);

            FieldSerializeUtil.Serialize(HitPoint, writer);
            FieldSerializeUtil.Serialize((byte) EnvironmentType, writer);
            FieldSerializeUtil.Serialize(HitAudioId, writer);
            FieldSerializeUtil.Serialize(needEffectEntity?(byte)1:(byte)0, writer);
            FieldSerializeUtil.Serialize(ChunkId, writer);


        }

        public void RewindTo(IEvent value)
        {
            HitEnvironmentEvent right = value as HitEnvironmentEvent;

            Offset = right.Offset;
            HitPoint = right.HitPoint;
            HitAudioId = right.HitAudioId;
            EnvironmentType = right.EnvironmentType;
            needEffectEntity = right.needEffectEntity;
            ChunkId = right.ChunkId;
        }
    }

    public class HitEnvironmentEventHandler : DefaultEventHandler
    {
        public override EEventType EventType
        {
            get { return EEventType.HitEnvironment; }
        }


        public override void DoEventClient(Entitas.IContexts contexts, IEntity entity, IEvent e)
        {
            HitEnvironmentEvent hitEvent = e as HitEnvironmentEvent;
            ClientEffectFactory.CreateHitEnvironmentEffect(hitEvent);
        }


        public override bool ClientFilter(IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            return playerEntity != null;
        }
    }
}