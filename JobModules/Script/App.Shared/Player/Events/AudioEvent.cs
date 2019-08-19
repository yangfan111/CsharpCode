using System;
using System.IO;
using System.Text;
using App.Shared.Audio;
using Core.Event;
using Core.ObjectPool;
using Entitas;
using UnityEngine;
using App.Shared.Components.Serializer;
using Core;
using Core.Components;
using Core.Utils;
using Utils.Singleton;

namespace App.Shared.Player.Events
{
    public class AudioFootstepEvent : AudioDoubleGroupEvent
    {
        public AudioGrp_Footstep FootstepGrp
        {
            get { return (AudioGrp_Footstep) groupOne; }
        }

        public override EEventType EventType
        {
            get { return EEventType.AFootstep; }
        }

        public AudioGrp_FootMatType FootMatType
        {
            get { return (AudioGrp_FootMatType) groupTwo; }
        }

        public void Initialize(AudioGrp_Footstep footstep, AudioGrp_FootMatType matType, Vector3 relatedPos,
                               Vector3           relatedRocation)
        {
            Initialize((byte) footstep, (byte) matType, relatedPos, relatedRocation);
        }
    }

    public abstract class AudioDoubleGroupEvent : AudioEvent
    {
        protected byte groupOne;
        protected byte groupTwo;

        public void Initialize(byte groupOne, byte groupTwo, Vector3 relatedPos, Vector3 relatedRocation)
        {
            this.groupOne = groupOne;
            this.groupTwo = groupTwo;
            Initialize(relatedPos, relatedRocation);
        }

        public override void ReadBody(BinaryReader reader)
        {
            base.ReadBody(reader);
            groupOne = FieldSerializeUtil.Deserialize((byte) 0, reader);
            groupTwo = FieldSerializeUtil.Deserialize((byte) 0, reader);
        }

        public override void RewindTo(AudioEvent right)
        {
            base.RewindTo(right);
            groupOne = ((AudioDoubleGroupEvent) right).groupOne;
            groupTwo = ((AudioDoubleGroupEvent) right).groupTwo;
        }

        public override void WriteBody(MyBinaryWriter writer)
        {
            base.WriteBody(writer);
            FieldSerializeUtil.Serialize((byte) groupOne, writer);
            FieldSerializeUtil.Serialize((byte) groupTwo, writer);
        }
    }

    public class AudioWeaponFireEvent : AudioSimpleGroupEvent
    {
        public AudioGrp_ShotMode ShotMode
        {
            get { return (AudioGrp_ShotMode) groupOne; }
        }

        public override EEventType EventType
        {
            get { return EEventType.AWeaponFire; }
        }

        public int WeaponId;


        public void Initialize(AudioGrp_ShotMode shotMode, int weaponId, Vector3 relatedPos, Vector3 relatedRocation)
        {
            this.WeaponId = weaponId;
            Initialize((byte) shotMode, relatedPos, relatedRocation);
        }

        public override void ReadBody(BinaryReader reader)
        {
            base.ReadBody(reader);
            WeaponId = FieldSerializeUtil.Deserialize(WeaponId, reader);
        }

        public override void RewindTo(AudioEvent right)
        {
            base.RewindTo(right);
            WeaponId = ((AudioWeaponFireEvent) right).WeaponId;
        }

        public override void WriteBody(MyBinaryWriter writer)
        {
            base.WriteBody(writer);
            FieldSerializeUtil.Serialize(WeaponId, writer);
        }
    }
    public class AudioPullboltEvent : AudioSimpleGroupEvent
    {
        public int WeaponId;
        public AudioGrp_Magazine Magazine
        {
            get { return (AudioGrp_Magazine) groupOne; }
        }

        public override EEventType EventType
        {
            get { return EEventType.APullbolt; }
        }

        public void Initialize(AudioGrp_Magazine magazine, int weaponId, Vector3 relatedPos, Vector3 relatedRocation)
        {
            this.WeaponId = weaponId;
            Initialize((byte) magazine, relatedPos, relatedRocation);
        }

        public override void ReadBody(BinaryReader reader)
        {
            base.ReadBody(reader);
            WeaponId = FieldSerializeUtil.Deserialize(WeaponId, reader);
        }

        public override void RewindTo(AudioEvent right)
        {
            base.RewindTo(right);
            WeaponId = ((AudioPullboltEvent) right).WeaponId;
        }

        public override void WriteBody(MyBinaryWriter writer)
        {
            base.WriteBody(writer);
            FieldSerializeUtil.Serialize(WeaponId, writer);
        }
    }
    public class AudioJumpstepEvent : AudioSimpleGroupEvent
    {
        public AudioGrp_FootMatType FootMatType
        {
            get { return (AudioGrp_FootMatType) groupOne; }
        }

        public override EEventType EventType
        {
            get { return EEventType.AJumpstep; }
        }

        public void Initialize(AudioGrp_FootMatType footMatType, Vector3 relatedPos, Vector3 relatedRocation)
        {
            Initialize((byte) footMatType, relatedPos, relatedRocation);
        }
    }
    public class AudioMeleeAtkEvent : AudioSimpleGroupEvent
    {
        public AudioGrp_MeleeAttack MeleeAttack
        {
            get { return (AudioGrp_MeleeAttack ) groupOne; }
        }
        public int eventId;
        public override EEventType EventType
        {
            get { return EEventType.AMeleeAttack; }
        }


     
        public void Initialize(int eventId,int attackType, Vector3 relatedPos, Vector3 relatedRocation)
        {
            this.eventId = eventId;
            Initialize((byte) attackType, relatedPos, relatedRocation);
        }

        public override void ReadBody(BinaryReader reader)
        {
            base.ReadBody(reader);
            eventId = FieldSerializeUtil.Deserialize(eventId, reader);
        }

        public override void RewindTo(AudioEvent right)
        {
            base.RewindTo(right);
            eventId = ((AudioMeleeAtkEvent) right).eventId;
        }

        public override void WriteBody(MyBinaryWriter writer)
        {
            base.WriteBody(writer);
            FieldSerializeUtil.Serialize(eventId, writer);
        }
    }
    public abstract class AudioSimpleGroupEvent : AudioEvent
    {
        protected byte groupOne;

        public void Initialize(byte groupOne, Vector3 relatedPos, Vector3 relatedRocation)
        {
            base.Initialize(relatedPos, relatedRocation);
            this.groupOne = groupOne;
        }

        public override void ReadBody(BinaryReader reader)
        {
            base.ReadBody(reader);
            groupOne = FieldSerializeUtil.Deserialize((byte) 0, reader);
        }

        public override void RewindTo(AudioEvent right)
        {
            base.RewindTo(right);
            groupOne = ((AudioSimpleGroupEvent) right).groupOne;
        }

        public override void WriteBody(MyBinaryWriter writer)
        {
            base.WriteBody(writer);
            FieldSerializeUtil.Serialize(groupOne, writer);
        }
    }

    public class AudioDefaultEvent : AudioEvent
    {
        public int EventId { get; private set; }


        public override EEventType EventType
        {
            get { return EEventType.ADefault; }
        }

        public void Initialize(EAudioUniqueId uniqueId, Vector3 relatedPos, Vector3 relatedRocation)
        {
            Initialize((int) uniqueId, relatedPos, relatedRocation);
        }

        public void Initialize(int eventId, Vector3 relatedPos, Vector3 relatedRocation)
        {
            base.Initialize(relatedPos, relatedRocation);
            this.EventId = eventId;
        }

        public override void ReadBody(BinaryReader reader)
        {
            base.ReadBody(reader);
            EventId = FieldSerializeUtil.Deserialize(EventId, reader);
        }

        public override void RewindTo(AudioEvent right)
        {
            base.RewindTo(right);
            EventId = ((AudioDefaultEvent) right).EventId;
        }

        public override void WriteBody(MyBinaryWriter writer)
        {
            base.WriteBody(writer);
            FieldSerializeUtil.Serialize(EventId, writer);
        }
    }

    public abstract class AudioEvent : IEvent
    {
//        public AudioGrp_Footstep    footstepState;
//        public AudioGrp_FootMatType footMatType;
        public FixedVector3 relatedPos;
        public Vector3 relatedRocation;
        
        public AudioEvent()
        {
        }

        public void Initialize(Vector3 relatedPos, Vector3 relatedRocation)
        {
            this.relatedPos      = relatedPos.ShiftedToFixedVector3();
            this.relatedRocation = relatedRocation;
        }

        public abstract EEventType EventType { get; }


        public bool IsRemote { get; set; }

        public virtual void ReadBody(BinaryReader reader)
        {
//            footstepState = (AudioGrp_Footstep) FieldSerializeUtil.Deserialize((byte) 0, reader);
//            footMatType   = (AudioGrp_FootMatType) FieldSerializeUtil.Deserialize((byte) 0, reader);
            relatedPos      = FieldSerializeUtil.Deserialize(relatedPos, reader);
            relatedRocation = FieldSerializeUtil.Deserialize(relatedRocation, reader);
        }

        public virtual void WriteBody(MyBinaryWriter writer)
        {
//            FieldSerializeUtil.Serialize((byte) footstepState, writer);
//            FieldSerializeUtil.Serialize((byte) footMatType, writer);
            // FieldSerializeUtil.Serialize((byte) subType, writer);
            FieldSerializeUtil.Serialize(relatedPos, writer);
            FieldSerializeUtil.Serialize(relatedRocation, writer);
        }

        public void RewindTo(IEvent value)
        {
            RewindTo(value as AudioEvent);
        }

        public virtual void RewindTo(AudioEvent right)
        {
            //  subType         = right.subType;
            relatedPos      = right.relatedPos;
            relatedRocation = right.relatedRocation;
        }
    }

    public class AudioEventHandler : DefaultEventHandler
    {
        public override EEventType EventType
        {
            get { return EEventType.End; }
        }

        private EEventType[] eventTypes =
            {EEventType.ADefault, EEventType.AWeaponFire, EEventType.AFootstep,EEventType.AJumpstep,EEventType.APullbolt,EEventType.AMeleeAttack};

        public override EEventType[] EventTypes
        {
            get { return eventTypes; }
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
            if (SharedConfig.IsMute)
                return;
            //var controller = (entity as PlayerEntity).AudioController();
            var targetPlayerEntity = entity as PlayerEntity;
            int cmrEntityId = (contexts as Contexts).player.flagSelfEntity.gamePlay.CameraEntityId;
         // foreach(PlayerEntity playerEntity in (contexts as Contexts).player.GetEntities())
            // {
         //    if(playerEntity.gamePlay.CameraEntityId == player.entityKey.Value.EntityId)
            //     {
            //         //TODO
            //         observePlayer = playerEntity;
            //         break;
            //     }
            // }
      
            AudioEvent audioEvent = e as AudioEvent;
        
            //DebugUtil.MyLog("Play other event :"+audioEvent.EventType);
            switch (audioEvent.EventType)
            {
                case EEventType.AFootstep:
                    GameAudioMedia.PlayFootstepAudio(audioEvent as AudioFootstepEvent);
                    break;
                case EEventType.AJumpstep:
                    GameAudioMedia.PlayJumpstepAudio(audioEvent as AudioJumpstepEvent);
                    break;
                case EEventType.AWeaponFire:
                    GameAudioMedia.PlayWeaponFireAudio(audioEvent as AudioWeaponFireEvent);
                    break;
                case EEventType.ADefault:
                    if(cmrEntityId ==targetPlayerEntity.entityKey.Value.EntityId)
                    {
                        float RTPCvalue = 1;
                        GameAudioMedia.PlayUniqueEventAudio(audioEvent as AudioDefaultEvent, RTPCvalue);
                        break;
                    }
                    else
                    {
                        float RTPCvalue = 0;
                        GameAudioMedia.PlayUniqueEventAudio(audioEvent as AudioDefaultEvent, RTPCvalue);
                        break;
                    }
                case EEventType.APullbolt:
                    GameAudioMedia.PlayWeaponReloadAudio(audioEvent as AudioPullboltEvent);
                    break;
                case EEventType.AMeleeAttack:
                    GameAudioMedia.PlayMeleeAttackAudio(audioEvent as AudioMeleeAtkEvent);
                    break;
                default:
                    break;
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
            return (entity as PlayerEntity) != null;
        }
    }
}