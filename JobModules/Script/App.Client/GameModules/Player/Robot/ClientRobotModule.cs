using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.ClientSession;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player.Robot.Module;
using Core.GameModule.Module;

namespace App.Client.GameModules.Player.Robot
{
    class ClientRobotModule : GameModule
    {
        public ClientRobotModule(Contexts contexts)
        {
            AddSystem(new RobotUserCmdProviderInitSystem(contexts));
            AddSystem(new NavMeshBridgeSystem(contexts.player));
            AddSystem(new RobotBehaviorLoadSystem(contexts));
        }
    }
}
