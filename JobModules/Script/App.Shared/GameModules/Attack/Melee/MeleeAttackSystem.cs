using System;
using App.Shared.Components.Player;
using App.Shared.GameModules.Attack;
using App.Shared.GameModules.Player;
using Core.Attack;
using Core.Compensation;
using Core.GameModule.Interface;
using Core.HitBox;
using Core.ObjectPool;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Entitas;
using System.Collections.Generic;
using App.Shared.GameModules.Weapon;
using App.Shared.Util;
using Core;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Attack
{
    public class MeleeAttackSystem : AbstractUserCmdExecuteSystem
    {
        public static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MeleeAttackSystem));
        private ICompensationWorldFactory _compensationWorldFactory;
        private Contexts _contexts;
        private MeleeHitHandler hitHandler;

        public MeleeAttackSystem(
            Contexts contexts,
            ICompensationWorldFactory compensationWorld,
            MeleeHitHandler hitHandler)
        {
            _contexts = contexts;
            _compensationWorldFactory = compensationWorld;
            this.hitHandler = hitHandler;
        }

        protected override bool Filter(PlayerEntity player)
        {
            if (!player.hasMeleeAttackInfoSync) return false;
            if (!player.hasMeleeAttackInfo) return false;
            if (!player.hasGamePlay) return false;
            if (player.gamePlay.LifeState != (int) EPlayerLifeState.Alive) return false;
            if (!player.hasPosition) return false;
            if (!player.hasOrientation) return false;
            if (!player.hasPlayerHitMaskController) return false;
            return true;
        }

        protected override void ExecuteUserCmd(PlayerEntity player, IUserCmd cmd)
        {
            if (player.time.ClientTime < player.meleeAttackInfoSync.AttackTime)
                return;
            // Logger.Info("Try Attack One Time, Interval:"+ (player.meleeAttackInfoSync.AttackTime - player.meleeAttackInfoSync.BeforeAttackTime));
            // Logger.Info("Try Attack One Time, Interval:"+ (player.time.ClientTime - player.meleeAttackInfoSync.BeforeAttackTime));

            var config = player.meleeAttackInfo.AttackConfig;
            if(null == config)
            {
                Logger.Error("attack info in player MeleeAttackInfo is null");
                return;
            }
            var attackInfo = player.meleeAttackInfo.AttackInfo;
            player.RemoveMeleeAttackInfoSync();
            var compensationWorld = _compensationWorldFactory.CreateCompensationWorld(cmd.RenderTime);
            if (null == compensationWorld)
            {
                Logger.ErrorFormat("CompensationWorld is null for time {0}", cmd.RenderTime);
                return;
            }

            compensationWorld.Self = player.entityKey.Value;
            compensationWorld.ExcludePlayerList =
                player.playerHitMaskController.HitMaskController.MeleeExcludeTargetList;

            Quaternion rotation;
            player.TryGetMeleeAttackRotation(out rotation); 
            RaycastHit hit;
            //小于这个距离没有检测,设一个足够小的值
            var minDistance = 0.01f;
            var extens = new Vector3(config.Width, config.Height, minDistance);
            Vector3 emitPos;
            if (!PlayerEntityUtility.TryGetMeleeAttackPosition(player, out emitPos))
            {
                Logger.Error("get melee attack position failed ");
                emitPos = player.position.Value + Vector3.up * MeleeHitUtil.GetDefaultHeight(player);
            }

            var box = new BoxInfo
            {
                Length = config.Range,
                Direction = rotation.Forward(),
                Origin = emitPos,
                Orientation = rotation,
                HalfExtens = extens / 2f,
            };
            if (compensationWorld.BoxCast(box, out hit, BulletLayers.GetBulletLayerMask()))
            {
                PlayerEntity targetPlayer = null;
                VehicleEntity targetVehicle = null;
                var comp = hit.collider.transform.gameObject.GetComponent<HitBoxOwnerComponent>();
                if (comp != null)
                {
                    targetPlayer = _contexts.player.GetEntityWithEntityKey(comp.OwnerEntityKey);
                    targetVehicle = _contexts.vehicle.GetEntityWithEntityKey(comp.OwnerEntityKey);
                }

                if (targetPlayer != null)
                {
                    hitHandler.OnHitPlayer(_contexts, player, targetPlayer, hit, attackInfo, config, cmd.Seq);
                }
                else if (targetVehicle != null)
                {
                    hitHandler.OnHitVehicle(_contexts, player, targetVehicle, hit, attackInfo, config);
                }
                else
                {
                    hitHandler.OnHitEnvrionment(_contexts, player, hit, attackInfo, config);
                }
            }

            compensationWorld.Release();
           // Logger.Info("Try Attack Finish");
        }

       
    }
}