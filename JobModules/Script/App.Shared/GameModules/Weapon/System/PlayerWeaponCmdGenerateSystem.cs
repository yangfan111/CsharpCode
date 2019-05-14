using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;
using XmlConfig;


namespace App.Shared.GameModules.Weapon
{
    //IBeforeUserCmdExecuteSystem是在cmd生成之前，加了没用
    public class PlayerWeaponCmdGenerateSystem : IUserCmdExecuteSystem
    {
       

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            var weaponController = GameModuleManagement.Get<PlayerWeaponController>(owner.OwnerEntityKey.EntityId);
            cmd.IsAutoFire = weaponController.HeldWeaponAgent.RunTimeComponent.NeedAutoBurstShoot;
            cmd.IsAutoReload = weaponController.HeldWeaponAgent.RunTimeComponent.NeedAutoReload;
        }
    }
}