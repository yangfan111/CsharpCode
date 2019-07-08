using System.IO;
using App.Shared.Components.Serializer;
using App.Shared.EntityFactory;
using Assets.Utils.Configuration;
using Core;
using Core.EntityComponent;
using Core.Event;
using Core.ObjectPool;
using Core.Utils;
using Entitas;
using UnityEngine;
using Utils.CharacterState;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.Player.Events
{
    public class FireEvent : IEvent
    {
        public enum FireEffectType
        {
            MuzzleOnly,
            EnjectOnly,
            Both,
        }

        //AudioGrp_ShotMode
      //  public Vector3 muzzlePosition;
        public FireEffectType fireEffectType;
        public EntityKey owner;
        public float pitch;
        public int weaponId;
        public float yaw;

        public EEventType EventType
        {
            get { return EEventType.Fire; }
        }

        public bool IsRemote { get; set; }


        public void ReadBody(BinaryReader reader)
        {
            owner = FieldSerializeUtil.Deserialize(owner, reader);
            fireEffectType = (FireEffectType)FieldSerializeUtil.Deserialize((byte)fireEffectType, reader);
            pitch          = FieldSerializeUtil.Deserialize(pitch, reader);
            yaw            = FieldSerializeUtil.Deserialize(yaw, reader);
            weaponId       = FieldSerializeUtil.Deserialize(weaponId, reader);
        }

        public void WriteBody(MyBinaryWriter writer)
        {
            FieldSerializeUtil.Serialize(owner, writer);
            FieldSerializeUtil.Serialize((byte)fireEffectType, writer);
            FieldSerializeUtil.Serialize(pitch, writer);
            FieldSerializeUtil.Serialize(yaw, writer);
            FieldSerializeUtil.Serialize(weaponId, writer);
        }

        public void RewindTo(IEvent value)
        {
            FireEvent right = value as FireEvent;

            owner = right.owner;
            fireEffectType = right.fireEffectType;
            pitch          = right.pitch;
            yaw            = right.yaw;
            weaponId       = right.weaponId;
        }

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
    }

    public class FireEventHandler : DefaultEventHandler
    {
        static LoggerAdapter logger = new LoggerAdapter("FireEventHandler");

        public override EEventType EventType
        {
            get { return EEventType.Fire; }
        }

        /// <summary>
        /// </summary>
        /// <param name="contexts"></param>
        /// <param name="entity"></param>
        /// <param name="e"></param>
        public override void DoEventClient(IContexts contexts, IEntity entity, IEvent e)
        {
            FireEvent evt = e as FireEvent;
            if (evt != null)
            {
                var effectCfg =
                                SingletonManager.Get<WeaponConfigManagement>().FindConfigById(evt.weaponId)
                                                .S_EffectConfig as DefaultWeaponEffectConfig;
                if (effectCfg == null)
                    return;
                Contexts ctx = contexts as Contexts;
                PlayerEntity player = ctx.player.GetEntityWithEntityKey(evt.owner);
                if (evt.fireEffectType != FireEvent.FireEffectType.EnjectOnly)
                {
                    var muzzleTrans =player.characterBoneInterface.CharacterBone.GetLocation(SpecialLocation.MuzzleEffectPosition,CharacterView.ThirdPerson);
                    if(muzzleTrans)
                        ClientEffectFactory.CreateMuzzleSparkEffct(muzzleTrans.position,
                            evt.pitch, evt.yaw, effectCfg.Spark,muzzleTrans);
                }

                if (evt.fireEffectType != FireEvent.FireEffectType.MuzzleOnly)
                {
                    var ejectTrans =player.characterBoneInterface.CharacterBone.GetLocation(SpecialLocation.EjectionLocation,CharacterView.ThirdPerson);
                    if(ejectTrans)
                        ClientEffectFactory.CreateBulletDrop( ejectTrans.position,
                            evt.yaw, evt.pitch, effectCfg.BulletDrop, evt.weaponId, AudioGrp_FootMatType.Concrete);
                }
               


                // var weaponGo = (entity as PlayerEntity).appearanceInterface.Appearance.GetWeaponP1InHand();
                //        GameAudioMedia.PlayWeaponFireAudio(evt.fireWeaponId, evt.audioFirePos, (AudioGrp_ShotMode)evt.audioFireMode);
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
            return playerEntity != null; //&& playerEntity.WeaponController().EffectList != null;
        }
    }
}