using System.IO;
using App.Shared.Components.Serializer;
using App.Shared.EntityFactory;
using Core.Components;
using Core.EntityComponent;
using Core.Event;
using Core.ObjectPool;
using Core.Utils;
using Entitas;
using UnityEngine;
using WeaponConfigNs;

namespace App.Shared.Player.Events
{
    public class HitPlayerEvent : IEvent
    {
        public EntityKey Target;
        public Vector3   Offset;
        public byte      HitBodyPart;
        public FixedVector3   HitPoint;
        public int       HitAudioId;

        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(HitPlayerEvent))
            {
            }

            public override object MakeObject()
            {
                return new HitPlayerEvent();
            }
        }

        public EEventType EventType
        {
            get { return EEventType.HitPlayer; }
        }

        public bool IsRemote { get; set; }

        public void ReadBody(BinaryReader reader)
        {
            Target = FieldSerializeUtil.Deserialize(Target, reader);
            Offset = FieldSerializeUtil.Deserialize(Offset, reader);

            HitPoint    = FieldSerializeUtil.Deserialize(HitPoint, reader);
            HitAudioId  = FieldSerializeUtil.Deserialize(HitAudioId, reader);
            HitBodyPart = FieldSerializeUtil.Deserialize(HitBodyPart, reader);
        }

        public void WriteBody(MyBinaryWriter writer)
        {
            FieldSerializeUtil.Serialize(Target, writer);
            FieldSerializeUtil.Serialize(Offset, writer);

            FieldSerializeUtil.Serialize(HitPoint, writer);
            FieldSerializeUtil.Serialize(HitAudioId, writer);
            FieldSerializeUtil.Serialize(HitBodyPart, writer);
        }

        public void RewindTo(IEvent value)
        {
            HitPlayerEvent right = value as HitPlayerEvent;
            Target      = right.Target;
            Offset      = right.Offset;
            HitPoint    = right.HitPoint;
            HitAudioId  = right.HitAudioId;
            HitBodyPart = right.HitBodyPart;
        }
    }

    public class HitPlayerEventHandler : DefaultEventHandler
    {
        public override EEventType EventType
        {
            get { return EEventType.HitPlayer; }
        }


        public override void DoEventClient(Entitas.IContexts contexts, IEntity entity, IEvent e)
        {
            var            playerEntity = entity as PlayerEntity;
            Contexts       c            = contexts as Contexts;
            HitPlayerEvent ev           = e as HitPlayerEvent;
            if (playerEntity != null)
                ClientEffectFactory.CreateHitPlayerEffect(c,playerEntity.entityKey.Value, ev);
        }


        public override bool ClientFilter(IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            return playerEntity != null;
        }
    }
}