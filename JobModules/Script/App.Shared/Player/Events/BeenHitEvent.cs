using System.IO;
using App.Shared.Components.Serializer;
using Core.EntityComponent;
using Core.Event;
using Core.Utils;
using Entitas;

namespace App.Shared.Player.Events
{
    public class BeenHitEvent:IEvent
    {
        public EntityKey Target;
        public int UniqueId;
        public int TriggerTime;
        
        public EEventType EventType
        {
            get { return EEventType.BeenHit; }
        }

        public override string ToString()
        {
            return string.Format("Target: {0}, UniqueId: {1}, TriggerTime: {2}, EventType: {3}, IsRemote: {4}", Target, UniqueId, TriggerTime, EventType, IsRemote);
        }

        public bool IsRemote { get; set; }
        public void ReadBody(BinaryReader reader)
        {
            Target = FieldSerializeUtil.Deserialize(Target, reader);
            UniqueId = FieldSerializeUtil.Deserialize(UniqueId, reader);
            TriggerTime = FieldSerializeUtil.Deserialize(TriggerTime, reader);

        }

        public void WriteBody(MyBinaryWriter writer)
        {
            FieldSerializeUtil.Serialize(Target, writer);
            FieldSerializeUtil.Serialize(UniqueId, writer);
            FieldSerializeUtil.Serialize(TriggerTime, writer);
        }

        public void RewindTo(IEvent value)
        {
            BeenHitEvent right = value as BeenHitEvent;
            Target = right.Target;
            UniqueId = right.UniqueId;
            TriggerTime = right.TriggerTime;
        }
    }

    public class BeenHitEventHandler : DefaultEventHandler
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BeenHitEventHandler));
        
        public override void DoEventServer(IContexts contexts, IEntity entity, IEvent e)
        {
            Contexts c =contexts as Contexts;
            BeenHitEvent ev = e as BeenHitEvent;
            var playerEntity = c.player.GetEntityWithEntityKey(ev.Target);

            if (playerEntity != null)
            {
                //由于remoteEvents是playback的，命令不会发送给受击者,让受击者调用受伤动画
//                var v = EventInfos.Instance.Allocate(e.EventType, true);
//                v.RewindTo(e);
//                playerEntity.remoteEvents.Events.AddEvent(v);
//                Logger.InfoFormat("server add to player:{0} remoteEvents:{1}", playerEntity.entityKey.Value, e.GetType());
            }
            else
            {
                Logger.WarnFormat("entity:{0} is not found in BeenHitEventHandler.DoEventServer", ev.Target);
            }
        }

        public override EEventType EventType
        {
            get { return EEventType.BeenHit; }
        }

        public override void DoEventClient(IContexts contexts, IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            Contexts c =contexts as Contexts;
            BeenHitEvent ev = e as BeenHitEvent;
            var playerEntityTarget = c.player.GetEntityWithEntityKey(ev.Target);
            if (playerEntity != null)
            {
                //服务器下发的被击中者调用受击命令
                if (playerEntity.entityKey.Value == ev.Target && playerEntity.hasStateInterface)
                {
                    Logger.DebugFormat("self:{0}, invoke , CanBeenHit:{1}", ev.Target, playerEntity.stateInterface.State.CanBeenHit());

                    if (playerEntity.stateInterface.State.CanBeenHit())
                    {
                        playerEntity.stateInterface.State.BeenHit();
                    }
                }
                //自己预测击中的受击者命令,强制使受击者播放受击动画
                else
                {
                    if (playerEntityTarget != null && playerEntityTarget.hasOverrideNetworkAnimator)
                    {
                        Logger.DebugFormat("target:{0}, invoke , IsInjuryAnimatorActive", ev.Target,ev.TriggerTime );

                        playerEntityTarget.overrideNetworkAnimator.IsInjuryAnimatorActive = true;
                        playerEntityTarget.overrideNetworkAnimator.InjuryTriigerTime = ev.TriggerTime;
                    }
                }
            }
        }

        public override bool ClientFilter(IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            return playerEntity != null  && (e is BeenHitEvent);
        }
    }
}