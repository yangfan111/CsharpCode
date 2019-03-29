using Core.Event;
using Core.ObjectPool;
using Entitas;

namespace App.Shared.Player.Events
{
    public class PullBoltEvent : DefaultEvent
    {
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(PullBoltEvent)){}
            public override object MakeObject()
            {
                return new PullBoltEvent();
            }

        }
       public PullBoltEvent()
        {
        }

        public override EEventType EventType
        {
            get { return EEventType.PullBolt; }
        }

      
    }
    public class PullPoltEventHandler:DefaultEventHandler
    {
        public override EEventType EventType
        {
            get { return EEventType.PullBolt; }
        }

       

        public override void DoEventClient( Entitas.IContexts contexts, IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;

            playerEntity.WeaponController().AddAuxEffect(XmlConfig.EClientEffectType.PullBolt);
         //   playerEntity.WeaponController().PlayPullBoltAudio();


        }

     

        public override bool ClientFilter(IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            return playerEntity.WeaponController().EffectList != null;
        }
    }
}