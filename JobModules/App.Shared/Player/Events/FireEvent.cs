using Core.EntityComponent;
using Core.Event;
using Core.ObjectPool;
using Entitas;
using UnityEngine;
using Core.Audio;

namespace App.Shared.Player.Events
{
    public class FireEvent : DefaultEvent
    {
       
        public class ObjcetFactory :CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(FireEvent)){}
            public override object MakeObject()
            {
                return new FireEvent();
            }

        }
       public FireEvent()
        {
        }

        public override EEventType EventType
        {
            get { return EEventType.Fire; }
        }

       
    }
    
    public class FireEventHandler:DefaultEventHandler
    {
        public override EEventType EventType
        {
            get { return EEventType.Fire; }
        }

      
        public override void DoEventClient( Entitas.IContexts contexts, IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            if (playerEntity != null)
            {
               
                playerEntity.weaponLogic.WeaponEffect.PlayMuzzleSparkEffect(playerEntity.weaponLogic.State);
                GameAudioMedium.PerformOnGunFire(playerEntity.weaponLogic.State);
            }
        }

      

        public override bool ClientFilter(IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            return playerEntity != null && playerEntity.hasWeaponLogic&&playerEntity.hasWeaponState;
        }
     
    }
}