using System.IO;
using Core.Event;
using Core.ObjectPool;
using Entitas;
using UnityEngine;
using App.Shared.Components.Serializer;
using Core.Utils;

namespace App.Shared.Player.Events
{
    public class AudioEvent : IEvent
    {
        public AudioGrp_Footstep    footstepState;
        public AudioGrp_FootMatType footMatType;

        public Vector3 relatedPos;

        public class ObjcetFactory : CustomAbstractObjectFactory
        {
            public ObjcetFactory() : base(typeof(AudioEvent))
            {
            }

            public override object MakeObject()
            {
                return new AudioEvent();
            }
        }

        public AudioEvent()
        {
        }

        public EEventType EventType
        {
            get { return EEventType.BroadcastAudio; }
        }

        public bool IsRemote { get; set; }

        public void ReadBody(BinaryReader reader)
        {
            footstepState = (AudioGrp_Footstep) FieldSerializeUtil.Deserialize((byte) 0, reader);
            footMatType   = (AudioGrp_FootMatType) FieldSerializeUtil.Deserialize((byte) 0, reader);
            relatedPos    = FieldSerializeUtil.Deserialize(relatedPos, reader);
        }

        public void WriteBody(MyBinaryWriter writer)
        {
            FieldSerializeUtil.Serialize((byte) footstepState, writer);
            FieldSerializeUtil.Serialize((byte) footMatType, writer);
            FieldSerializeUtil.Serialize(relatedPos, writer);
        }

        public void RewindTo(IEvent value)
        {
            AudioEvent right = value as AudioEvent;

            relatedPos    = right.relatedPos;
            footstepState = right.footstepState;
            footMatType = right.footMatType;

        }
    }

    public class AudioEventHandler : DefaultEventHandler
    {
        public override EEventType EventType
        {
            get { return EEventType.BroadcastAudio; }
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
            //  logger.Info("Wise step come in");
            if ((entity as PlayerEntity).isFlagSelf)
            {
                return;
            }
            //  var controller = (entity as PlayerEntity).AudioController();
            AudioEvent audioEvent = e as AudioEvent;
            GameAudioMedia.PlayStepEnvironmentAudio(audioEvent);

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
            return (entity as PlayerEntity) != null;
        }
    }
}