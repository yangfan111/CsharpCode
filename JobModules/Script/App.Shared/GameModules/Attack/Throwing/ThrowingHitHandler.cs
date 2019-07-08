using App.Shared.GameModules.Vehicle;
using App.Shared.GameModules.Weapon;
using Core.Prediction.VehiclePrediction.Cmd;
using Core.Utils;

namespace App.Shared.GameModules.Attack
{
    public class ThrowingHitHandler
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ThrowingHitHandler));
        private IPlayerDamager _damager;

        public ThrowingHitHandler(IPlayerDamager damager)
        {
            _damager = damager;
        }

        public void OnPlayerDamage(Contexts contexts, PlayerEntity sourcePlayer, PlayerEntity targetPlayer,
                                   PlayerDamageInfo damage)
        {
            BulletPlayerUtil.ProcessPlayerHealthDamage(contexts, _damager, sourcePlayer, targetPlayer, damage);
        }

        public void OnVehicleDamage(VehicleEntity vehicle, float damage)
        {
            var gameData = vehicle.GetGameData();
            gameData.DecreaseHp(VehiclePartIndex.Body, damage);
        }
    }
}