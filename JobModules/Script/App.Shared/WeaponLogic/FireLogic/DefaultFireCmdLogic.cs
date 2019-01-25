using Core.Utils;
using Core.WeaponLogic;
using Core.WeaponLogic.WeaponLogicInterface;

namespace App.Shared.WeaponLogic.FireLogic
{
    public class DefaultFireCmdLogic : IFireTriggger 
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultFireCmdLogic));
        public bool IsTrigger(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            if(null != cmd.FilteredInput && cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsLeftAttack))
            {
                return true;
            }
            else
            {
                if(null == cmd.FilteredInput)
                {
                    Logger.Error("FilteredInput in cmd should never be null !");
                }
            }

            if (playerEntity.hasWeaponAutoState && playerEntity.weaponAutoState.AutoFire > 0)
            {
                return true;
            }
            return false;
        }
    }
}
