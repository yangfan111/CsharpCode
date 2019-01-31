using Core;
using Core.GameModeLogic;
using Core.Utils;
using Entitas;

namespace App.Shared.GameModeLogic.WeaponActionListener
{
    public class NormalWeaponActionListener : IWeaponProcessListener
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(NormalWeaponActionListener));
        public void OnExpend(Entity playerEntity, EWeaponSlotType slot)
        {
            if(!slot.IsSlotChangeByCost())
            {
                return;
            }
            var player = playerEntity as PlayerEntity;
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("{0} OnExpend", player.entityKey.Value);
            }
            LockPlayerBag(player);
        }

        public void OnDrop(Entity playerEntity, EWeaponSlotType slot)
        {
            var player = playerEntity as PlayerEntity;
            Logger.DebugFormat("{0} OnDrop", player.entityKey.Value);
            LockPlayerBag(player);
        }

        public void OnPickup(Entity playerEntity, EWeaponSlotType slot)
        {
            var player = playerEntity as PlayerEntity;
            Logger.DebugFormat("{0} OnPickup", player.entityKey.Value);
            LockPlayerBag(player);
        }

        private void LockPlayerBag(PlayerEntity player)
        {
            Logger.DebugFormat("{0} LockPlayerBag", player.entityKey.Value);
            player.weaponState.BagLocked = true;
        }
    }
}
