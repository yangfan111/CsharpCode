using Core.Utils;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="DefaultFireCmdTrigger" />
    /// </summary>
    public class DefaultFireCmdTrigger : IFireTriggger
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultFireCmdTrigger));

        public bool IsTrigger(WeaponSideCmd cmd)
        {
            return cmd.FiltedInput(XmlConfig.EPlayerInput.IsLeftAttack)||
                   cmd.UserCmd.IsAutoFire;
        }
    }
}
