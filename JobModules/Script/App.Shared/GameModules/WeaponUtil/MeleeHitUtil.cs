using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Player;
using App.Shared.GameModules.Vehicle;
using Core;
using Core.Attack;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using UnityEngine;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    public static class MeleeHitUtil
    {
        public static float GetBaseDamage(MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            var damage = 0;
            switch (attackInfo.AttackType)
            {
                case EMeleeAttackType.Soft:
                    damage = config.LeftDamage;
                    break;
                case EMeleeAttackType.Hard:
                    damage = config.RightDamage;
                    break;
                default:
                    MeleeAttackSystem.Logger.ErrorFormat("[HitErr]Melee AttackType {0} is illegal ", attackInfo.AttackType);
                    break;
            }
            return damage;
        }
        public static float GetPlayerFactor(RaycastHit hit, MeleeFireLogicConfig config, EBodyPart part)
        {
        
            var       factor = 1f;
            for (int i = 0; i < config.DamageFactor.Length; i++)
            {
                if (config.DamageFactor[i].BodyPart == part)
                {
                    factor = config.DamageFactor[i].Factor;
                    break;
                }
            }
            return factor;
        }
        public static float GetVehicleFactor(RaycastHit hit, VehicleEntity target, out VehiclePartIndex partIndex)
        {
            Collider collider     = hit.collider;
            var      hitBoxFactor = VehicleEntityUtility.GetHitFactor(target, collider, out partIndex);
            return hitBoxFactor;
        }
        public static bool CanMeleeAttackShowHit(PlayerEntity playerEntity, out RaycastHit effectHit, float distance)
        {
            Vector3    pos;
            Quaternion rot;
            effectHit = new RaycastHit();
            if (playerEntity.TryGetMeleeAttackPosition(out pos) && playerEntity.TryGetMeleeAttackRotation(out rot))
            {
                if (Physics.Raycast(pos, rot.Forward(), out effectHit, distance, BulletLayers.GetBulletLayerMask()))
                {
                    return true;
                }
            }

            return false;
        }
        public static float GetDefaultHeight(PlayerEntity player)
        {
            var height = 1f;
            if (player.hasStateInterface)
            {
                var postureInState = player.stateInterface.State.GetCurrentPostureState();
                switch (postureInState)
                {
                    case PostureInConfig.Crouch:
                        height = 0.7f;
                        break;
                    case PostureInConfig.Prone:
                        height = 0.3f;
                        break;
                    case PostureInConfig.Stand:
                        height = 1.2f;
                        break;
                }
            }

            return height;
        }
    }
}