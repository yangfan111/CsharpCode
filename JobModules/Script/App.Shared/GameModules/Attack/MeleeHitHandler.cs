using Core.Attack;
using UnityEngine;
using WeaponConfigNs;
using App.Shared.GameModules.Bullet;
using Core.Utils;
using App.Shared.EntityFactory;
using App.Shared.GameModules.Vehicle;
using Core.Prediction.VehiclePrediction.Cmd;
using App.Shared.Components;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.Enums;
using Core.IFactory;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player;
using Core.BulletSimulation;
using Utils.Singleton;

using App.Shared.GameModules.Weapon;

namespace App.Shared.GameModules.Attack
{
    public interface IMeleeHitHandler
    {
        void OnHitPlayer(Contexts contexts, PlayerEntity src, PlayerEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config, int seq);
        void OnHitVehicle(Contexts contexts, PlayerEntity src, VehicleEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config);
        void OnHitEnvrionment(Contexts contexts, PlayerEntity src, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config);
    }

    public class DummyMeleeHitHandler : IMeleeHitHandler
    {
        public void OnHitEnvrionment(Contexts contexts, PlayerEntity src, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            DebugDraw.DebugPoint(hit.point, color: Color.blue, duration: 10);
        }

        public void OnHitPlayer(Contexts contexts, PlayerEntity src, PlayerEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config, int seq)
        {
            DebugDraw.DebugPoint(hit.point, color: Color.red, duration: 10);
        }

        public void OnHitVehicle(Contexts contexts, PlayerEntity src, VehicleEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            DebugDraw.DebugPoint(hit.point, color: Color.yellow, duration: 10);
        }
    }

    public class MeleeHitHandler : IMeleeHitHandler
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(MeleeHitHandler));
        private IPlayerDamager _damager;
        private ClientEffectContext _clientEffectContext;
        private IEntityIdGenerator _entityIdGenerator;
        private IDamageInfoCollector _damageInfoCollector;
        private ISoundEntityFactory _soundEntityFactory;
        private Contexts _contexts;

        public MeleeHitHandler(
            Contexts contexts,
            IPlayerDamager damager, 
            IEntityIdGenerator idGenerator, 
            IDamageInfoCollector damageInfoCollector,
            ISoundEntityFactory soundEntityFactory)
        {
            _contexts = contexts;
            _damager = damager;
            _clientEffectContext = contexts.clientEffect;
            _entityIdGenerator = idGenerator;
            _damageInfoCollector = damageInfoCollector;
            _soundEntityFactory = soundEntityFactory;
        }

        private float GetBaseDamage(MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            var damage = 0;
            switch (attackInfo.AttackType)
            {
                case MeleeAttckType.LeftMeleeAttack:
                    damage = config.LeftDamage;
                    break;
                case MeleeAttckType.RightMeleeAttack:
                    damage = config.RightDamage;
                    break;
                default:
                    damage = 1;
                    Logger.ErrorFormat("AttackType {0} is illegal ", attackInfo.AttackType);
                    break;
            }
            return damage;
        }

        private float GetPlayerFactor(RaycastHit hit, MeleeFireLogicConfig config)
        {
            Collider collider = hit.collider;

            EBodyPart part = BulletPlayerUtility.GetBodyPartByHitBoxName(collider);
            var factor = 1f;
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

        private float GetVehicleFactor(RaycastHit hit, VehicleEntity target, out VehiclePartIndex partIndex)
        {
            Collider collider = hit.collider;
            var hitBoxFactor = VehicleEntityUtility.GetHitFactor(target, collider, out partIndex);
            return hitBoxFactor;
        }

        public void OnHitPlayer(Contexts contexts, PlayerEntity src, PlayerEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config, int seq)
        {
            var baseDamage = GetPlayerFactor(hit, config) * GetBaseDamage(attackInfo, config);
            Collider collider = hit.collider;
            EBodyPart part = BulletPlayerUtility.GetBodyPartByHitBoxName(collider);
            
            //有效命中
            /*if (target.gamePlay.IsLastLifeState(EPlayerLifeState.Alive))
            {
                src.statisticsData.Statistics.ShootingPlayerCount++;
            }*/

            WeaponResConfigItem newConfig = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(src.WeaponController().HeldWeaponAgent.ConfigId);
            if (null != newConfig && newConfig.SubType == (int)EWeaponSubType.Hand)
            {
                
                BulletPlayerUtility.ProcessPlayerHealthDamage(contexts, _damager, src, target, new PlayerDamageInfo(Mathf.CeilToInt(baseDamage), (int) EUIDeadType.Unarmed, (int) part, 0), _damageInfoCollector);
            }
            else
            {
                BulletPlayerUtility.ProcessPlayerHealthDamage(contexts, _damager, src, target, new PlayerDamageInfo(Mathf.CeilToInt(baseDamage), (int)EUIDeadType.Weapon, (int)part, src.WeaponController().HeldWeaponAgent.ConfigId), _damageInfoCollector);
            }

            //由于动画放在客户端做了,服务器调用的命令会被忽视,需要发送事件到客户端
//            if (target.hasStateInterface && target.stateInterface.State.CanBeenHit())
//            {
//                target.stateInterface.State.BeenHit();
//            }
            
            ClientEffectFactory.AddBeenHitEvent(src, target.entityKey.Value, BulletHitHandler.GeneraterUniqueHitId(src, seq), contexts.session.currentTimeObject.CurrentTime);
            ClientEffectFactory.AddHitPlayerEffectEvent(src, target.entityKey.Value, hit.point, hit.point - target.position.Value);
        }

        public void OnHitVehicle(Contexts contexts, PlayerEntity src, VehicleEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            VehiclePartIndex partIndex;
            var baseDamage = GetVehicleFactor(hit, target, out partIndex) * GetBaseDamage(attackInfo, config);
            var gameData = target.GetGameData();
            gameData.DecreaseHp(partIndex, baseDamage);
            if(!src.WeaponController().IsHeldSlotEmpty)
            {
                RaycastHit effectHit;
                if(TryGetEffectShowHit(src, out effectHit, config.Range))
                {
                    ClientEffectFactory.AddHitVehicleEffectEvent(src, target.entityKey.Value, effectHit.point, effectHit.point - target.position.Value, effectHit.normal);
 
                }
            }
        }

        public void OnHitEnvrionment(Contexts contexts, PlayerEntity src, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            if(!src.WeaponController().IsHeldSlotEmpty)
            {
                RaycastHit effectHit;
                if (TryGetEffectShowHit(src, out effectHit, config.Range))
                {
                    ClientEffectFactory.AdHitEnvironmentEffectEvent(src, effectHit.point, effectHit.normal, EEnvironmentType.Wood);
                }
            }
        }

        private bool TryGetEffectShowHit(PlayerEntity playerEntity, out RaycastHit effectHit, float distance)
        {
            Vector3 pos;
            Quaternion rot;
            effectHit = new RaycastHit();
            if(playerEntity.TryGetMeleeAttackPosition(out pos) && playerEntity.TryGetMeleeAttackRotation(out rot))
            {
                if(Physics.Raycast(pos, rot.Forward(), out effectHit, distance, BulletLayers.GetBulletLayerMask()))
                {
                    return true;                    
                }
            }
            return false;
        }
    }
}
