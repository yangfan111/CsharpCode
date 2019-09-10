using System;
using System.IO;
using App.Shared.Components.Serializer;
using App.Shared.GameModules.Player;
using Core.Components;
using Core.EntityComponent;
using Core.Event;
using Core.ObjectPool;
using Core.Utils;
using Entitas;
using WeaponConfigNs;

namespace App.Shared.Player.Events
{
    public class StatisticsHitPlayerEvent : IEvent
    {
        public EBodyPart bodyPart;

        // public string bulletBaseText;
        // public string bulletRuntimeText;
        public PrecisionsVector3 hitPoint;
        public PrecisionsVector3 posValue;
        public int cmdSeq;
        public int serverTime;
        public string shootKey;
        public string statisticStr;
        public float totalDamage;
        public EntityKey shootTarget;

        public EEventType EventType
        {
            get { return EEventType.HitPlayerStastics; }
        }

        public bool IsRemote { get; set; }

        public void ReadBody(BinaryReader reader)
        {
            bodyPart = (EBodyPart) FieldSerializeUtil.Deserialize((byte) bodyPart, reader);
            // bulletBaseText = FieldSerializeUtil.Deserialize(bulletBaseText, reader);
            //
            statisticStr = FieldSerializeUtil.Deserialize(statisticStr, reader);
            cmdSeq       = FieldSerializeUtil.Deserialize(cmdSeq, reader);
            serverTime   = FieldSerializeUtil.Deserialize(serverTime, reader);
            totalDamage  = FieldSerializeUtil.Deserialize(totalDamage, reader);
            shootKey     = FieldSerializeUtil.Deserialize(shootKey, reader);
            shootTarget = FieldSerializeUtil.Deserialize(shootTarget, reader);
        }

        public void WriteBody(MyBinaryWriter writer)
        {
            FieldSerializeUtil.Serialize((byte) bodyPart, writer);
            // FieldSerializeUtil.Serialize(bulletBaseText, writer);
            //
            FieldSerializeUtil.Serialize(statisticStr, writer);
            FieldSerializeUtil.Serialize(cmdSeq, writer);
            FieldSerializeUtil.Serialize(serverTime, writer);
            FieldSerializeUtil.Serialize(totalDamage, writer);
            FieldSerializeUtil.Serialize(shootKey, writer);
            FieldSerializeUtil.Serialize(shootTarget, writer);

        }

        public void RewindTo(IEvent value)
        {
            StatisticsHitPlayerEvent other = value as StatisticsHitPlayerEvent;
            cmdSeq = other.cmdSeq;
            // bulletRuntimeText = other.bulletRuntimeText;
            statisticStr = other.statisticStr;
            bodyPart     = other.bodyPart;
            totalDamage  = other.totalDamage;
            posValue     = other.posValue;
            hitPoint     = other.hitPoint;
            shootKey     = other.shootKey;
            shootTarget = other.shootTarget;
            serverTime   = other.serverTime;
        }

        public override string ToString()
        {
            return string.Format("cmd:{0},serverTime:{1},totalDamage:{2},HitPart:{3}\n{4}", cmdSeq, serverTime, totalDamage,
                bodyPart,statisticStr);
        }

        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(StatisticsHitPlayerEvent))
            {
            }

            public override object MakeObject()
            {
                return new StatisticsHitPlayerEvent();
            }
        }
    }

    public class StatisticsHitPlayerEventHandler : DefaultEventHandler
    {
        public override EEventType EventType
        {
            get { return EEventType.HitPlayerStastics; }
        }


        public override void DoEventClient(IContexts contexts, IEntity entity, IEvent e)
        {
            throw new NotImplementedException();
        }

        public override void DoEventServer(IContexts contexts, IEntity entity, IEvent e)
        {
            (entity as PlayerEntity).StatisticsController().HandleDispatchedEvt(e as StatisticsHitPlayerEvent, true);
            // var playerEntity = entity as PlayerEntity;
            // if (playerEntity != null)
            // {
            //     var v = EventInfos.Instance.Allocate(e.EventType, true);
            //     v.RewindTo(e);
            //     playerEntity.remoteEvents.Events.AddEvent(v);
            // }
        }

        public override bool ClientFilter(IEntity entity, IEvent e)
        {
            return false;
        }
    }
}