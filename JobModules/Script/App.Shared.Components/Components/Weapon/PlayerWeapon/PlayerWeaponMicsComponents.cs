using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Components;
using Core.Prediction.UserPrediction;
using Entitas.CodeGeneration.Attributes;
using Core;
using Core.Utils;
using System.Collections.Generic;
using XmlConfig;
using UnityEngine;
using Core.ObjectPool;
using Entitas;
using System;
using Core.EntityComponent;
using System.Text;
using Core.UpdateLatest;

namespace App.Shared.Components.Player
{
  
    [Player]
    public class PlayerAudioComponent : IGameComponent
    {
        [DontInitilize] public bool InWaterState;
        [DontInitilize] public float C4UnityStamp;
        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerAudio;
        }
    }
    [Player]
    public class PlayerWeaponDebugComponent : IUpdateComponent
    {
        [DontInitilize] public EntityKey Slot1;
        [DontInitilize] public int       ConfigId1;
        [DontInitilize] public EntityKey Slot2;
        [DontInitilize] public int       ConfigId2;
        [DontInitilize] public EntityKey Slot3;
        [DontInitilize] public int       ConfigId3;
        [DontInitilize] public EntityKey Slot4;
        [DontInitilize] public int       ConfigId4;

        [DontInitilize] public EntityKey Slot5;
        [DontInitilize] public int       ConfigId5;

        [DontInitilize] public EntityKey Slot6;
        [DontInitilize] public int       ConfigId6;
        /// <summary>
        /// 0:None;1:left,-1:right
        /// </summary>
        [DontInitilize,NetworkProperty] public int DebugAutoMove; 


        public void CopyFrom(object rightComponent)
        {
            DebugAutoMove = (rightComponent as PlayerWeaponDebugComponent).DebugAutoMove;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.WeaponDebug;
        }
    }

    [Player,]
        public class PlayerWeaponAuxiliaryComponent : IGameComponent
        {
            // PlayerInterruptStateComponent
            //PlayerBulletDataComponent
            [DontInitilize] public List<PlayerBulletData> BulletList;




            [DontInitilize] public bool ClientInitialize;

            [DontInitilize] public List<EClientEffectType> EffectList;



            //Auto Fire
            [DontInitilize] public bool HasAutoAction;
            [DontInitilize] public bool AutoFire;

            [DontInitilize] public bool AutoThrowing;

            //TODO:移到别处


            public int GetComponentId()
            {
                return (int) EComponentIds.WeaponAux;
            }

        }

        

        public class PlayerBulletData : BaseRefCounter
        {
            public PrecisionsVector3 Dir;
            public PrecisionsVector3 ViewPosition;
            public Vector3 EmitPosition;

            
            public override string ToString()
            {
                return string.Format("{0},{1},{2}", Dir, ViewPosition, EmitPosition);
            }
            public  string ToStringExt(StringBuilder stringBuilder)
            {
                stringBuilder.Length = 0;
                stringBuilder.AppendFormat("Dir:{0},ViewPosition:{1}", Dir, ViewPosition);
                return stringBuilder.ToString();
            }
            public static PlayerBulletData Allocate()
            {
                return ObjectAllocatorHolder<PlayerBulletData>.Allocate();
            }

            protected override void OnCleanUp()
            {
                ObjectAllocatorHolder<PlayerBulletData>.Free(this);
            }
        }


        [System.Obsolete]
        [Player]
        public class WeaponSound : IComponent
        {
            public List<EWeaponSoundType> PlayList;
        }
}