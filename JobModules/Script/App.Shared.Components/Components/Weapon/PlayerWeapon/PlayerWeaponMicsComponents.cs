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
        [DontInitilize] public int LastFootPrintPlayStamp;
        [DontInitilize] public int ReloadedBulletLeft;

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerAudio;
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
        [DontInitilize] public bool  AutoFire;

        [DontInitilize] public bool AutoThrowing;

        //TODO:移到别处


        public int GetComponentId()
        {
            return (int) EComponentIds.WeaponAux;
        }

    }



    public class PlayerBulletData : BaseRefCounter
        {
            public Vector3 Dir;
            public Vector3 ViewPosition;
            public Vector3 EmitPosition;
            

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