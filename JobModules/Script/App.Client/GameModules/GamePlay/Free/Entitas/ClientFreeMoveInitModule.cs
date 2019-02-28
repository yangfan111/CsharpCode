using Core.GameModule.Module;

namespace App.Client.GameModules.GamePlay.Free.Entitas
{
    public class ClientFreeMoveInitModule : GameModule
    {
        public ClientFreeMoveInitModule(Contexts contexts)
        {
            AddSystem(new AirPlaneInitSystem(contexts.freeMove));
        }
    }
}
