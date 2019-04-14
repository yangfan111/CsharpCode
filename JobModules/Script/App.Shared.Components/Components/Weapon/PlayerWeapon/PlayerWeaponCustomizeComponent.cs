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
    public class PlayerWeaponCustomizeComponent : IUserPredictionComponent
    {
        [NetworkProperty] public EntityKey GrenadeConstWeaponKey;
        [NetworkProperty] public EntityKey EmptyConstWeaponkey;
        [DontInitilize, NetworkProperty] public int BagOpenLimitTime;
        [DontInitilize, NetworkProperty] public bool BagLockState;
        [DontInitilize,NetworkProperty] public int TacticWeapon;
        [DontInitilize, NetworkProperty] public int LastFireWeaponId;

        public void CopyFrom(object rightComponent)
        {
            CopyFrom(rightComponent as PlayerWeaponCustomizeComponent);
        }

        void CopyFrom(PlayerWeaponCustomizeComponent comp)
        {
            GrenadeConstWeaponKey = comp.GrenadeConstWeaponKey;
            EmptyConstWeaponkey = comp.EmptyConstWeaponkey;
            BagOpenLimitTime = comp.BagOpenLimitTime;
            BagLockState = comp.BagLockState;
            TacticWeapon = comp.TacticWeapon;
            LastFireWeaponId = comp.LastFireWeaponId;
        }

        public int GetComponentId()
        {
            return (int)EComponentIds.PlayerWeaponCustomize;
        }

        public bool IsApproximatelyEqual(object right)
        {
            var cmp = right as PlayerWeaponCustomizeComponent;
            return GrenadeConstWeaponKey == cmp.GrenadeConstWeaponKey &&
                   EmptyConstWeaponkey == cmp.EmptyConstWeaponkey && BagOpenLimitTime == cmp.BagOpenLimitTime
                    && BagLockState == cmp.BagLockState && TacticWeapon == cmp.TacticWeapon && LastFireWeaponId == cmp.LastFireWeaponId;
        }

        public void RewindTo(object rightComponent)
        {
            CopyFrom(rightComponent);
        }
    }

  
}