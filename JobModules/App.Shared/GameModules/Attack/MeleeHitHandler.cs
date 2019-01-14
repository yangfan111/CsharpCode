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

namespace App.Shared.GameModules.Attack
{
    public interface IMeleeHitHandler
    {
        void OnHitPlayer(PlayerEntity src, PlayerEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config);
        void OnHitVehicle(PlayerEntity src, VehicleEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config);
        void OnHitEnvrionment(PlayerEntity src, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config);
    }

    public class DummyMeleeHitHandler : IMeleeHitHandler
    {
        public void OnHitEnvrionment(PlayerEntity src, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            DebugDraw.DebugPoint(hit.point, color: Color.blue, duration: 10);
        }

        public void OnHitPlayer(PlayerEntity src, PlayerEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            DebugDraw.DebugPoint(hit.point, color: Color.red, duration: 10);
        }

        public void OnHitVehicle(PlayerEntity src, VehicleEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
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

        public MeleeHitHandler(
            IPlayerDamager damager, 
            ClientEffectContext context, 
            IEntityIdGenerator idGenerator, 
            IDamageInfoCollector damageInfoCollector,
            ISoundEntityFactory soundEntityFactory)
        {
            _damager = damager;
            _clientEffectContext = context;
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

        public void OnHitPlayer(PlayerEntity src, PlayerEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            var baseDamage = GetPlayerFactor(hit, config) * GetBaseDamage(attackInfo, config);
            Collider collider = hit.collider;
            EBodyPart part = BulletPlayerUtility.GetBodyPartByHitBoxName(collider);
            
            //有效命中
            if (target.gamePlay.IsLastLifeState(EPlayerLifeState.Alive))
            {
                src.statisticsData.Statistics.ShootingPlayerCount++;
            }

            NewWeaponConfigItem newConfig = SingletonManager.Get<WeaponConfigManager>().GetConfigById(src.weaponLogicInfo.WeaponId);
            if (null != newConfig && newConfig.SubType == (int)EWeaponSubType.Hand)
            {
                BulletPlayerUtility.ProcessPlayerHealthDamage(_damager, src, target, new PlayerDamageInfo(Mathf.CeilToInt(baseDamage), (int) EUIDeadType.Unarmed, (int) part, 0), _damageInfoCollector);
            }
            else
            {
                BulletPlayerUtility.ProcessPlayerHealthDamage(_damager, src, target, new PlayerDamageInfo(Mathf.CeilToInt(baseDamage), (int)EUIDeadType.Weapon, (int)part, src.weaponLogicInfo.WeaponId), _damageInfoCollector);
            }

            if (target.hasStateInterface && target.stateInterface.State.CanBeenHit())
                target.stateInterface.State.BeenHit();
            ClientEffectFactory.AddHitPlayerEffectEvent(src, target.entityKey.Value, hit.point, hit.point - target.position.Value);
        }

        public void OnHitVehicle(PlayerEntity src, VehicleEntity target, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            VehiclePartIndex partIndex;
            var baseDamage = GetVehicleFactor(hit, target, out partIndex) * GetBaseDamage(attackInfo, config);
            var gameData = target.GetGameData();
            gameData.DecreaseHp(partIndex, baseDamage);
            if(!src.weaponLogic.Weapon.EmptyHand)
            {
                RaycastHit effectHit;
                if(TryGetEffectShowHit(src, out effectHit, config.Range))
                {
                    ClientEffectFactory.AddHitVehicleEffectEvent(
                        src,
                        target.entityKey.Value,
                        effectHit.point,
                        effectHit.point - target.position.Value,
                        effectHit.normal);
 
                }
            }
        }

        public void OnHitEnvrionment(PlayerEntity src, RaycastHit hit, MeleeAttackInfo attackInfo, MeleeFireLogicConfig config)
        {
            if(!src.weaponLogic.Weapon.EmptyHand)
            {
                RaycastHit effectHit;
                if (TryGetEffectShowHit(src, out effectHit, config.Range))
                {
                    ClientEffectFactory.AdHitEnvironmentEffectEvent(src, effectHit.point,
                        effectHit.normal,
                        EEnvironmentType.Wood);
                   
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
