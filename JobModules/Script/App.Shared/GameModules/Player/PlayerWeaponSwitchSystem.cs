using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core;
using App.Shared.GameModules.Weapon;

namespace App.Shared.GameModules.Player
{
    public class PlayerWeaponSwitchSystem : IUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponSwitchSystem));
        private Contexts _contexts;
        public PlayerWeaponSwitchSystem(Contexts contexts)
        {
            _contexts = contexts;
        } 
       /// <summary>
       /// 切换槽位
       /// </summary>
       /// <param name="owner"></param>
       /// <param name="cmd"></param>
        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
        
            if (cmd.CurWeapon == (int)EWeaponSlotType.None)
            {
                return;
            }

            var playerEntity = owner.OwnerEntity as PlayerEntity;
            if (null == playerEntity)
            {
                Logger.Error("Owner is not player");
                return;
            }

            if(null != cmd.FilteredInput && !cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsSwitchWeapon))
            {
                return;
            }
            else
            {
                if(null == cmd.FilteredInput)
                {
                    Logger.Error("FilteredInput in cmd should never be null !");
                }
            }
            var newSlot = playerEntity.ModeController().GetSlotByIndex(cmd.CurWeapon);
            playerEntity.WeaponController().SwitchIn(newSlot);
            //playerEntity.WeaponController().PureSwitchIn(newSlot);
            //{
            //    Logger.Error("No bag attached to player");
            //    return;
            //}
            //

            //    var weaponAchive = playerEntity.WeaponController();
            //   var curSlot = playerEntity.WeaponController().CurrSlotType;

            //   var newWeapon = weaponAchive.GetSlotWeaponInfo(newSlot);
            //if(newWeapon.Id < 1)
            //{
            //    playerEntity.tip.TipType = ETipType.NoWeaponInSlot;
            //    return;
            //}

        }
    }
}
