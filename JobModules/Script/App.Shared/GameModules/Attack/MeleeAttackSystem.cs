using App.Shared.Components.Player;
using App.Shared.GameModules.Bullet;
using App.Shared.GameModules.Player;
using Core.BulletSimulation;
using Core.Compensation;
using Core.GameModule.Interface;
using Core.HitBox;
using Core.ObjectPool;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Entitas;
using System.Collections.Generic;
using App.Shared.Util;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Attack
{
    public class MeleeAttackSystem : AbstractUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MeleeAttackSystem));
        private IGroup<PlayerEntity> _attackers;
        private ICompensationWorldFactory _compensationWorldFactory;
        private Contexts _contexts;
        private IMeleeHitHandler _meleeHitHandler;

        public MeleeAttackSystem(
            Contexts contexts,
            ICompensationWorldFactory compensationWorld,
            IMeleeHitHandler hitHandler)
        {
            _contexts = contexts;
            _attackers = contexts.player.GetGroup(PlayerMatcher.MeleeAttackInfoSync);
            _compensationWorldFactory = compensationWorld;
            _meleeHitHandler = hitHandler;
        }

        protected override bool filter(PlayerEntity player)
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
            if (cmd.RenderTime < player.meleeAttackInfoSync.AttackTime)
            {
                return;
            }

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
                emitPos = player.position.Value + Vector3.up * GetDefaultHeight(player);
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
                    _meleeHitHandler.OnHitPlayer(_contexts, player, targetPlayer, hit, attackInfo, config, cmd.Seq);
                }
                else if (targetVehicle != null)
                {
                    _meleeHitHandler.OnHitVehicle(_contexts, player, targetVehicle, hit, attackInfo, config);
                }
                else
                {
                    _meleeHitHandler.OnHitEnvrionment(_contexts, player, hit, attackInfo, config);
                }
            }

            compensationWorld.Release();
        }

        private float GetDefaultHeight(PlayerEntity player)
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
                    default:
                        break;
                }
            }

            return height;
        }
    }
}