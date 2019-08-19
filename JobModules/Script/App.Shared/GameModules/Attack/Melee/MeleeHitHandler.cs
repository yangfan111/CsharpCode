using App.Shared.EntityFactory;
using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Weapon;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Enums;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using UltimateFracturing;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Attack
{
    //    public interface IMeleeHitHandler
    //    {
    //        void OnHitPlayer(Contexts contexts, PlayerEntity src, PlayerEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config, int seq);
    //        void OnHitVehicle(Contexts contexts, PlayerEntity src, VehicleEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config);
    //        void OnHitEnvrionment(Contexts contexts, PlayerEntity src, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config);
    //    }
    //
    //    public class DummyMeleeHitHandler : IMeleeHitHandler
    //    {
    //        public void OnHitEnvrionment(Contexts contexts, PlayerEntity src, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
    //        {
    //            DebugDraw.DebugPoint(hit.point, color: Color.blue, duration: 10);
    //        }
    //
    //        public void OnHitPlayer(Contexts contexts, PlayerEntity src, PlayerEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config, int seq)
    //        {
    //            DebugDraw.DebugPoint(hit.point, color: Color.red, duration: 10);
    //        }
    //
    //        public void OnHitVehicle(Contexts contexts, PlayerEntity src, VehicleEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
    //        {
    //            DebugDraw.DebugPoint(hit.point, color: Color.yellow, duration: 10);
    //        }
    //    }

    public class MeleeHitHandler
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MeleeHitHandler));
        private IPlayerDamager _damager;

        public MeleeHitHandler(IPlayerDamager damager)
        {
            _damager = damager;
        }


        public void OnHitPlayer(Contexts contexts, PlayerEntity src, PlayerEntity target, RaycastHit hit,
                                MeleeAttackInfo attackInfo, MeleeFireLogicConfig config, int seq)
        {
            EBodyPart part = BulletPlayerUtil.GetBodyPartByHitBoxName(hit.collider);
            var baseDamage = MeleeHitUtil.GetPlayerFactor(hit, config, part) *
                            MeleeHitUtil.GetBaseDamage(attackInfo, config);
            if (src.hasStatisticsData)
            {
                src.statisticsData.Statistics.AttackType = (int) attackInfo.AttackType;
            }

            //有效命中
            /*if (target.gamePlay.IsLastLifeState(EPlayerLifeState.Alive))
            {
                src.statisticsData.Statistics.ShootingPlayerCount++;
            }*/
            var playerWeaponId = src.WeaponController().HeldConfigId;
            WeaponResConfigItem newConfig =
                            SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(playerWeaponId);
            EUIDeadType euiDeadType = (null != newConfig && newConfig.SubType == (int) EWeaponSubType.Hand)
                            ? EUIDeadType.Unarmed
                            : EUIDeadType.Weapon;

            BulletPlayerUtil.ProcessPlayerHealthDamage(contexts, _damager, src, target,
                new PlayerDamageInfo(Mathf.CeilToInt(baseDamage), (int) euiDeadType, (int) part,
                    src.WeaponController().HeldWeaponAgent.ConfigId, false, false, false, hit.point,
                    target.position.Value - src.position.Value));

            //   Logger.InfoFormat("[Hit] process damage sucess,dvalue:{0}", baseDamage);
            //由于动画放在客户端做了,服务器调用的命令会被忽视,需要发送事件到客户端
            //            if (target.hasStateInterface && target.stateInterface.State.CanBeenHit())
            //            {
            //                target.stateInterface.State.BeenHit();
            //            }

            ClientEffectFactory.AddBeenHitEvent(src, target, AttackUtil.GeneraterUniqueHitId(src, seq),
                contexts.session.currentTimeObject.CurrentTime);
            int audioId = SingletonManager.Get<AudioWeaponManager>().FindById(playerWeaponId).HitList.Body;
            ClientEffectFactory.AddHitPlayerEffectEvent(src, target.entityKey.Value, hit.point, audioId, part);
        }

        public void OnHitVehicle(Contexts contexts, PlayerEntity src, VehicleEntity target, RaycastHit hit,
                                 MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            VehiclePartIndex partIndex;
            var baseDamage = MeleeHitUtil.GetVehicleFactor(hit, target, out partIndex) *
                            MeleeHitUtil.GetBaseDamage(attackInfo, config);
            var gameData = target.GetGameData();
            gameData.DecreaseHp(partIndex, baseDamage);
            if (!src.WeaponController().IsHeldSlotEmpty)
            {
                RaycastHit effectHit;
                if (MeleeHitUtil.CanMeleeAttackShowHit(src, out effectHit, config.Range))
                {
                    ClientEffectFactory.AddHitVehicleEffectEvent(src, target.entityKey.Value, effectHit.point,
                        effectHit.point - target.position.Value, effectHit.normal);
                }
            }
        }

        public void OnHitEnvrionment(Contexts contexts, PlayerEntity src, RaycastHit hit, MeleeAttackInfo attackInfo,
                                     MeleeFireLogicConfig config)
        {
            var        heldConfigId = src.WeaponController().HeldConfigId;
            RaycastHit effectHit;
            bool showAttackEffect = (MeleeHitUtil.CanMeleeAttackShowHit(src, out effectHit, config.Range));
            var collider          = effectHit.collider != null ? effectHit.collider : hit.collider;
            FracturedHittable fracturedHittable = collider.GetComponent<FracturedHittable>();
            FracturedAbstractChunk fracturedChunk = HitFracturedHandler.HitFracturedObj(src, effectHit, fracturedHittable);
            var hasHole = fracturedChunk == null || (fracturedChunk.HasBulletHole && !fracturedChunk.IsBroken());
            if (showAttackEffect && hasHole)
            {
                int audioId = SingletonManager.Get<AudioWeaponManager>().FindById(heldConfigId).HitList.Body;
                ClientEffectFactory.AdHitEnvironmentEffectEvent(src, effectHit.point, effectHit.normal,
                    EEnvironmentType.Wood, audioId, 0, heldConfigId != WeaponUtil.EmptyHandId);
            }
            
        }
    }
}