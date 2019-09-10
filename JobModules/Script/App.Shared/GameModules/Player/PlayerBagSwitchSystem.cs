using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.framework.trigger;
using com.wd.free.@event;
using Core.Free;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Prediction.UserPrediction.Cmd;
using XmlConfig;

namespace App.Shared.GameModules.Player
{
    public class PlayerBagSwitchSystem : IUserCmdExecuteSystem
    {
        private ICommonSessionObjects _commonSessionObjects;
        private Contexts _contexts;

        public PlayerBagSwitchSystem(ICommonSessionObjects commonSessionObjects, Contexts contexts)
        {
            _commonSessionObjects = commonSessionObjects;
            _contexts = contexts;
        }

        public void ExecuteUserCmd(IPlayerUserCmdGetter getter, IUserCmd cmd)
        {

            if (cmd.BagIndex > 0)
            {
                var player = getter.OwnerEntity as PlayerEntity;
                if (player.StateInteractController().GetCurrStates().Contains(EPlayerState.SwitchWeapon))
                    return;

                player.WeaponController().SwitchBag(cmd.BagIndex-1);

                var args = _contexts.session.commonSession.FreeArgs as IEventArgs;
                if (args != null)
                {
                    args.Trigger(FreeTriggerConstant.PLAYER_SWITCH_BAG, new TempUnit("current", (FreeData) player.freeData.FreeData));
                }
            }
        }
    }
}
