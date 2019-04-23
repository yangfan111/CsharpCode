using Core.EntityComponent;
using Core.Event;
using Core.ObjectPool;
using Entitas;
using UnityEngine;
using App.Shared.GameModules.Weapon;
using App.Shared.Audio;
using Core.Utils;
using WeaponConfigNs;
using System.IO;
using App.Shared.Components.Serializer;

namespace App.Shared.Player.Events
{
    public class FireEvent : IEvent
    {
        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(FireEvent))
            {
            }

            public override object MakeObject()
            {
                return new FireEvent();
            }
        }

        public FireEvent()
        {
        }
        //AudioGrp_ShotMode
        public int audioFireMode;
        public int fireWeaponId;
        public Vector3 audioFirePos;
        public EEventType EventType
        {
            get { return EEventType.Fire; }
        }

        public bool IsRemote { get; set; }


        public void ReadBody(BinaryReader reader)
        {
            audioFireMode = FieldSerializeUtil.Deserialize(audioFireMode, reader);
            fireWeaponId = FieldSerializeUtil.Deserialize(fireWeaponId, reader);
            audioFirePos = FieldSerializeUtil.Deserialize(audioFirePos, reader);
        }

        public void WriteBody(MyBinaryWriter writer)
        {
            FieldSerializeUtil.Serialize(audioFireMode, writer);
            FieldSerializeUtil.Serialize(fireWeaponId, writer);
            FieldSerializeUtil.Serialize(audioFirePos, writer);
        }

        public void RewindTo(IEvent value)
        {
            FireEvent right = value as FireEvent;

            audioFireMode = right.audioFireMode;
            fireWeaponId = right.fireWeaponId;
            audioFirePos = right.audioFirePos;

        }
    }

    public class FireEventHandler : DefaultEventHandler
    {
        public override EEventType EventType
        {
            get { return EEventType.Fire; }
        }

        static LoggerAdapter logger = new LoggerAdapter("FireEventHandler");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contexts"></param>
        /// <param name="entity"></param>
        /// <param name="e"></param>
        public override void DoEventClient(Entitas.IContexts contexts, IEntity entity, IEvent e)
        {
            FireEvent evt = e as FireEvent;
            if (evt != null)
            {
                if ((entity as PlayerEntity).isFlagSelf)
                {
                    return;
                }
                // var weaponGo = (entity as PlayerEntity).appearanceInterface.Appearance.GetWeaponP1InHand();
                GameAudioMedia.PlayWeaponFireAudio(evt.fireWeaponId, evt.audioFirePos, (AudioGrp_ShotMode)evt.audioFireMode);

            }

            // GameAudioMedium.ProcessWeaponAudio(playerEntity,allContexts,(item)=>item.Fire);
            // if (playerEntity.appearanceInterface.Appearance.IsFirstPerson)
            // {

            // }
            // else
            // {
            ////     GameAudioMedium.PerformOnGunFire();
            // }
        }


        public override bool ClientFilter(IEntity entity, IEvent e)
        {
            var playerEntity = entity as PlayerEntity;
            return playerEntity != null;//&& playerEntity.WeaponController().EffectList != null;
        }
    }
}