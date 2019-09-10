using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using XmlConfig;


namespace App.Shared.GameModules.Weapon
{
    //IBeforeUserCmdExecuteSystem是在cmd生成之前，加了没用
    public class PlayerWeaponCmdUpdateSystem : IUserCmdExecuteSystem
    {

        public void ExecuteUserCmd(IPlayerUserCmdGetter getter, IUserCmd cmd)
        {
            var weaponController = GameModuleManagement.Get<PlayerWeaponController>(getter.OwnerEntityKey.EntityId);
            cmd.IsAutoFire = weaponController.HeldWeaponAgent.RunTimeComponent.NeedAutoBurstShoot;
            cmd.IsAutoReload = weaponController.HeldWeaponAgent.RunTimeComponent.NeedAutoReload;
            weaponController.InternalUpdate();
            #if UNITY_EDITOR
            if (cmd.IsYDown)
            {
                
            }
            #endif    
        }
    }
}