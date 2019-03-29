using System.Collections.Generic;
using App.Shared.GameModules.Vehicle.WheelCarrier;
using Core.GameModule.Module;

namespace App.Client.GameModules.ClientEffect
{
    public class ClientEffectModule : GameModule
    {
        public ClientEffectModule(Contexts contexts) 
        {
            AddSystem(new ClientEffectEntityInitSystem(contexts));
            AddSystem(new ClientEffectRenderSystem(contexts));
            AddSystem(new ClientEffectCleanUpSystem(contexts.clientEffect));
            AddSystem(new ClientEffectLimitSystem(contexts));
        }
    }

    
}