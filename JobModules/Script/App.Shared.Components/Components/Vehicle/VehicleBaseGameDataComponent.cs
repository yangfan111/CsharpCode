using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using Core.Configuration;
using Core.EntityComponent;
using Core.Enums;
using Core.Prediction;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.Utils;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Vehicle
{
    public abstract class VehicleBaseGameDataComponent : IRewindableComponent, IVehicleResetableComponent
    {

        [DontInitilize] public int MaxHp;
        [DontInitilize] public int MaxFuel;

        [DontInitilize] public float FuelCost;
        [DontInitilize] public float FuleCostOnAcceleration;

        [DontInitilize] public EUIDeadType LastDamageType;
        [DontInitilize] public EntityKey LastDamageSource;

        [NetworkProperty] [DontInitilize] public float Hp;

        [NetworkProperty] [DontInitilize] public float RemainingFuel;

        [NetworkProperty] [DontInitilize] public bool IsAccelerated;

        [NetworkProperty] [DontInitilize] public int CurrentSoundId;

        [NetworkProperty] [DontInitilize] public float SoundSyncTime;


        [DontInitilize] public float MusicStartTime;

        [DontInitilize] public int SoundChannel;
        [DontInitilize] public int ChannelSoundId1;
        [DontInitilize] public int ChannelSoundId2;
        [DontInitilize] public int ChannelSoundId3;

        public virtual float DecreaseHp(VehiclePartIndex partIndex, float amount, EUIDeadType damageType, EntityKey damageSource)
        {
            if(partIndex == VehiclePartIndex.Body)
            {
                if (Hp > 0 && Hp < amount)
                {
                    LastDamageType = damageType;
                    LastDamageSource = damageSource;
                }

                Hp = Hp < amount ? 0.0f : Hp - amount;
                return Hp;
            }

            return 0.0f;
        }

        public float DecreaseHp(VehiclePartIndex partIndex, float amount, EUIDeadType damageType)
        {
            return DecreaseHp(partIndex, amount, damageType, EntityKey.Default);
        }

        public float DecreaseHp(VehiclePartIndex partIndex, float amount, EntityKey damageSource)
        {
            return DecreaseHp(partIndex, amount, EUIDeadType.Weapon, damageSource);
        }

        public float DecreaseHp(VehiclePartIndex partIndex, float amount)
        {
            return DecreaseHp(partIndex, amount, EUIDeadType.Weapon, EntityKey.Default);
        }

        public float DecreaseRemainingFuel(float amount)
        {
            AssertUtility.Assert(amount >= 0);

            RemainingFuel = RemainingFuel < amount ? 0.0f : RemainingFuel - amount;
            return RemainingFuel;
        }

        public virtual void CopyFrom(object rightComponent)
        {
            var r = rightComponent as VehicleBaseGameDataComponent;

            LastDamageSource = r.LastDamageSource;

            Hp = r.Hp;
            RemainingFuel = r.RemainingFuel;
            IsAccelerated = r.IsAccelerated;
            CurrentSoundId = r.CurrentSoundId;
            SoundSyncTime = r.SoundSyncTime;
        }

        public void SetSoundChannel(EVehicleChannel channel)
        {
            MusicStartTime = Time.time;
            SoundSyncTime = 0.0f;

            SoundChannel = (int) channel;
            switch (channel)
            {
                case EVehicleChannel.C1:
                    CurrentSoundId = ChannelSoundId1;
                   
                    break;
                case EVehicleChannel.C2:
                    CurrentSoundId = ChannelSoundId2;
                    break;
                case EVehicleChannel.C3:
                    CurrentSoundId = ChannelSoundId3;
                    break;
                default:
                    CurrentSoundId = (int) EVehicleSoundId.Invalid;
                    SoundChannel = (int) EVehicleChannel.None;
                    break;
            }
        }

        public void SetMusicSyncTime(float currentTime)
        {
            SoundSyncTime = currentTime - MusicStartTime;
        }

        public void ClearAllSounds()
        {
            CurrentSoundId = (int)EVehicleSoundId.Invalid;
            SoundChannel = (int)EVehicleChannel.None;
            SoundSyncTime = 0.0f;
        }

        public virtual void Reset()
        {
            Hp = MaxHp;
            RemainingFuel = MaxFuel;
            IsAccelerated = false;
            CurrentSoundId = 0;
            SoundSyncTime = 0;

            LastDamageType = EUIDeadType.Unkown;
            LastDamageSource = EntityKey.Default;
        }

        public abstract void RewindTo(object rightComponent);
    }
}
