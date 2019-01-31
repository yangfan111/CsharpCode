using App.Shared.Components.Vehicle;
using App.Shared.GameModules.Bullet;
using Core.EntityComponent;
using Core.Enums;
using Core.Prediction.VehiclePrediction.Event;
using Core.Utils;
using WeaponConfigNs;

namespace App.Shared.GameModules.Vehicle
{
    public static class VehicleDamageUtility
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(VehicleDamageUtility));
        public static IPlayerDamager _damager;

        public static void DoDamageToAllPassgers(Contexts contexts, VehicleEntity vehicle, float damage, EUIDeadType deadType, PlayerEntity sourcePlayer = null, bool isDamagePercent = false, bool sendToServer = false)
        {
            var seats = vehicle.vehicleSeat;
            var orignalDamge = damage;
            for (int seatId = 0; seatId < VehicleSeatComponent.MaxSeatCount; ++seatId)
            {
                EntityKey entityKey;
                if (seats.GetEntityKey(seatId, out entityKey))
                {
                    var player = contexts.player.GetEntityWithEntityKey(entityKey);
                    if (player != null)
                    {
                        if(sourcePlayer == null && deadType == EUIDeadType.VehicleHit)
                        {
                            var lastDriveEntityKey = seats.LastDriverEntityKey;
                            sourcePlayer = contexts.player.GetEntityWithEntityKey(lastDriveEntityKey);
                        }

                        //the case in which the damage is from self
                        if (sourcePlayer == player)
                        {
                            sourcePlayer = null;
                        }

                        if (isDamagePercent)
                        {
                            var gamePlay = player.gamePlay;
                            damage = orignalDamge * gamePlay.MaxHp;
                        }

                        BulletPlayerUtility.ProcessPlayerHealthDamage(contexts, _damager, sourcePlayer, player, new PlayerDamageInfo(damage, (int)deadType, (int)EBodyPart.Chest, vehicle.vehicleAssetInfo.Id));
                        if (sendToServer)
                        {
                            SendDamageToServer(vehicle, player.entityKey.Value, damage);
                        }
                    }
                }
            }
        }

        public static void DoPlayerDamage(Contexts contexts, PlayerEntity sourcePlayer, PlayerEntity targetPlayer, float damage, EUIDeadType hitType = EUIDeadType.VehicleHit, EBodyPart hitPart = EBodyPart.Chest, int weaponId = 0)
        {
            BulletPlayerUtility.ProcessPlayerHealthDamage(contexts, _damager, sourcePlayer, targetPlayer, new PlayerDamageInfo(damage, (int)hitType, (int)hitPart, weaponId));
        }

        public static void SendDamageToServer(VehicleEntity vehicle, EntityKey entityKey, float damage)
        {
            var e = VehicleDamangeSyncEvent.Allocate();
            e.EType = VehicleSyncEventType.Damage;
            e.SourceObjectId = vehicle.entityKey.Value.EntityId;
            e.TargetObject = entityKey;
            e.Damage = damage;
            var comp = vehicle.vehicleSyncEvent;
            comp.SyncEvents.Enqueue(e);
        }
    }
}
