using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components;
using Core.GameModule.Module;
using Core.GameModule.System;
using Core.SessionState;

namespace App.Client.GameModules.GamePlay.Free.Entitas
{
    public class ClientFreeMoveModule : GameModule
    {
        public ClientFreeMoveModule(Contexts contexts)
        {
            AddSystem(new FreeMoveAddSystem(contexts.freeMove));
            AddSystem(new FreeMoveCleanupSystem(contexts.freeMove));
            AddSystem(new AirPlaneInitSystem(contexts.freeMove));
        }
    }
}
