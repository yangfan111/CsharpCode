using Core.SessionState;

namespace App.Client.GameModules.Player
{
    public class ClientFreeCmdGenerateSystem : AbstractStepExecuteSystem
    {
        private Contexts _contexts;
        public ClientFreeCmdGenerateSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        protected override void InternalExecute()
        {
            var player = _contexts.player.flagSelfEntity;
            if(player.freeUserCmd.ForceUnmountWeapon)
            {
                _contexts.session.clientSessionObjects.UserCmdGenerator.SetUserCmd((cmd) => 
                {
                    cmd.IsForceUnmountWeapon = true;
                });
                player.freeUserCmd.ForceUnmountWeapon = false;
            }
            if(player.freeUserCmd.MountWeapon)
            {
                _contexts.session.clientSessionObjects.UserCmdGenerator.SetUserCmd((cmd) =>
                {
                    cmd.IsDrawWeapon = true;
                });
                player.freeUserCmd.MountWeapon= false;
            }
        }
    }
}