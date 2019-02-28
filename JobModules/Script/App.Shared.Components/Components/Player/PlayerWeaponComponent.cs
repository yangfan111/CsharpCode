using System;
using System.Collections.Generic;
using Core;
using Core.ObjectPool;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;
using XmlConfig;

namespace App.Shared.Components.Player
{
 

    [Player]
    public class WeaponSound : IComponent
    {
        public List<EWeaponSoundType> PlayList;
    }

    [Player]
    public class WeaponEffect:IComponent
    {
        public List<EClientEffectType> PlayList;
    }

    [Player]
    public class WeaponAutoStateComponent : IComponent
    {
        [Flags]
        public enum EAutoFireState
        {
            Burst,
            ReloadBreak,
        } 
        [DontInitilize] public bool AutoThrowing;
        [DontInitilize] public int AutoFire;

        public void Reset()
        {
            AutoThrowing = false; 
        }
    }

    [Player]
    public class PlayerInterruptStateComponent : IComponent
    {
        [Flags]
        public enum InterruptReason
        {
            BagUI = 1,
        }
        [DontInitilize] public int ForceInterruptGunSight; 
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

    [Player]
    public class PlayerBulletDataComponent : IComponent
    {
        public List<PlayerBulletData> DataList;
    }
}