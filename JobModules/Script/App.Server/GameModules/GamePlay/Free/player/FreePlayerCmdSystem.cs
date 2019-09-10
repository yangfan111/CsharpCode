using Core.GameModule.Interface;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Server.GameModules.GamePlay.free.player
{
    public class FreePlayerCmdSystem : IUserCmdExecuteSystem
    {
        private Contexts _contexts;
        private ServerRoom _room;

        public FreePlayerCmdSystem(Contexts contexts, ServerRoom room)
        {
            this._room = room;
            this._contexts = contexts;
        }

        public void ExecuteUserCmd(IPlayerUserCmdGetter owner, IUserCmd cmd)
        {
            this._room.GameRule.PlayerPressCmd(_contexts, owner.OwnerEntity as PlayerEntity, cmd);
        }
    }
}
