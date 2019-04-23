using App.Client.GameModules.GamePlay.Free.Player;
using App.Shared.FreeFramework.framework.ai.player;
using Core.SessionState;
using System.Collections.Generic;

namespace App.Client.GameModules.GamePlay.Free.Entitas
{
    public class PlayerInterceptCmdSystem : AbstractStepExecuteSystem
    {
        private Contexts _contexts;
        private List<IPlayerCmdHandler> generators;

        public PlayerInterceptCmdSystem(Contexts contexts)
        {
            this._contexts = contexts;
            generators = new List<IPlayerCmdHandler>();
            generators.Add(new PlayerMoveCmdHandler());
            generators.Add(new PlayerKeyCmdHandler());
            generators.Add(new PlayerAttackCmdHandler());
        }

        protected override void InternalExecute()
        {
            PlayerEntity player = _contexts.player.flagSelfEntity;

            if (player != null)
            {
                for (int i = 0; i < generators.Count; i++)
                {
                    if (generators[i].CanHandle(_contexts, player, player.userCmd.LastTemp))
                    {
                        generators[i].Handle(_contexts, player, player.userCmd.LastTemp);
                    }
                }
            }
        }
    }
}
