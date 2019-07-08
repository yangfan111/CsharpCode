using App.Shared.EntityFactory;
using App.Shared.GameModules.Vehicle;
using Core.Attack;
using Core.Compensation;
using Core.HitBox;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace App.Shared.GameModules.Attack
{
    public class HitVehicleHandler:AbstractHitHandler
    {
        private VehicleEntity hitTargetVehicle;
        public HitVehicleHandler(Contexts contexts) : base(contexts)
        {
            profilerInfo = SingletonManager.Get<DurationHelp>().GetCustomProfileInfo("BulletHitHandler_OnHitVehicle");
        }
        protected override bool Filter(HitBoxOwnerComponent boxOwnerComponent, int cmdSeq)
        {
            if (boxOwnerComponent && boxOwnerComponent.OwnerEntityKey.EntityType == (short) EEntityType.Vehicle)
            {
                hitTargetVehicle = contexts.vehicle.GetEntityWithEntityKey(boxOwnerComponent.OwnerEntityKey) ?? null;
                if (hitTargetVehicle == null)
                {
                    return false;
                }
                this.cmdSeq = cmdSeq;
                return true;
            }

            return false;
        }

        protected override void Handle(CompensationWorld compensationWorld, IBulletEntityAgent bulletEntityAgent, RaycastHit hit,
                                       PlayerEntity srcPlayer)
        {
                DoHitVehicle(srcPlayer, bulletEntityAgent, hit);
                bulletEntityAgent.HitType = EHitType.Vehicle;

            
        }
        private void DoHitVehicle(PlayerEntity srcPlayer,
                                 IBulletEntityAgent bulletEntityAgent, RaycastHit hit)
        {
            if (srcPlayer.gamePlay.IsDead())
            {
                return;
            }

            Collider         collider = hit.collider;
            VehiclePartIndex partIndex;
            var              hitBoxFactor = VehicleEntityUtility.GetHitFactor(hitTargetVehicle, collider, out partIndex);

            var totalDamage = GetBulletDamage(bulletEntityAgent, hitBoxFactor,
                Vector3.Distance(hit.point, bulletEntityAgent.GunEmitPosition));
            var gameData = hitTargetVehicle.GetGameData();
            gameData.DecreaseHp(partIndex, totalDamage, srcPlayer.entityKey.Value);
            srcPlayer.statisticsData.Statistics.TotalDamage += totalDamage;

            bulletEntityAgent.IsValid = false;

            srcPlayer.statisticsData.Statistics.ShootingSuccCount++;
            BulletHitHandler._logger.InfoFormat("[Hit{0}]bullet from {0} hit vehicle {1}, part {2}", bulletEntityAgent.OwnerEntityKey,
                hitTargetVehicle.entityKey.Value, collider.name, cmdSeq);

            ClientEffectFactory.AddHitVehicleEffectEvent(srcPlayer, hitTargetVehicle.entityKey.Value, hit.point,
                hit.point - hitTargetVehicle.position.Value, hit.normal);
        }

      
    }
}