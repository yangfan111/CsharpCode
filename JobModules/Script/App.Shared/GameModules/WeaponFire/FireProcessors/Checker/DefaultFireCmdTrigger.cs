using Core.Utils;

namespace App.Shared.GameModules.Weapon.Behavior
{
    /// <summary>
    /// Defines the <see cref="DefaultFireCmdTrigger" />
    /// </summary>
    public class DefaultFireCmdTrigger : IFireTriggger
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(DefaultFireCmdTrigger));

        public bool IsTrigger(PlayerWeaponController controller, IWeaponCmd cmd)
        {
            if (null != cmd.FilteredInput && cmd.FilteredInput.IsInput(XmlConfig.EPlayerInput.IsLeftAttack))
            {
                return true;
            }
            else
            {
                if (null == cmd.FilteredInput)
                {
                    Logger.Error("FilteredInput in cmd should never be null !");
                }
            }
            return controller.AutoFire.HasValue && controller.AutoFire.Value>0;
        }
    }
}
