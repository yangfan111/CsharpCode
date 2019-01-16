using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Core.Bag;
using Core.Common;
using App.Shared.WeaponLogic;

namespace App.Shared.GameModules.Player
{
    public class PlayerWeaponSwitchSystem : IUserCmdExecuteSystem
    {
        public PlayerWeaponSwitchSystem()
        {
        } 
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerWeaponSwitchSystem));
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
            if (!playerEntity.hasBag)
            {
                Logger.Error("No bag attached to player");
                return;
            }

            var newSlot = playerEntity.modeLogic.ModeLogic.GetSlotByIndex(cmd.CurWeapon);
            var curSlot = playerEntity.GetBagLogicImp().HeldSlotType;
            
            var newWeapon = playerEntity.GetBagLogicImp().GetSlot_WeaponInfo(newSlot);
            if(newWeapon.Id < 1)
            {
                playerEntity.tip.TipType = ETipType.NoWeaponInSlot;
                return;
            }
            playerEntity.playerAction.Logic.SwitchIn(newSlot);
        }
    }
}
