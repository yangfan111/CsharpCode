using Core.GameModule.Module;

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
