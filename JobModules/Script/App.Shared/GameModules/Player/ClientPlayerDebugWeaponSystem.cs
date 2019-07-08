using App.Client.GameModules.Player;
using App.Shared.GameModules.Player.Appearance.WeaponControllerPackage;
using Core;
using Core.Utils;
using Entitas;

namespace App.Shared.GameModules.Player
{
    public class ClientPlayerDebugWeaponSystem: AbstractGamePlaySystem<PlayerEntity>
    {
        public ClientPlayerDebugWeaponSystem(Contexts contexts) : base(contexts)
        {
        }

        protected override IGroup<PlayerEntity> GetIGroup(Contexts contexts)
        {
            return contexts.player.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.PlayerWeaponDebug, PlayerMatcher.FlagSelf));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }

        protected override void OnGamePlay(PlayerEntity entity)
        {
            var weaponController = entity.WeaponController();
            var agent = weaponController.GetWeaponAgent((EWeaponSlotType) 1);
            entity.playerWeaponDebug.Slot1 = agent.WeaponKey;
            entity.playerWeaponDebug.ConfigId1 = agent.ConfigId;
            agent = weaponController.GetWeaponAgent((EWeaponSlotType) 2); 
            entity.playerWeaponDebug.Slot2     = agent.WeaponKey;
            entity.playerWeaponDebug.ConfigId2 = agent.ConfigId;
            agent = weaponController.GetWeaponAgent((EWeaponSlotType) 3); 
            entity.playerWeaponDebug.Slot3     = agent.WeaponKey;
            entity.playerWeaponDebug.ConfigId3 = agent.ConfigId;
            agent = weaponController.GetWeaponAgent((EWeaponSlotType) 4); 
            entity.playerWeaponDebug.Slot4     = agent.WeaponKey;
            entity.playerWeaponDebug.ConfigId4 = agent.ConfigId;
            agent = weaponController.GetWeaponAgent((EWeaponSlotType) 5); 
            entity.playerWeaponDebug.Slot5     = agent.WeaponKey;
            entity.playerWeaponDebug.ConfigId5 = agent.ConfigId;
            agent = weaponController.GetWeaponAgent((EWeaponSlotType) 6); 
            entity.playerWeaponDebug.Slot6     = agent.WeaponKey;
            entity.playerWeaponDebug.ConfigId6 = agent.ConfigId;
            entity.playerWeaponDebug.DebugAutoMove = GlobalConst.autoMoveSignal;
          //  entity.StatisticsController().ShowShootStatisticsTip(DebugConfig.AppendShootArchiveText);               

        }
    }
}